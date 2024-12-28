using System.Text.Json.Serialization;

namespace Proxarr.Api.Models
{
    public class MediaAdded
    {
        [JsonPropertyName("eventType")]
        public required string EventType { get; set; }

        [JsonPropertyName("instanceName")]
        public required string InstanceName { get; set; }

        [JsonPropertyName("applicationUrl")]
        public required string ApplicationUrl { get; set; }
    }
}
