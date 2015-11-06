using DailySubsSubtitleDownloader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DailySubsSubtitleDownloaderTest
{
    [TestClass]
    public class DailySubsLanguageUtilsTest
    {
        [TestMethod]
        public void TestThreeLetterEnglish()
        {
            var english = DailySubsLanguageUtils.GetLanguageNameFromThreeLetterCode("eng");
            Assert.AreEqual("English", english.EnglishName);
            Assert.AreEqual("en", english.TwoLetterISOLanguageName);
        }
        
        [TestMethod]
        public void TestThreeLetterHebrew()
        {
            var hebrew = DailySubsLanguageUtils.GetLanguageNameFromThreeLetterCode("heb");
            Assert.AreEqual("Hebrew", hebrew.EnglishName);
            Assert.AreEqual("he", hebrew.TwoLetterISOLanguageName);
        }

        [TestMethod]
        public void TestTwoLetterEnglish()
        {
            var english = DailySubsLanguageUtils.GetLanguageNameFromTwoLetterCode("en");
            Assert.AreEqual("English", english.EnglishName);
            Assert.AreEqual("eng", english.ThreeLetterISOLanguageName);
        }

        [TestMethod]
        public void TestTwoLetterHebrew()
        {
            var hebrew = DailySubsLanguageUtils.GetLanguageNameFromTwoLetterCode("he");
            Assert.AreEqual("Hebrew", hebrew.EnglishName);
            Assert.AreEqual("heb", hebrew.ThreeLetterISOLanguageName);
        }
    }
}
