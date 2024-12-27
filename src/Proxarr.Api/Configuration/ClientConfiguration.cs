namespace Proxarr.Api.Configuration
{
    public sealed class ClientConfiguration
    {
        public const string SECTION_NAME = $"{AppConfiguration.SECTION_NAME}:Clients";
        public required string ApiKey { get; set; }
        public required string BaseUrl { get; set; }
    }
}
