using System.Collections.Generic;
using System.Linq;
using DailySubsSubtitleDownloader;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DailySubsSubtitleDownloaderTest
{
    [TestClass]
    public class XmlUtilsTest
    {
        [TestMethod]
        public void TestXmlUtils()
        {
            // Initial dictionary
            var dictionary = new Dictionary<string, string> { { "Castle (2009)", "Castle" } };
            const string xml = "dailySubs-test.xml";
            
            // Save #1
            XmlUtils.SaveDictionaryAsXml(dictionary, xml);

            // Read #1
            var readXmlToDictionary = XmlUtils.ReadXmlToDictionary(xml);

            // Compare #1
            Assert.AreEqual(1, readXmlToDictionary.Count);
            var keyValuePair = readXmlToDictionary.First();
            Assert.AreEqual("Castle (2009)", keyValuePair.Key);
            Assert.AreEqual("Castle", keyValuePair.Value);

            // Add
            readXmlToDictionary.Add("TestKey", "TestValue");
            
            // Save #2
            XmlUtils.SaveDictionaryAsXml(readXmlToDictionary, xml);

            // Read #2
            XmlUtils.SaveDictionaryAsXml(readXmlToDictionary, xml);
            readXmlToDictionary = XmlUtils.ReadXmlToDictionary(xml);

            // Compare #2
            Assert.AreEqual(2, readXmlToDictionary.Count);
            string value;
            readXmlToDictionary.TryGetValue("TestKey", out value);
            Assert.AreEqual("TestValue", value);
        }
    }
}
