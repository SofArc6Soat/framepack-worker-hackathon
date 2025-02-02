using Core.Infra.S3;

namespace Gateways.Handlers
{
    public interface IArquivoHandler
    {
        /// <summary>
        /// Faz o download de um vídeo para um caminho temporário local.
        /// </summary>
        Task<string> DownloadVideoAsync(Guid id, string urlArquivoVideo);

        /// <summary>
        /// Compacta os frames gerados, faz o upload para o S3 e retorna a URL.
        /// </summary>
        Task<string> CompactarUploadFramesAsync(Guid id, string framesPath, string videoPath, IS3Service s3Service);

        /// <summary>
        /// Realiza a limpeza dos arquivos locais e no S3.
        /// </summary>
        Task LimpezaAsync(Guid id, string urlArquivoVideo, IS3Service s3Service);

        /// <summary>
        /// Realiza a limpeza dos arquivos do S3 após download.
        /// </summary>
        Task<bool> ExcluiArquivoAposDownloadAsync(Guid id, string urlArquivoVideo, IS3Service s3Service);
    }
}