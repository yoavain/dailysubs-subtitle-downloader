using System.Collections.Generic;
using System.IO;

namespace DailySubsSubtitleDownloader
{
    class SubtitleUtils
    {
        /// <summary>
        /// Checks if file has subtitle extension
        /// </summary>
        /// <param name="fileInfo">file info</param>
        /// <returns>true if file has subtitle extension</returns>
        public static bool HasSubtitleExtension(FileSystemInfo fileInfo)
        {
            return SubtitleExtensions.Contains(fileInfo.Extension.ToLowerInvariant());
        }

        private static readonly List<string> SubtitleExtensions = new List<string>
        {
            ".aqt",
            ".asc",
            ".ass",
            ".dat",
            ".dks",
            "idx",
            ".js",
            ".jss",
            ".lrc",
            ".mpl",
            ".ovr",
            ".pan",
            ".pjs",
            ".psb",
            ".rt",
            ".rtf",
            ".s2k",
            ".sbt",
            ".scr",
            ".smi",
            ".son",
            ".srt",
            ".ssa",
            ".sst",
            ".ssts",
            ".stl",
            ".sub",
            ".vkt",
            ".vsf",
            ".zeg"
        };
    }
}
