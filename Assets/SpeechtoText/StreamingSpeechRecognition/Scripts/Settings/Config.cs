using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.StreamingSpeechRecognition
{
	[CreateAssetMenu(fileName = "GCStreamingSpeechRecognitionConfig", menuName = "Frostweep Games/GCStreamingSpeechRecognition/Config", order = 51)]
	public class Config : ScriptableObject
	{
		[Range(0, 10)]
		public int maxAlternatives;

		public bool googleCredentialLoadFromResources;

		public string googleCredentialFilePath;

		public string googleCredentialJson;

		public bool interimResults;

		public Config()
		{
			maxAlternatives = 1;
			interimResults = true;
			googleCredentialLoadFromResources = true;
			googleCredentialFilePath = string.Empty;
			googleCredentialJson = string.Empty;
		}
	}
}