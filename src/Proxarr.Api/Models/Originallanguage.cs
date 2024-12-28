using System.Text.Json.Serialization;

namespace Proxarr.Api.Models
{
    public class Originallanguage
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
