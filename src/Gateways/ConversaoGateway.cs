using Amazon.DynamoDBv2.DataModel;
using Core.Infra.S3;
using Domain.Entities;
using Domain.ValueObjects;
using Gateways.Handlers;
using Infra.Dto;

namespace Gateways
{
    public class ConversaoGateway(IDynamoDBContext repository, IS3Service s3Service, IVideoHandler videoHandler, IArquivoHandler arquivoHandler) : IConversaoGateway
    {
        public async Task<Conversao?> ObterConversaoAsync(Guid id, CancellationToken cancellationToken)
        {
            var conversaoDto = await repository.LoadAsync<ConversaoDb>(id, cancellationToken);
            return conversaoDto is null
                ? null
                : MapConversao(conversaoDto);
        }

        public async Task<bool> EfetuarConversaoAsync(Conversao conversao, CancellationToken cancellationToken)
        {
            if (!conversao.AlterarStatusParaProcessando(conversao.Status))
            {
                return false;
            }

            await AtualizarStatus(conversao, cancellationToken);

            try
            {
                var videoPath = await arquivoHandler.DownloadVideoAsync(conversao.Id, s3Service.GerarPreSignedUrl(conversao.UrlArquivoVideo));
                var framesPath = await videoHandler.ExtrairFramesAsync(conversao.Id, videoPath);

                if (conversao.AlterarStatusParaCompactando(conversao.Status))
                {
                    await AtualizarStatus(conversao, cancellationToken);

                    var urlArquivoCompactado = await arquivoHandler.CompactarUploadFramesAsync(conversao.Id, framesPath, videoPath, s3Service);

                    if (!string.IsNullOrEmpty(urlArquivoCompactado))
                    {
                        conversao.AlterarStatusParaConcluido(conversao.Status);
                        conversao.SetUrlArquivoCompactado(urlArquivoCompactado);

                        await AtualizarStatus(conversao, cancellationToken);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                conversao.AlterarStatusParaErro();
                await AtualizarStatus(conversao, cancellationToken);
                throw;
            }
            finally
            {
                await arquivoHandler.LimpezaAsync(conversao.Id, conversao.UrlArquivoVideo, s3Service);
            }

            return false;
        }

        public async Task<bool> DownloadEfetuadoAsync(Conversao conversao, CancellationToken cancellationToken)
        {
            if (await arquivoHandler.ExcluiArquivoAposDownloadAsync(conversao.Id, conversao.UrlArquivoCompactado, s3Service))
            {
                conversao.AlterarStatusParaDownloadEfetuado();

                await AtualizarStatus(conversao, cancellationToken);

                return true;
            }

            return false;
        }


        private async Task AtualizarStatus(Conversao conversao, CancellationToken cancellationToken) =>
            await repository.SaveAsync(EntityToDb(conversao), cancellationToken);

        private static Conversao MapConversao(ConversaoDb db)
        {
            var conversao = new Conversao(db.Id, db.UsuarioId, db.Data, Enum.Parse<Status>(db.Status), db.NomeArquivo, db.UrlArquivoVideo);
            conversao.SetUrlArquivoCompactado(db.UrlArquivoCompactado);

            return conversao;
        }

        private static ConversaoDb EntityToDb(Conversao conversao) => new()
        {
            Id = conversao.Id,
            UsuarioId = conversao.UsuarioId,
            Status = conversao.Status.ToString(),
            Data = conversao.Data,
            NomeArquivo = conversao.NomeArquivo,
            UrlArquivoVideo = conversao.UrlArquivoVideo,
            UrlArquivoCompactado = conversao.UrlArquivoCompactado
        };
    }
}
