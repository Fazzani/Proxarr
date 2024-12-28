using System.Text.Json.Serialization;

namespace Proxarr.Api.Models
{
    public class MediaAdded
    {
        [JsonPropertyName("eventType")]
        public required string EventType { get; set; }

        [JsonPropertyName("instanceName")]
        public string? InstanceName { get; set; }

        [JsonPropertyName("applicationUrl")]
        public required string ApplicationUrl { get; set; }
    }
}
