using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DailySubsSubtitleDownloader
{

    public class Sub
    {
        public string id { get; set; }
        public string oldid { get; set; }
        public string imdb { get; set; }
        public string lang { get; set; }
        public string FPS { get; set; }
        public string HD { get; set; }
        public string V2 { get; set; }
        public string trusted { get; set; }
        public string autotrans { get; set; }
        public string badhearing { get; set; }
        public string lastmodt { get; set; }
        public string from { get; set; }
        public string filename { get; set; }
        public string file { get; set; }
        public string ext { get; set; }
        public string size { get; set; }
        public string lines { get; set; }
        public object voteup { get; set; }
        public object votedown { get; set; }
        public object comments { get; set; }
    }

    public class TvInfoResponse
    {
        public string imdb { get; set; }
        public List<Sub> subs { get; set; }
        public int s { get; set; }
        public int e { get; set; }
    }

}
