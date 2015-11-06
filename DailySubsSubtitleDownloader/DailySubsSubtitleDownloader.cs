using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MediaPortal.Configuration;
using MediaPortal.Profile;
using MediaPortal.Services;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using SubtitleDownloader.Core;

namespace DailySubsSubtitleDownloader
{
    /// <summary>
    ///     This class is an implementation of SubtitleDownloader API (http://www.assembla.com/spaces/subtitledownloader/wiki)
    ///     For the site DailySubs.net (https://dailysubs.net)
    /// </summary>
    public class DailySubsSubtitleDownloader : ISubtitleDownloader
    {
        private const string BaseUrl = "https://dailysubs.net";
        private const string QueryUrl = BaseUrl + "/ajax/search.php";
        private const string InfoUrl = BaseUrl + "/ajax/info.php";
        private const string DownloadUrl = BaseUrl + "/ajax/down.php";

        public DailySubsSubtitleDownloader(int searchTimeout)
        {
            SearchTimeout = searchTimeout;
        }

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private const string LogFileName = "DailySubsDownloader.log";
        private const string OldLogFileName = "DailySubsDownloader.bak";

        private void InitLogger()
        {
            var loggingConfiguration = LogManager.Configuration ?? new LoggingConfiguration();
            try
            {
                var fileInfo = new FileInfo(Config.GetFile((Config.Dir)1, LogFileName));
                if (fileInfo.Exists)
                {
                    if (File.Exists(Config.GetFile((Config.Dir)1, OldLogFileName)))
                        File.Delete(Config.GetFile((Config.Dir)1, OldLogFileName));
                    fileInfo.CopyTo(Config.GetFile((Config.Dir)1, OldLogFileName));
                    fileInfo.Delete();
                }
            }
            catch { }

            var fileTarget = new FileTarget
            {
                FileName = Config.GetFile((Config.Dir) 1, LogFileName),
                Encoding = "utf-8",
                Layout =
                    "${date:format=dd-MMM-yyyy HH\\:mm\\:ss} ${level:fixedLength=true:padding=5} [${logger:fixedLength=true:padding=20:shortName=true}]: ${message} ${exception:format=tostring}"
            };
            loggingConfiguration.AddTarget("file", fileTarget);
            var settings = new Settings(Config.GetFile((Config.Dir)10, "MediaPortal.xml"));
            LogLevel minLevel;
            switch ((int)(Level)settings.GetValueAsInt("general", "loglevel", 0))
            {
                case 0:
                    minLevel = LogLevel.Error;
                    break;
                case 1:
                    minLevel = LogLevel.Warn;
                    break;
                case 2:
                    minLevel = LogLevel.Info;
                    break;
                default:
                    minLevel = LogLevel.Debug;
                    break;
            }
            var loggingRule = new LoggingRule("*", minLevel, fileTarget);
            loggingConfiguration.LoggingRules.Add(loggingRule);
            LogManager.Configuration = loggingConfiguration;
        }

        public DailySubsSubtitleDownloader()
        {
            InitLogger();
            _logger.Info("DailySubsDownloader is starting");
        }

        // ===============================================================================
        // Public interface functions
        // ===============================================================================

        public string GetName()
        {
            return "dailySubs.net";
        }

        public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
        {
            _logger.Info("Searching for subtitles for " + query.SerieTitle + " season " + query.Season + " episode " + query.Episode);

            // Convert language codes to their english name
            //var languages = query.LanguageCodes.Select(DailySubsLanguageUtils.TryGetLanguageNameFromCode).Select(languageName => languageName.EnglishName).ToList();
            //_logger.Info("Languages: " + string.Join("; ", languages));
            var languages = string.Join(",", query.LanguageCodes);

            // Search subtitles
            var subtitles = SearchSubtitles(query.SerieTitle, query.Season.ToString(CultureInfo.InvariantCulture), query.Episode.ToString(CultureInfo.InvariantCulture), languages);
            var subtitleDatas = subtitles as IList<SubtitleData> ?? subtitles.ToList();
            _logger.Info("Found: " + subtitleDatas.Count() + " subtitle" + (subtitleDatas.Count() > 1 ? "s" : "") + (subtitleDatas.Any() ? "\n" : "") + 
                string.Join("; ", subtitleDatas.Select(i => i.Filename).ToList()));
            
            // Choose only relevant languages
            var filteredSubtitles = subtitleDatas.Where(subtitle => languages.Contains(subtitle.LanguageName));
            var filteredSubtitlesDatas = filteredSubtitles as IList<SubtitleData> ?? filteredSubtitles.ToList();
            _logger.Info("After filtering: " + filteredSubtitlesDatas.Count() + " subtitle" + (filteredSubtitlesDatas.Count() > 1 ? "s" : "") + (filteredSubtitlesDatas.Any() ? "\n" : "") + 
                string.Join("; ", filteredSubtitlesDatas.Select(i => i.Filename).ToList()));

            // Convert list of SubtitleData to list of Subtitle
            return filteredSubtitlesDatas.Select(subtitle => new Subtitle(subtitle.SubtitleUrl, query.SerieTitle, subtitle.Filename, Languages.GetLanguageCode(subtitle.LanguageName))).ToList();
        }

