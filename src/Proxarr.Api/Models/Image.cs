using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Proxarr.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class Image
    {
        [JsonPropertyName("coverType")]
        public string? CoverType { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("remoteUrl")]
        public string? RemoteUrl { get; set; }
    }
}
