using System.Collections.Generic;
using System.Linq;
using NReco.VideoInfo;

namespace MovieCleaner.VideoInfo
{
    public class MediaInfoParser
    {
        private MediaInfo mediaInfo;

        public MediaInfoParser(string fullPathToFile)
        {
            try
            {
                var ffProbe = new FFProbe();
                mediaInfo = ffProbe.GetMediaInfo(fullPathToFile);  
            }
            catch { }
        }

        public List<string> GetAudioLanguages()
        {
            var languages = new List<string>();
            if (mediaInfo != null)
            {
                foreach (var stream in mediaInfo.Streams.Where(m => m.CodecType == "audio"))
                {
                    var language = stream.Tags.SingleOrDefault(t => t.Key == "language");
                    if (language.Key != null)
                    {
                        languages.Add(language.Value);
                    }
                }
            }

            return languages;
        }

        public List<string> GetSubtitlesLanguages()
        {
            var languages = new List<string>();
            if (mediaInfo != null)
            {
                foreach (var stream in mediaInfo.Streams.Where(m => m.CodecType == "subtitle"))
                {
                    var language = stream.Tags.SingleOrDefault(t => t.Key == "language");
                    if (language.Key != null)
                    {
                        languages.Add(language.Value);
                    }
                }
            }

            return languages;
        }
    }
}
