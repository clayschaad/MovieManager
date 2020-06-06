using MovieCleaner.MovieDatabase;
using MovieCleaner.VideoInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MovieCleaner
{
    public class MovieFile
    {
        public string Filename { get; set; }
        public string FullPath { get; set; }
        public string FullPathToFile { get; set; }
        public string Title { get; set; }
        public bool Include { get; set; }
        public List<string> AudioLanguages {get; set; }
        public List<string> SubtitleLanguages { get; set; }
        public List<MovieSearchResult> MovieSearchResults { get; set; }


        public static readonly string[] IgnoreFolders = new[] { "clipinf", "playlist", "bdmv", "sample", "certificate", "proof" };

        public static readonly string[] AllowedExtensions = new[] { "mkv", "avi", "mpg", "mpeg" };

        private string[] stopWords = new[] {"german", "uncut", "special extended edition", "unrated", "extended se" };

        public MovieFile(string fullPathToFile)
        {
            FullPathToFile = fullPathToFile;

            Filename = Path.GetFileName(fullPathToFile);
            FullPath = Path.GetDirectoryName(fullPathToFile);
            Title = ParseTitle(FullPath, stopWords);

            var mediaInfoParser = new MediaInfoParser(fullPathToFile);
            AudioLanguages = mediaInfoParser.GetAudioLanguages();
            SubtitleLanguages = mediaInfoParser.GetSubtitlesLanguages();

            var movieInfos = MovieDatabaseClient.SearchMovie(Title);
            MovieSearchResults = movieInfos.Results;

            Include = true;
        }

        // for serializer
        public MovieFile()
        {
                    }

        public void SetTitle(string title)
        {
            Title = title;
        }

        public void DoInclude(bool include)
        {
            Include = include;
        }

        public string GetInfo()
        {
            return Filename + Environment.NewLine + FullPath.Replace("\\", Environment.NewLine);
        }

        private static string ParseTitle(string fullDirectory, string[] stopWords)
        {
            var title = Path.GetFileNameWithoutExtension(fullDirectory);
            title = title.Replace('.', ' ');
            foreach (var stopWord in stopWords)
            {
                var pos = title.IndexOf(stopWord, StringComparison.InvariantCultureIgnoreCase);
                if (pos > 0)
                {
                    title = title.Substring(0, pos - 1);
                }
            }

            var regex = new Regex(@"[1-2]{1}[0-9]{3}");
            title = regex.Split(title)[0].Trim();

            return title;
        }
    }
}
