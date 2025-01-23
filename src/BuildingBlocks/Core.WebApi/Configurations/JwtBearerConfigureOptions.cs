namespace Core.WebApi.Configurations
{
    public class JwtBearerConfigureOptions
    {
        public string Authority { get; set; } = string.Empty;
        public string MetadataAddress { get; set; } = string.Empty;
    }
}
