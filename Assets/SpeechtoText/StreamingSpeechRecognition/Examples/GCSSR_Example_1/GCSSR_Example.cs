using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrostweepGames.Plugins.GoogleCloud.StreamingSpeechRecognition.Examples
{
	public class GCSSR_Example : MonoBehaviour
	{
		private GCStreamingSpeechRecognition _speechRecognition;

		public Button _startRecordButton,
					   _stopRecordButton,
					   _refreshMicrophonesButton,
					_confirmButton;

		public Image _speechRecognitionState;
		public Image _speechRecognitionState_whole;

		public Text _resultText;
		public Text _debug;

		public Dropdown _languageDropdown,
						 _microphoneDevicesDropdown;

		public InputField _contextPhrasesInputField;

		public ScrollRect scrollRect;

		public Image voiceLevelImage;

		public float voiceDetectionThreshold = 0.02f;

		public Text noteBTNText;
		public bool isStart;

		public GameObject[] saveCameras;
		public GameObject[] iconRECs;

		private void Start()
		{
			_speechRecognition = GCStreamingSpeechRecognition.Instance;
			_speechRecognition.StreamingRecognitionStartedEvent += StreamingRecognitionStartedEventHandler;
			_speechRecognition.StreamingRecognitionFailedEvent += StreamingRecognitionFailedEventHandler;
			_speechRecognition.StreamingRecognitionEndedEvent += StreamingRecognitionEndedEventHandler;
			_speechRecognition.InterimResultDetectedEvent += InterimResultDetectedEventHandler;
			_speechRecognition.FinalResultDetectedEvent += FinalResultDetectedEventHandler;

			_startRecordButton.onClick.AddListener(NoteRecordButtonOnClickHandler);
			//_startRecordButton.onClick.AddListener(StartRecordButtonOnClickHandler);
			_stopRecordButton.onClick.AddListener(StopRecordButtonOnClickHandler);
			_refreshMicrophonesButton.onClick.AddListener(RefreshMicsButtonOnClickHandler);

			_confirmButton.onClick.AddListener(ConfirmButtonOnClickHandler);
			_confirmButton.gameObject.SetActive(false);

			_microphoneDevicesDropdown.onValueChanged.AddListener(MicrophoneDevicesDropdownOnValueChangedEventHandler);

			_startRecordButton.interactable = true;
			//_stopRecordButton.interactable = false;
			isStart = false;
			_speechRecognitionState.color = Color.yellow;

			_languageDropdown.ClearOptions();

			for (int i = 0; i < Enum.GetNames(typeof(Enumerators.LanguageCode)).Length; i++)
			{
				_languageDropdown.options.Add(new Dropdown.OptionData(((Enumerators.LanguageCode)i).Parse()));
			}

			_languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.en_US.Parse()));

			RefreshMicsButtonOnClickHandler();

			saveCameras = GameObject.FindGameObjectsWithTag("SaveCam");
		}

		private void OnDestroy()
		{
			_speechRecognition.StreamingRecognitionStartedEvent -= StreamingRecognitionStartedEventHandler;
			_speechRecognition.StreamingRecognitionFailedEvent -= StreamingRecognitionFailedEventHandler;
			_speechRecognition.StreamingRecognitionEndedEvent -= StreamingRecognitionEndedEventHandler;
			_speechRecognition.InterimResultDetectedEvent -= InterimResultDetectedEventHandler;
			_speechRecognition.FinalResultDetectedEvent -= FinalResultDetectedEventHandler;
		}

        private void Update()
        {
			if (_speechRecognition.isRecording)
			{
				if (_speechRecognition.GetMaxFrame() > 0)
				{
					float max = voiceDetectionThreshold;
					float current = _speechRecognition.GetLastFrame() / max;

					if (current >= 1f)
					{
						voiceLevelImage.fillAmount = Mathf.Lerp(voiceLevelImage.fillAmount, Mathf.Clamp(current / 2f, 0, 1f), 30 * Time.deltaTime);
					}
					else
					{
						voiceLevelImage.fillAmount = Mathf.Lerp(voiceLevelImage.fillAmount, Mathf.Clamp(current / 2f, 0, 0.5f), 30 * Time.deltaTime);
					}

					voiceLevelImage.color = current >= 1f ? Color.green : Color.red;
				}
			}
			else
			{
				voiceLevelImage.fillAmount = 0f;
			}

			if (saveCameras.Length < 3)
			{
				saveCameras = GameObject.FindGameObjectsWithTag("SaveCam");
			}
		}

        private void RefreshMicsButtonOnClickHandler()
		{
			_speechRecognition.RequestMicrophonePermission();

			_microphoneDevicesDropdown.ClearOptions();

			for (int i = 0; i < _speechRecognition.GetMicrophoneDevices().Length; i++)
			{
				_microphoneDevicesDropdown.options.Add(new Dropdown.OptionData(_speechRecognition.GetMicrophoneDevices()[i]));
			}

			//smart fix of dropdowns
			_microphoneDevicesDropdown.value = 1;
			_microphoneDevicesDropdown.value = 0;
		}

		private void MicrophoneDevicesDropdownOnValueChangedEventHandler(int value)
		{
			if (!_speechRecognition.HasConnectedMicrophoneDevices())
				return;
			_speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[value]);
		}

		private void StartRecordButtonOnClickHandler()
		{
			_debug.text = "StartRecordButtonOnClickHandler";
			_resultText.text = string.Empty;

			List<List<string>> context = new List<List<string>>();

			if(_contextPhrasesInputField.text.Length > 0)
			{
				string[] split = _contextPhrasesInputField.text.Split(',');

				List<string> context1 = new List<string>();
				foreach(var item in split)
				{
					context1.Add(item.TrimStart(' ').TrimEnd(' '));
				}

				context.Add(context1);
			}

			_speechRecognition.StartStreamingRecognition((Enumerators.LanguageCode)_languageDropdown.value, context);
		}


		/// <summary>
		/// created by qiao
		/// </summary>
        ///
		
		public void NoteRecordButtonOnClickHandler()
		{
			if (isStart)
			{
				//Debug.Log("isStart-true->false");
				isStart = false;
				_confirmButton.gameObject.SetActive(false);
				_speechRecognitionState_whole.gameObject.SetActive(false);

				_startRecordButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

				foreach (GameObject saveCam in saveCameras)
				{
					saveCam.SendMessage("HideREC");
				}

				foreach (GameObject iconREC in iconRECs)
				{
					//iconREC.SendMessage("StopRecording");
					iconREC.GetComponent<RecordStatusIndicator>().StopRecording();
				}

				StopRecordButtonOnClickHandler();
				_resultText.text = string.Empty;
			}
			else
			{
				
				//Debug.Log("isStart-false->true");
				isStart = true;
				_confirmButton.gameObject.SetActive(true);
				_speechRecognitionState_whole.gameObject.SetActive(true);
				_startRecordButton.GetComponent<Image>().color = Color.green;

				// Now take a screenshot:

				foreach (GameObject saveCam in saveCameras)
				{
					_debug.text = "NoteRecordButtonOnClickHandler - start-save cam";
					saveCam.SendMessage("TakeNoteSC");
					saveCam.SendMessage("ShowREC");
				}

				foreach (GameObject iconREC in iconRECs)
				{
					//iconREC.SendMessage("StartRecording");
					_debug.text = "NoteRecordButtonOnClickHandler - iconREC";
					iconREC.GetComponent<RecordStatusIndicator>().StartRecording();
				}


				StartRecordButtonOnClickHandler();
				

			}

		}

		public void ConfirmButtonOnClickHandler()
		{
			isStart = false;
			_confirmButton.gameObject.SetActive(false);


			_startRecordButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

			// at this point, _resultText contains all the recognized note
			// add these notes to the screenshot:
			
			foreach (GameObject saveCam in saveCameras)
			{
				saveCam.SendMessage("HideREC");
				if (_resultText.text != null)
                {
					saveCam.SendMessage("SetNoteContent", _resultText.text);
				}
			}

			
			foreach (GameObject iconREC in iconRECs)
			{
				//iconREC.SendMessage("StopRecording");
				iconREC.GetComponent<RecordStatusIndicator>().StopRecording();
			}

			StopRecordButtonOnClickHandler();
			_resultText.text = string.Empty;
		}



		private async void StopRecordButtonOnClickHandler()
		{
			isStart = false;
			await _speechRecognition.StopStreamingRecognition();
		}

		private void StreamingRecognitionStartedEventHandler()
		{
			_speechRecognitionState.color = Color.red;

			//_stopRecordButton.interactable = true;
			//_startRecordButton.interactable = false;
		}

		private void StreamingRecognitionFailedEventHandler(string error)
		{
			_speechRecognitionState.color = Color.yellow;
			_resultText.text = "<color=red>Start record Failed. Please check microphone device and try again.</color>";

			//_stopRecordButton.interactable = false;
			//_startRecordButton.interactable = true;
			isStart = false;
		}

		private void StreamingRecognitionEndedEventHandler()
        {
			_speechRecognitionState.color = Color.green;

			_stopRecordButton.interactable = false;
			_startRecordButton.interactable = true;
		}

		private void InterimResultDetectedEventHandler(string alternative)
        {
			if(_resultText.text.Length > 1000)
				_resultText.text = string.Empty;

			_resultText.text = $"<b>Note:</b> {alternative}\n";

			scrollRect.verticalNormalizedPosition = 0f;
		}

		private void FinalResultDetectedEventHandler(string alternative)
		{
			if (_resultText.text.Length > 1000)
				_resultText.text = string.Empty;

			_resultText.text = $"<b>Note:</b> {alternative}\n";

			scrollRect.verticalNormalizedPosition = 0f;
		}
    }
}