using System.Text.Json.Serialization;

namespace Proxarr.Api.Models
{
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
