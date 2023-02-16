namespace FrostweepGames.Plugins.GoogleCloud.StreamingSpeechRecognition
{
    [UnityEditor.InitializeOnLoad]
    public class DefineProcessing : Plugins.DefineProcessing
    {
        internal static readonly string[] _Defines = new string[]
        {
            "FG_GCSSR"
        };

        static DefineProcessing()
        {
            AddOrRemoveDefines(true, true, _Defines);
        }
    }
}