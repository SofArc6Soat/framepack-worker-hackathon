using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;

namespace Core.Infra.S3
{
    public class S3Service(IAmazonS3 s3Client)
    {
        private const string BucketName = "amzn-s3-bucket-26bda3ac-c185-4185-a9f8-d3697a89754c-framepack";

        public async Task<string> UploadArquivoAsync(Guid id, IFormFile arquivo)
        {
            var key = string.Concat("aguardando-processamento/", id, Path.GetExtension(arquivo.FileName).ToLowerInvariant());

            try
            {
                using var memoryStream = new MemoryStream();
                await arquivo.CopyToAsync(memoryStream);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = memoryStream,
                    Key = key,
                    BucketName = BucketName,
                    ContentType = arquivo.ContentType
                };

                var transferUtility = new TransferUtility(s3Client);
                await transferUtility.UploadAsync(uploadRequest);

                var existente = await VerificarExistenciaArquivo(BucketName, key);
                return !existente ? string.Empty : key;
            }
            catch (AmazonS3Exception ex)
            {
                throw new AmazonS3Exception($"Erro ao efetuar o upload do arquivo: {ex.Message}", ex);
            }
        }

        public async Task<string> UploadArquivoAsync(string caminhoArquivo)
        {
            var nomeArquivo = Path.GetFileName(caminhoArquivo);
            var key = $"processado/{nomeArquivo}";

            try
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    FilePath = caminhoArquivo,
                    BucketName = BucketName,
                    Key = key
                };

                var transferUtility = new TransferUtility(s3Client);
                await transferUtility.UploadAsync(uploadRequest);

                var existente = await VerificarExistenciaArquivo(BucketName, key);
                return !existente ? string.Empty : key;
            }
            catch (AmazonS3Exception ex)
            {
                throw new AmazonS3Exception($"Erro ao efetuar o upload do arquivo: {ex.Message}", ex);
            }
        }

        public string GerarPreSignedUrl(string key, int duracaoMinutos = 120)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(duracaoMinutos)
            };

            return s3Client.GetPreSignedURL(request);
        }

        public async Task DeletarArquivoAsync(string key)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = BucketName,
                    Key = key
                };

                await s3Client.DeleteObjectAsync(deleteObjectRequest);
                Console.WriteLine($"Arquivo deletado com sucesso.");
            }
            catch (AmazonS3Exception ex)
            {
                throw new AmazonS3Exception($"Erro ao deletar o arquivo': {ex.Message}", ex);
            }
        }

        private async Task<bool> VerificarExistenciaArquivo(string bucketName, string key)
        {
            try
            {
                var response = await s3Client.GetObjectMetadataAsync(bucketName, key);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
    }
}