using Amazon.DynamoDBv2.DataModel;
using Core.Infra.EmailSender;
using Core.Infra.S3;
using Domain.Entities;
using Domain.ValueObjects;
using Gateways.Handlers;
using Infra.Dto;

namespace Gateways
{
    public class ConversaoGateway(
        IDynamoDBContext repository,
        IS3Service s3Service,
        IVideoHandler videoHandler,
        IArquivoHandler arquivoHandler,
        IEmailService emailService) : IConversaoGateway
    {
        public async Task<Conversao?> ObterConversaoAsync(Guid id, CancellationToken cancellationToken)
        {
            var conversaoDto = await repository.LoadAsync<ConversaoDb>(id, cancellationToken);
            return conversaoDto is null ? null : MapConversao(conversaoDto);
        }

        public async Task<bool> EfetuarConversaoAsync(Conversao conversao, CancellationToken cancellationToken)
        {
            if (!conversao.AlterarStatusParaProcessando(conversao.Status))
            {
                await TratarErroConversaoAsync(conversao, cancellationToken);
                return false;
            }

            await AtualizarStatusAsync(conversao, cancellationToken);

            try
            {
                var preSignedUrl = s3Service.GerarPreSignedUrl(conversao.UrlArquivoVideo);
                var videoPath = await arquivoHandler.DownloadVideoAsync(conversao.Id, preSignedUrl);
                var framesPath = await videoHandler.ExtrairFramesAsync(conversao.Id, videoPath);

                if (!conversao.AlterarStatusParaCompactando(conversao.Status))
                {
                    await TratarErroConversaoAsync(conversao, cancellationToken);
                    return false;
                }

                await AtualizarStatusAsync(conversao, cancellationToken);

                var urlArquivoCompactado = await arquivoHandler.CompactarUploadFramesAsync(conversao.Id, framesPath, videoPath, s3Service);

                if (string.IsNullOrEmpty(urlArquivoCompactado))
                {
                    await TratarErroConversaoAsync(conversao, cancellationToken);
                    return false;
                }

                conversao.AlterarStatusParaConcluido(conversao.Status);
                conversao.SetUrlArquivoCompactado(urlArquivoCompactado);

                await AtualizarStatusAsync(conversao, cancellationToken);
                await EnviarEmailConversaoAsync(conversao, sucesso: true);
                return true;
            }
            catch (Exception)
            {
                await TratarErroConversaoAsync(conversao, cancellationToken);
                throw;
            }
            finally
            {
                await arquivoHandler.LimpezaAsync(conversao.Id, conversao.UrlArquivoVideo, s3Service);
            }
        }

        public async Task<bool> DownloadEfetuadoAsync(Conversao conversao, CancellationToken cancellationToken)
        {
            if (await arquivoHandler.ExcluiArquivoAposDownloadAsync(conversao.Id, conversao.UrlArquivoCompactado, s3Service))
            {
                conversao.AlterarStatusParaDownloadEfetuado();
                await AtualizarStatusAsync(conversao, cancellationToken);
                return true;
            }

            return false;
        }

        private async Task TratarErroConversaoAsync(Conversao conversao, CancellationToken cancellationToken)
        {
            conversao.AlterarStatusParaErro();
            await AtualizarStatusAsync(conversao, cancellationToken);
            await EnviarEmailConversaoAsync(conversao, sucesso: false);
        }

        private async Task AtualizarStatusAsync(Conversao conversao, CancellationToken cancellationToken) =>
            await repository.SaveAsync(EntityToDb(conversao), cancellationToken);

        private async Task EnviarEmailConversaoAsync(Conversao conversao, bool sucesso)
        {
            var assunto = sucesso
                ? "Conversão realizada com sucesso"
                : "Conversão realizada com erro";

            var mensagem = sucesso
                ? $"Olá, a conversão do seu vídeo foi realizada com sucesso! Acesse o Framepack para realizar o download. Informe o Id: {conversao.Id} para download."
                : $"Olá, não foi possível realizar a conversão do seu vídeo! Tente novamente mais tarde. Id da conversão: {conversao.Id}";

            await emailService.SendEmailAsync(conversao.EmailUsuario, assunto, mensagem);
        }

        private static Conversao MapConversao(ConversaoDb db)
        {
            var conversao = new Conversao(
                db.Id,
                db.UsuarioId,
                db.Data,
                Enum.Parse<Status>(db.Status),
                db.NomeArquivo,
                db.UrlArquivoVideo);

            conversao.SetUrlArquivoCompactado(db.UrlArquivoCompactado);
            conversao.SetEmailUsuario(db.EmailUsuario);

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
