using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Proxarr.Api.Models
{
    [ExcludeFromCodeCoverage]
    public class Originallanguage
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
