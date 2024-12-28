using System.Text.Json.Serialization;

namespace Proxarr.Api.Models
{
    public class MovieAdded : MediaAdded
    {
        [JsonPropertyName("movie")]
        public required Movie Movie { get; set; }

        [JsonPropertyName("addMethod")]
        public string? AddMethod { get; set; }
    }

    public class Movie
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }

        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("releaseDate")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("folderPath")]
        public required string FolderPath { get; set; }

        [JsonPropertyName("tmdbId")]
        public int TmdbId { get; set; }

        [JsonPropertyName("imdbId")]
        public string? ImdbId { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("genres")]
        public string[]? Genres { get; set; }

        [JsonPropertyName("images")]
        public Image[]? Images { get; set; }

        [JsonPropertyName("tags")]
        public string[]? Tags { get; set; }

        [JsonPropertyName("originalLanguage")]
        public Originallanguage? OriginalLanguage { get; set; }
    }
}
