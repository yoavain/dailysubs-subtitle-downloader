using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DailySubsSubtitleDownloader
{

    public class Doc
    {
        public List<string> uTitles { get; set; }
        public List<string> langs { get; set; }
        public List<int> langsaantal { get; set; }
        public int aantal { get; set; }
        public int dbid { get; set; }
        public int epi { get; set; }
        public List<string> genres { get; set; }
        public int image { get; set; }
        public int imdb { get; set; }
        public string orgTitle { get; set; }
        public double rating { get; set; }
        public int ratingusers { get; set; }
        public int ser { get; set; }
        public string type { get; set; }
        public int year { get; set; }
        public string id { get; set; }
        public string _version_ { get; set; }
        public double score { get; set; }
    }

    public class Doclist
    {
        public int numFound { get; set; }
        public int start { get; set; }
        public double maxScore { get; set; }
        public List<Doc> docs { get; set; }
    }

    public class Group
    {
        public int groupValue { get; set; }
        public Doclist doclist { get; set; }
    }

    public class TvQueryResponse
    {
        public int matches { get; set; }
        public List<Group> groups { get; set; }
    }

}
