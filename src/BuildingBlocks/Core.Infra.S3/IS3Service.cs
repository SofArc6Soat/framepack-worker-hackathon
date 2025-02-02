using Microsoft.AspNetCore.Http;

namespace Core.Infra.S3
{
    public interface IS3Service
    {
        Task<string> UploadArquivoAsync(Guid id, IFormFile arquivo);
        Task<string> UploadArquivoAsync(string caminhoArquivo);
        string GerarPreSignedUrl(string key, int duracaoMinutos = 120);
        Task DeletarArquivoAsync(string key);
    }
}
