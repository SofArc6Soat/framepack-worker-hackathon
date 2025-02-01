using Microsoft.Extensions.Logging;
using Xabe.FFmpeg;

namespace Gateways.Handlers;

public class VideoHandler : IVideoHandler
{
    private readonly string _ffmpegPath = Path.Combine(AppContext.BaseDirectory, "Configuration/ffmpeg");
    private readonly ILogger<VideoHandler> logger;

    public VideoHandler(ILogger<VideoHandler> _logger)
    {
        logger = _logger;
        FFmpeg.SetExecutablesPath(_ffmpegPath);
    }

    public async Task<string> ExtrairFramesAsync(Guid id, string videoPath)
    {
        logger.LogInformation("Iniciando - Extração de frames - Id: {Id}", id);

        var framesPath = Path.Combine(Path.GetTempPath(), "video-processing", $"{id}");
        Directory.CreateDirectory(framesPath);

        var mediaInfo = await FFmpeg.GetMediaInfo(videoPath);

        _ = mediaInfo.VideoStreams.FirstOrDefault()
            ?? throw new FileNotFoundException("Nenhum stream de vídeo encontrado.");

        await FFmpeg.Conversions.New()
            .AddParameter($"-i {videoPath} -vf fps=20 {Path.Combine(framesPath, "frame_%03d.jpg")}")
            .Start();

        logger.LogInformation("Finalizado - Extração de frames - Id: {Id}", id);

        return framesPath;
    }
}