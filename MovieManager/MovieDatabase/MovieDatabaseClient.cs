using System.Net.Http;
using System.Text.Json;

namespace MovieCleaner.MovieDatabase
{
    class MovieDatabaseClient
    {
        private const string ApiKey = "";

        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// https://developers.themoviedb.org/3/search/search-movies
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static MovieSearch SearchMovie(string title)
        {
            var url = $"https://api.themoviedb.org/3/search/movie?api_key={ApiKey}&language=de-CH&query={title}";
            var movieSearchTask = client.GetStringAsync(url).Result;
            var movieSearch = JsonSerializer.Deserialize<MovieSearch>(movieSearchTask);
            return movieSearch;
        }
    }
}
