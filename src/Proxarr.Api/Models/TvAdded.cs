using System.Text.Json.Serialization;

namespace Proxarr.Api.Models
{
    public class TvAdded
    {
        [JsonPropertyName("series")]
        public required Series Series { get; set; }

        [JsonPropertyName("eventType")]
        public required string EventType { get; set; }

        [JsonPropertyName("instanceName")]
        public required string InstanceName { get; set; }

        [JsonPropertyName("applicationUrl")]
        public required string ApplicationUrl { get; set; }
    }

    public class Series
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("titleSlug")]
        public string? TitleSlug { get; set; }

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("tvdbId")]
        public int TvdbId { get; set; }

        [JsonPropertyName("tvMazeId")]
        public int TvMazeId { get; set; }

        [JsonPropertyName("tmdbId")]
        public int TmdbId { get; set; }

        [JsonPropertyName("imdbId")]
        public string? ImdbId { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonPropertyName("genres")]
        public string[]? Genres { get; set; }

        [JsonPropertyName("images")]
        public Image[]? Images { get; set; }

        [JsonPropertyName("tags")]
        public string[]? Tags { get; set; }
    }
}
