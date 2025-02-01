namespace Core.WebApi.Configurations;

public class JwtBearerConfigureOptions
{
    private string _authority = string.Empty;
    private string _metadataAddress = string.Empty;

    public string Authority
    {
        get => _authority;
        set => _authority = value ?? throw new ArgumentNullException(nameof(Authority));
    }

    public string MetadataAddress
    {
        get => _metadataAddress;
        set => _metadataAddress = value ?? throw new ArgumentNullException(nameof(MetadataAddress));
    }
}