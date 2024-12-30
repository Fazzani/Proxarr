using Proxarr.Api.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Proxarr.Api.Core
{
    [ExcludeFromCodeCoverage]
    public class MediaAddedJsonConverter : JsonConverter<MediaAdded>
    {
        public override MediaAdded Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            var eventType = root.GetProperty("eventType").GetString();

            if (eventType?.Equals("MovieAdded", StringComparison.OrdinalIgnoreCase) == true)
            {
                return JsonSerializer.Deserialize<MovieAdded>(root.GetRawText()) ?? throw new FormatException($"Malformed json content to deserialize {nameof(MovieAdded)} object");
            }
            else if (eventType?.Equals("SeriesAdd", StringComparison.OrdinalIgnoreCase) == true)
            {
                return JsonSerializer.Deserialize<TvAdded>(root.GetRawText()) ?? throw new FormatException($"Malformed json content to deserialize {nameof(TvAdded)} object");
            }
            else if (eventType?.Equals("test", StringComparison.OrdinalIgnoreCase) == true)
            {
                return JsonSerializer.Deserialize<MediaAdded>(root.GetRawText()) ?? throw new FormatException($"Malformed json content to deserialize {nameof(TvAdded)} object");
            }
            else
            {
                throw new JsonException();
            }
        }

        public override void Write(Utf8JsonWriter writer, MediaAdded value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
