using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MovieCleaner.MovieDatabase
{
    public class MovieSearch
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("results")]
        public List<MovieSearchResult> Results { get; set; }
    }

    public class MovieSearchResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("original_title")]
        public string OriginalTitle { get; set; }

        [JsonPropertyName("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonPropertyName("poster_path")]
        public string PosterPath { get; set; }

        public string ImageUrl => $"https://image.tmdb.org/t/p/w500{PosterPath}";

    }
}
