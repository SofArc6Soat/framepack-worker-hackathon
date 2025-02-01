using Core.Infra.S3;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace Gateways.Handlers;

public class ArquivoHandler : IArquivoHandler
{
    private readonly ILogger<ArquivoHandler> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _processingPath = Path.Combine(Path.GetTempPath(), "video-processing");
    private readonly string _outputPath = Path.Combine(Path.GetTempPath(), "video-output");

    public ArquivoHandler(ILogger<ArquivoHandler> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<string> DownloadVideoAsync(Guid id, string urlArquivoVideo)
    {
        _logger.LogInformation("Iniciando - Download video - Id: {Id}", id);

        var videoPath = Path.Combine(_processingPath, $"{id}.mp4");

        Directory.CreateDirectory(_processingPath);

        var response = await _httpClient.GetAsync(urlArquivoVideo);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Download falhou. StatusCode: {response.StatusCode}");
        }

        var videoBytes = await response.Content.ReadAsByteArrayAsync();
        await File.WriteAllBytesAsync(videoPath, videoBytes);

        _logger.LogInformation("Finalizado - Download video - Id: {Id}", id);

        return videoPath;
    }

    public async Task<string> CompactarUploadFramesAsync(Guid id, string framesPath, string videoPath, IS3Service s3Service)
    {
        _logger.LogInformation("Iniciando -  Compactação frames - Id: {Id}", id);

        var zipPath = Path.Combine(_outputPath, $"{id}.zip");
        Directory.CreateDirectory(_outputPath);

        ZipFile.CreateFromDirectory(framesPath, zipPath, CompressionLevel.Optimal, false);
        Directory.Delete(framesPath, true);
        File.Delete(videoPath);

        _logger.LogInformation("Finalizado - Compactação frames - Id: {Id}", id);

        return await UploadZipFramesAsync(_logger, id, s3Service, zipPath);
    }

    private static async Task<string> UploadZipFramesAsync(ILogger<ArquivoHandler> logger, Guid id, IS3Service s3Service, string zipPath)
    {
        logger.LogInformation("Iniciando - Upload Zip frames - Id: {Id}", id);

        var url = await s3Service.UploadArquivoAsync(zipPath);
        File.Delete(zipPath);

        logger.LogInformation("Finalizado - Upload Zip frames - Id: {Id}", id);
        return url;
    }

    public async Task LimpezaAsync(Guid id, string urlArquivoVideo, IS3Service s3Service)
    {
        _logger.LogInformation("Iniciando - Limpeza - Id: {Id}", id);

        var videoPath = Path.Combine(_processingPath, $"{id}.mp4");
        var framesPath = Path.Combine(_processingPath, $"{id}");

        if (Directory.Exists(framesPath))
        {
            Directory.Delete(framesPath, true);
        }

        if (File.Exists(videoPath))
        {
            File.Delete(videoPath);
        }

        await s3Service.DeletarArquivoAsync(urlArquivoVideo);

        _logger.LogInformation("Finalizado - Limpeza - Id: {Id}", id);
    }
}