        public List<Subtitle> SearchSubtitles(SearchQuery query)
        {
            const string message = "Movies subtitles on dailySubs.com are not supported";
            _logger.Warn(message);
            throw new Exception(message);
        }

        public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
        {
            const string message = "Movies subtitles on dailySubs.com are not supported";
            _logger.Warn(message);
            throw new Exception(message);
        }

        public List<FileInfo> SaveSubtitle(Subtitle subtitle)
        {
            _logger.Info("Saving subtitle " + subtitle.FileName + "[" + subtitle.LanguageCode + "]");

            var fileName = subtitle.FileName;
            var subtitleId = subtitle.Id;
            var languageCode = subtitle.LanguageCode;
            var languageName = DailySubsLanguageUtils.TryGetLanguageNameFromCode(languageCode).Name;

            var subtitleData = new SubtitleData(fileName, subtitleId, languageName);
            var subtitles = new List<SubtitleData>(1) {subtitleData};

            var savedSubtitle = DownloadSubtitles(subtitles, 0);
            _logger.Info("Subtitle saved");

            return savedSubtitle;
        }

        public int SearchTimeout { get; set; }


        // ===============================================================================
        // Private utility functions
        // ===============================================================================

        private int? GetShowIdFromDailySubsByName(string showName)
        {
            if (showName == null)
            {
                return null;
            }
            if (DailySubsConfiguration.GetInstance().TvShows != null)
            {
                var matches =
                    DailySubsConfiguration.GetInstance().TvShows.Where(pair => string.Equals(pair.Value, showName, StringComparison.OrdinalIgnoreCase))
                        .Select(pair => pair.Key);
                var enumerable = matches as IList<int> ?? matches.ToList();
                if (enumerable.Any())
                {
                    return enumerable.First();
                }
            }
            return null;
        }

        /// <summary>
        ///     Returns the content of the given URL
        /// </summary>
        /// <param name="url">the url</param>
        /// <returns>the content of the page</returns>
        private string GetUrl(string url)
        {
            _logger.Info("Getting URL: " + url);
            try
            {
                var cookieContainer = DailySubsConfiguration.GetInstance().DailySubsCookieContainer;
                var client = cookieContainer != null
                    ? new DailySubsCookieWebClient(cookieContainer)
                    : new DailySubsCookieWebClient();
                {
                    // Add headers
                    client.Headers.Add("user-agent",
                        "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2267.0 Safari/537.36");
                    client.Headers.Add("Referer", BaseUrl);

                    // Download
                    var downloadData = client.DownloadData(url);
                    var downloadString = Encoding.UTF8.GetString(downloadData);

                    // Update cookies
                    DailySubsConfiguration.GetInstance().DailySubsCookieContainer = client.CookieContainer;

                    return downloadString;
                }
            }
            catch (Exception)
            {
                throw new Exception("Could not retrieve URL: " + url);
            }
        }


