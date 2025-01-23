using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;

namespace Core.Infra.S3
{
    public class S3Service(IAmazonS3 s3Client)
    {
        private const string BucketName = "amzn-s3-bucket-26bda3ac-c185-4185-a9f8-d3697a89754c-framepack";

        public async Task<string> UploadFileAsync(IFormFile file, Guid userId)
        {
            var key = $"{userId}/{file.FileName}";

            try
            {
                using var newMemoryStream = new MemoryStream();
                await file.CopyToAsync(newMemoryStream);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = key,
                    BucketName = BucketName,
                    ContentType = file.ContentType
                };

                var transferUtility = new TransferUtility(s3Client);
                await transferUtility.UploadAsync(uploadRequest);

                var exists = await DoesS3ObjectExistAsync(BucketName, key);
                return !exists ? string.Empty : $"https://{BucketName}.s3.amazonaws.com/{key}";
            }
            catch (AmazonS3Exception ex)
            {
                throw new AmazonS3Exception($"Erro ao efetuar o upload do arquivo: {ex.Message}", ex);
            }
        }

        private async Task<bool> DoesS3ObjectExistAsync(string bucketName, string key)
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