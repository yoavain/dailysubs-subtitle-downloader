using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace DailySubsSubtitleDownloader
{
    public class DailySubsLanguageUtils
    {
        public static readonly Object Lock = new object();
        public static Dictionary<string, CultureInfo> LanguageCodesThreeLetters;
        public static Dictionary<string, CultureInfo> LanguageCodesTwoLetters;

        public static void Init()
        {
            Monitor.Enter(Lock);
            {
                try
                {
                    if (LanguageCodesThreeLetters == null)
                    {
                        LanguageCodesThreeLetters = new Dictionary<string, CultureInfo>();
                        LanguageCodesTwoLetters = new Dictionary<string, CultureInfo>();
                        var cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);
                        foreach (var culture in cultures)
                        {
                            try
                            {
                                LanguageCodesThreeLetters[culture.ThreeLetterISOLanguageName] = culture;
                                LanguageCodesTwoLetters[culture.TwoLetterISOLanguageName] = culture;
                            }
                            catch (CultureNotFoundException) {}
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(Lock);
                }
                
            }
        }


        public static CultureInfo GetLanguageNameFromThreeLetterCode(string code)
        {
            Init();
            CultureInfo cultureInfo;
            LanguageCodesThreeLetters.TryGetValue(code, out cultureInfo);
            return cultureInfo;
        }

        public static CultureInfo GetLanguageNameFromTwoLetterCode(string code)
        {
            Init();
            CultureInfo cultureInfo;
            LanguageCodesTwoLetters.TryGetValue(code, out cultureInfo);
            return cultureInfo;
        }

        public static CultureInfo TryGetLanguageNameFromCode(string code)
        {
            return GetLanguageNameFromThreeLetterCode(code) ?? GetLanguageNameFromTwoLetterCode(code.Substring(0, 2));
        }

        public static List<string> ConvertThreeLetterToTwoLetterLanguageCodes(IEnumerable<string> languageCodes)
        {
            if (languageCodes == null)
            {
                return new List<string>();
            }
            return languageCodes.Select(language => TryGetLanguageNameFromCode(language).TwoLetterISOLanguageName).ToList();
        }
    }
}