        /// <summary>
        ///     Downloads a file of the given URL to the given download filename
        /// </summary>
        /// <param name="url">the url</param>
        /// <param name="downloadFile">filename to download</param>
        private string DownloadFile(string url, string downloadFile)
        {
            _logger.Info("Downloading file URL: " + url + "; Filename: " + downloadFile);
            try
            {
                var cookieContainer = DailySubsConfiguration.GetInstance().DailySubsCookieContainer;
                var client = cookieContainer != null
                    ? new DailySubsCookieWebClient(cookieContainer) {Encoding = Encoding.UTF8}
                    : new DailySubsCookieWebClient {Encoding = Encoding.UTF8};
                {
                    // Add headers
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2267.0 Safari/537.36");
                    client.Headers.Add("Referer", BaseUrl);

                    // Download
                    client.DownloadFile(url, downloadFile);

                    var responseHeaders = client.ResponseHeaders;
                    var contentDisposition = responseHeaders.Get("Content-Disposition");
                    if (contentDisposition != null)
                    {
                        var parts = contentDisposition.Split(';');
                        if (parts.Length > 1)
                        {
                            var param = parts[1].Split('=');
                            if (param.Length > 1)
                            {
                                var filename = param[1].Replace("\"", "");
                                return filename;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                var message = "Could not download URL: " + url;
                _logger.Error(message);
                throw new Exception(message);
            }

            return null;
        }

        /// <summary>
        ///     This function is called when the service is selected through the subtitles addon OSD.
        /// </summary>
        /// <param name="tvshow">Name of a tv show. Empty if video isn't a tv show (as are season and episode)</param>
        /// <param name="season">Season number</param>
        /// <param name="episode">Episode number</param>
        /// <param name="languages"></param>
        private IEnumerable<SubtitleData> SearchSubtitles(string tvshow, string season, string episode, string languages)
        {
            var subtitlesList = new List<SubtitleData>();

            // Send 1st request
            var cleanTvShowName = Uri.EscapeDataString(tvshow);
            var getQueryUrl = String.Format("{0}?term={1}&type=tv&langs={2}&s={3}&e={4}", 
                QueryUrl, cleanTvShowName, languages, season, episode);
            var queryResponse = GetUrl(getQueryUrl);
            if (queryResponse == null || queryResponse.IndexOf("{", StringComparison.InvariantCulture) < 0) {
                const string message = "Search timed out, please try again later.";
                _logger.Info(message);
                throw new Exception(message);
            }

            // Parse 1st response
            var queryJson = queryResponse.Substring(queryResponse.IndexOf("{", StringComparison.InvariantCulture));
            var tvQueryResponse = JsonConvert.DeserializeObject<TvQueryResponse>(queryJson);
            var groups = tvQueryResponse.groups;
            if (groups.Count == 0)
            {
                return subtitlesList;
            }
            var firstGroup = groups[0];
            var groupValue = firstGroup.groupValue; // for 2nd request
            var doclist = firstGroup.doclist;
            var docs = doclist.docs;
            if (docs.Count == 0)
            {
                return subtitlesList;
            }
            var firstDoc = docs[0];
            var dbid = firstDoc.dbid; // for 2nd request

            // Send 2nd request
            var getInfoUrl = String.Format("{0}?tvsubs={1}&dbid={2}&s={3}&e={4}&langs={5}", 
                InfoUrl, groupValue, dbid, season, episode, languages);
            var infoJson = GetUrl(getInfoUrl);
            if (infoJson == null || !infoJson.StartsWith("{", StringComparison.InvariantCulture)) {
                const string message = "Search timed out, please try again later.";
                _logger.Info(message);
                throw new Exception(message);
            }

            // Parse 2nd response
            var tvInfoResponse = JsonConvert.DeserializeObject<TvInfoResponse>(infoJson);
            var subs = tvInfoResponse.subs;
            if (subs.Count == 0)
            {
                return subtitlesList;
            }
            foreach (var sub in subs)
            {
                var id = sub.id;
                var filename = sub.filename;
                var from = sub.from;
                var subtitleUrl = String.Format("{0}?id={1}&title={2}&from={3}", DownloadUrl, id, filename, @from);
                var language = sub.lang;

                subtitlesList.Add(new SubtitleData(filename, subtitleUrl, language));
            }

            return subtitlesList;
        }


        /// <summary>
        ///     This function is called when a specific subtitle from the list generated by search_subtitles() is selected in the
        ///     subtitles addon OSD.
        /// </summary>
        /// <param name="subtitlesList">Same list returned in search function</param>
        /// <param name="pos">The selected item's number in subtitles_list</param>
        private List<FileInfo> DownloadSubtitles(IList<SubtitleData> subtitlesList, int pos)
        {
            var response = new List<FileInfo>();
            var url = subtitlesList[pos].SubtitleUrl;

            // Going to write them to standrad zip file (always zips in sratim)
            var archiveFile = Path.GetTempFileName().Replace(".tmp", ".srt");

            // Download the file, use cookie
            var downloadFile = DownloadFile(url, archiveFile);
            var destFileName = Path.GetTempPath() + downloadFile;
            File.Delete(destFileName);
            File.Move(archiveFile, destFileName);
            response.Add(new FileInfo(destFileName));

            return response;
        }


        // ===============================================================================
        // Utility functions
        // ===============================================================================

        // ===============================================================================
        // Private data structures
        // ===============================================================================

        /// <summary>
        ///     Data structure for subtitle object
        /// </summary>
        private class SubtitleData
        {
            private readonly string _filename;
            private readonly string _languageName;
            private readonly string _subtitleUrl;

            public SubtitleData(string filename, string subtitleUrl, string languageName)
            {
                _filename = filename;
                _subtitleUrl = subtitleUrl;
                _languageName = languageName;
            }

            public string Filename
            {
                get { return _filename; }
            }

            public string SubtitleUrl
            {
                get { return _subtitleUrl; }
            }

            public string LanguageName
            {
                get { return _languageName; }
            }
        }
    }
}