namespace Proxarr.Api.Configuration
{
    public sealed class ClientConfiguration
    {
        public required string ApiKey { get; set; }
        public required string BaseUrl { get; set; }
    }
}
