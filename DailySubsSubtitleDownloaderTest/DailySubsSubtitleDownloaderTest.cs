using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleDownloader.Core;

namespace DailySubsSubtitleDownloaderTest
{
    [TestClass]
    public class DailySubsSubtitleDownloaderTest
    {
        [TestMethod]
        public void TestDailySubsSearch()
        {
            var dailySubsSubtitleDownloader = new DailySubsSubtitleDownloader.DailySubsSubtitleDownloader();
            var episodeSearchQuery = new EpisodeSearchQuery("the big bang theory", 1, 1, null)
            {
                LanguageCodes = new[] {"eng"}
            };
            var searchSubtitles = dailySubsSubtitleDownloader.SearchSubtitles(episodeSearchQuery);
            Assert.IsNotNull(searchSubtitles);
        }
        
        [TestMethod]
        public void TestDailySubsDownload()
        {
            var dailySubsSubtitleDownloader = new DailySubsSubtitleDownloader.DailySubsSubtitleDownloader();
            var episodeSearchQuery = new EpisodeSearchQuery("house m.d.", 1, 1, null)
            {
                LanguageCodes = new [] {"eng"}
            };
            var searchSubtitles = dailySubsSubtitleDownloader.SearchSubtitles(episodeSearchQuery);

            // make sure there are resuts
            Assert.IsNotNull(searchSubtitles);

            // check result
            var saveSubtitle = dailySubsSubtitleDownloader.SaveSubtitle(searchSubtitles[0]);
            Assert.AreNotEqual(null, saveSubtitle);
            Assert.AreNotEqual(0, saveSubtitle.Count);
        }

        [TestMethod]
        public void TestDailySubsDownloadLanguage()
        {
            var dailySubsSubtitleDownloader = new DailySubsSubtitleDownloader.DailySubsSubtitleDownloader();
            var episodeSearchQuery = new EpisodeSearchQuery("castle 2009", 7, 13, null)
            {
                LanguageCodes = new[] { "eng" }
            };
            var searchSubtitles = dailySubsSubtitleDownloader.SearchSubtitles(episodeSearchQuery);

            // make sure there are resuts
            Assert.IsNotNull(searchSubtitles);

            // check result
            var saveSubtitle = dailySubsSubtitleDownloader.SaveSubtitle(searchSubtitles[0]);
            Assert.AreNotEqual(null, saveSubtitle);
            Assert.AreNotEqual(0, saveSubtitle.Count);
        }
    }
}
