using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Proxarr.Api.Models
{
    [ExcludeFromCodeCoverage]
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
