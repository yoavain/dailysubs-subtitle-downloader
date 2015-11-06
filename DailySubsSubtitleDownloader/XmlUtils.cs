using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DailySubsSubtitleDownloader
{
    public class XmlUtils
    {
        public static Dictionary<string, string> ReadXmlToDictionary(string filename)
        {
            var xElement = XElement.Load(filename);
            return xElement.Descendants("tvShow").ToDictionary(x => (string)x.Attribute("key"), x => (string)x.Attribute("value"));
        }

        public static void SaveDictionaryAsXml(Dictionary<string, string> dictionary, string filename)
        {
            var xElem = new XElement("tvShows", dictionary.Select(x => new XElement("tvShow", new XAttribute("key", x.Key), new XAttribute("value", x.Value))));
            xElem.Save(filename);
        }
    }
}
