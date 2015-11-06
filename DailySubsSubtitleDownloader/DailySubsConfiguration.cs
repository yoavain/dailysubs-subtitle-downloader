using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace DailySubsSubtitleDownloader
{
    public sealed class DailySubsConfiguration
    {
        public const string ConfigurationDirectory = "SubtitleDownloaders";
        public const string ConfigurationFilename = "DailySubsSubtitleDownloader.xml";

        #region Singleton

        private DailySubsConfiguration()
        {
            try
            {
                // Check directory (create if needed)
                if (!Directory.Exists(ConfigurationDirectory))
                {
                    Directory.CreateDirectory(ConfigurationDirectory);
                }

                // File path
                var path = ConfigurationDirectory + Path.DirectorySeparatorChar + ConfigurationFilename;

                // Check file
                if (!File.Exists(path))
                {
                    TvShowsOverrides = new Dictionary<string, string> {{"Castle (2009)", "Castle"}};
                    XmlUtils.SaveDictionaryAsXml(TvShowsOverrides, path);
                }
                else
                {
                    TvShowsOverrides = XmlUtils.ReadXmlToDictionary(path);    
                }
            }
            catch (Exception)
            {
                TvShowsOverrides = new Dictionary<string, string>();
            }
        }

        private static class DailySubsConfigurationHolder
        {
            public static readonly DailySubsConfiguration Instance = new DailySubsConfiguration();
        }

        public static DailySubsConfiguration GetInstance()
        {
            return DailySubsConfigurationHolder.Instance;
        }

        #endregion Singleton


        #region private

        

        #endregion private

        #region public methods
        
        // Cookie container
        public CookieContainer DailySubsCookieContainer { get; set; }
        
        // TV shows map
        public Dictionary<int, string> TvShows { get; set; }

        // Overriding map
        public Dictionary<string, string> TvShowsOverrides { get; set; }

        #endregion public methods

    }
}
