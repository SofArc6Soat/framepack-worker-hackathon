using Amazon.DynamoDBv2.DataModel;
using Core.Infra.S3;
using Domain.Entities;
using Domain.ValueObjects;
using Infra.Dto;
using System.IO.Compression;
using Xabe.FFmpeg;

namespace Gateways
{
    public class ConversaoGateway(IDynamoDBContext repository, IS3Service s3Service) : IConversaoGateway
    {
        private static readonly string LocalProcessamentoPath = Path.Combine(Path.GetTempPath(), "video-processing");
        private static readonly string LocalSaidaPath = Path.Combine(Path.GetTempPath(), "video-output");

        public async Task<Conversao?> ObterConversaoAsync(Guid id, CancellationToken cancellationToken)
        {
            var conversaoDto = await repository.LoadAsync<ConversaoDb>(id, cancellationToken);

            if (conversaoDto is null)
            {
                return null;
            }

            var status = (Status)Enum.Parse(typeof(Status), conversaoDto.Status, ignoreCase: true);

            return new Conversao(conversaoDto.Id, conversaoDto.UsuarioId, conversaoDto.Data, status, conversaoDto.NomeArquivo, conversaoDto.UrlArquivoVideo);
        }

        public async Task<bool> EfetuarConversaoAsync(Conversao conversao, CancellationToken cancellationToken)
        {
            if (conversao.AlterarStatusParaProcessando(conversao.Status))
            {
                await repository.SaveAsync(EntityToDb(conversao), cancellationToken);

                Directory.CreateDirectory(LocalProcessamentoPath);
                Directory.CreateDirectory(LocalSaidaPath);

                var arquivoVideoPath = await EfetuarDownloadVideoAsync(conversao.Id, conversao.UrlArquivoVideo, s3Service);

                var framesPath = await ExtrairFramesAsync(conversao.Id, arquivoVideoPath);

                if (conversao.AlterarStatusParaCompactando(conversao.Status))
                {
                    await repository.SaveAsync(EntityToDb(conversao), cancellationToken);

                    var urlArquivoCompactado = await CompactarFramesAsync(conversao.Id, s3Service, arquivoVideoPath, framesPath);

                    if (!string.IsNullOrEmpty(urlArquivoCompactado))
                    {
                        conversao.AlterarStatusParaConcluido(conversao.Status);
                        conversao.SetUrlArquivoCompactado(urlArquivoCompactado);

                        await repository.SaveAsync(EntityToDb(conversao), cancellationToken);

                        return true;
                    }

                    await ConversaoComErro(repository, conversao, cancellationToken);

                    return false;
                }
            }

            await ConversaoComErro(repository, conversao, cancellationToken);

            return false;
        }

        private static async Task ConversaoComErro(IDynamoDBContext repository, Conversao conversao, CancellationToken cancellationToken)
        {
            conversao.AlterarStatusParaErro();
            await repository.SaveAsync(EntityToDb(conversao), cancellationToken);
        }

        private static async Task<string> CompactarFramesAsync(Guid id, IS3Service s3Service, string arquivoVideoPath, string framesPath)
        {
            var arquivoZipPath = Path.Combine(LocalSaidaPath, $"{id}.zip");
            ZipFile.CreateFromDirectory(framesPath, arquivoZipPath);

            Directory.Delete(framesPath, true);
            File.Delete(arquivoVideoPath);

            var urlArquivoCompactado = await s3Service.UploadArquivoAsync(arquivoZipPath);

            File.Delete(arquivoZipPath);

            if (string.IsNullOrEmpty(urlArquivoCompactado))
            {
                return string.Empty;
            }

            await s3Service.DeletarArquivoAsync(arquivoVideoPath);

            return urlArquivoCompactado;
        }

        private static async Task<string> ExtrairFramesAsync(Guid id, string arquivoVideoPath)
        {
            var framesPath = Path.Combine(LocalProcessamentoPath, $"{id}");
            Directory.CreateDirectory(framesPath);

            var ffmpegPath = Path.Combine(AppContext.BaseDirectory, "Configuration\\ffmpeg");
            FFmpeg.SetExecutablesPath(ffmpegPath);

            var mediaInfo = await FFmpeg.GetMediaInfo(arquivoVideoPath);

            _ = mediaInfo.VideoStreams.FirstOrDefault() ?? throw new Exception("Nenhum video stream encontrado no arquivo.");

            // Extrair frames a cada 20 segundo
            await FFmpeg.Conversions.New()
                .AddParameter($"-i {arquivoVideoPath} -vf fps=20 {Path.Combine(framesPath, "frame_%03d.jpg")}")
                .Start();

            return framesPath;
        }

        private static async Task<string> EfetuarDownloadVideoAsync(Guid id, string urlArquivoVideo, IS3Service s3Service)
        {
            var videoPath = Path.Combine(LocalProcessamentoPath, $"{id}.mp4");

            using var httpClient = new HttpClient();

            var url = s3Service.GerarPreSignedUrl(urlArquivoVideo);

            try
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Falha ao efetuar downloado do arquivo. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                }

                var videoBytes = await httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(videoPath, videoBytes);

                return videoPath;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request falhou: {ex.Message}");
                throw;
            }
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
