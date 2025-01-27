namespace Gateways.Handlers
{
    public interface IVideoHandler
    {
        /// <summary>
        /// Extrai frames de um vídeo e os armazena em um diretório temporário.
        /// </summary>
        Task<string> ExtrairFramesAsync(Guid id, string videoPath);
    }
}