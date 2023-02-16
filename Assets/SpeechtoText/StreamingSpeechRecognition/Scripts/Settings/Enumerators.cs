using System;

namespace FrostweepGames.Plugins.GoogleCloud.StreamingSpeechRecognition
{
    public static class Enumerators
    {
		public enum LanguageCode
        {
        
            en_GB,
            en_US,
            en_AU,
            en_CA,
            en_GH,
            en_HK,
            en_IN,
            en_IE,
            en_KE,
            en_NZ,
            en_NG,
            en_PK,
            en_PH,
            en_SG,
            en_ZA,
            en_TZ,
            cmn_Hans_CN
        }

        public static string Parse(this Enum value, char symbolFrom = '_', char symbolTo = '-')
		{
			return value.ToString().Replace(symbolFrom, symbolTo);
		}
    }
}