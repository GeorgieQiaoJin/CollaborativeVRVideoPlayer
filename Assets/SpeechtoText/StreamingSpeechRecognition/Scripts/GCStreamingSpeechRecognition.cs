using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Google.Protobuf;
using Google.Cloud.Speech.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using FrostweepGames.Plugins.Native;
using FrostweepGames.Plugins.Editor;
using System.Collections;
using UnityEngine.Assertions;
using Grpc.Core;

namespace FrostweepGames.Plugins.GoogleCloud.StreamingSpeechRecognition
{
    public class GCStreamingSpeechRecognition : MonoBehaviour
    {
		private const bool LogExceptions = false;

		private const int SampleRate = 16000; // LINEAR 16 frequency

		private const int StreamingRecognitionTimeLimit = 110; // around 2 minutes. ~ 1.83333334

		private const int AudioChunkSize = SampleRate; // amount of samples

		public static GCStreamingSpeechRecognition Instance { get; private set; }

		public event Action StreamingRecognitionStartedEvent;
		public event Action<string> StreamingRecognitionFailedEvent;
		public event Action StreamingRecognitionEndedEvent;
		public event Action<string> InterimResultDetectedEvent;
		public event Action<string> FinalResultDetectedEvent;

		private SpeechClient _speechClient;

		private SpeechClient.StreamingRecognizeStream _streamingRecognizeStream;

		private AudioClip _workingClip;

		private Coroutine _checkOnMicAndRunStreamRoutine;

		private CancellationTokenSource _cancellationToken;

		private float _recordingTime;

		private int _currentSamplePosition;

		private int _previousSamplePosition;

		private float[] _currentAudioSamples;

		private List<byte> _currentRecordedSamples;

		private Enumerators.LanguageCode _currentLanguageCode;

		private List<List<string>> _currentRecogntionContext;

		private bool _initialized;

		private bool _recognition;

		private float _maxVoiceFrame;

		public Config config;

		[ReadOnly]
		public bool isRecording;

		[ReadOnly]
		public string microphoneDevice;

		private void Awake()
		{
			if(Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;

			Assert.IsNotNull(config, "Config is requried to be added.");

			Initialize();	
		}

		private async void OnDestroy()
		{
			if (Instance != this || !_initialized)
				return;

			await StopStreamingRecognition(); // wait for all data to be sent successfully

			Instance = null;
		}

		private async void Update()
		{
			if (Instance != this || !_initialized)
				return;

			if (!isRecording)
				return;

			_recordingTime += Time.unscaledDeltaTime;

			if(_recordingTime >= StreamingRecognitionTimeLimit)
			{
				await RestartStreamingRecognitionAfterLimit(); // restart speech recogntion when time limit is reached
				_recordingTime = 0;
			}

			HandleRecordingData(); // handle data from microphone each frame
		}

		private void FixedUpdate()
		{
			if (Instance != this || !_initialized)
				return;

			WriteDataToStream(); // write data to stream each physics frame
		}

		public float GetLastFrame()
		{
			int minValue = SampleRate / 8;

			if (_currentAudioSamples == null)
				return 0;

			int position = Mathf.Clamp(_currentSamplePosition - (minValue + 1), 0, _currentAudioSamples.Length - 1);

			float sum = 0f;
			for (int i = position; i < _currentAudioSamples.Length; i++)
			{
				sum += Mathf.Abs(_currentAudioSamples[i]);
			}

			sum /= minValue;

			return sum;
		}

		public float GetMaxFrame()
		{
			return _maxVoiceFrame;
		}

		/// <summary>
		/// Requests permission for Microphone device if it not granted
		/// </summary>
		public void RequestMicrophonePermission()
		{
			if (!CustomMicrophone.HasMicrophonePermission())
			{
				CustomMicrophone.RequestMicrophonePermission();
			}
		}

		/// <summary>
		/// Configures current Microphone device for recording
		/// </summary>
		/// <param name="deviceName"></param>
		public void SetMicrophoneDevice(string deviceName)
		{
			if (isRecording)
				return;

			microphoneDevice = deviceName;
		}

		/// <summary>
		/// Returns array of connected microphone devices
		/// </summary>
		/// <returns></returns>
		public string[] GetMicrophoneDevices()
		{
			return CustomMicrophone.devices;
		}

		/// <summary>
		/// Returns true if at least 1 microphone device is connected
		/// </summary>
		/// <returns></returns>
		public bool HasConnectedMicrophoneDevices()
		{
			return CustomMicrophone.HasConnectedMicrophoneDevices();
		}

		/// <summary>
		/// Starts streamign recogntion.
		/// </summary>
		/// <param name="languageCode">langauge detection</param>
		/// <param name="context">audio context</param>
		public void StartStreamingRecognition(Enumerators.LanguageCode languageCode, List<List<string>> context)
		{
			if(!_initialized)
			{
				StreamingRecognitionFailedEvent?.Invoke("Failed to start recogntion due to: 'Not initialized'");
				return;
			}

			_currentLanguageCode = languageCode;
			_currentRecogntionContext = context;
			_checkOnMicAndRunStreamRoutine = StartCoroutine(CheckOnMicrophoneAndRunStream());
		}

		/// <summary>
		/// Stops streaming recognition if started
		/// </summary>
		/// <returns></returns>
		public async Task StopStreamingRecognition()
		{
			if (!isRecording || !_recognition)
				return;

			_recognition = false;

			StopRecording();

			if (_streamingRecognizeStream != null)
			{
				await _streamingRecognizeStream.WriteCompleteAsync();
			}

			_streamingRecognizeStream = null;
			_currentRecordedSamples = null;

			if(_checkOnMicAndRunStreamRoutine != null)
			{
				StopCoroutine(_checkOnMicAndRunStreamRoutine);
				_checkOnMicAndRunStreamRoutine = null;
			}

			if (_cancellationToken != null)
			{
				_cancellationToken.Cancel();
				_cancellationToken.Dispose();
				_cancellationToken = null;
			}

			StreamingRecognitionEndedEvent?.Invoke();
		}

		/// <summary>
		/// Restats automatically streamign recognition when time limit is reached
		/// </summary>
		/// <returns></returns>
		private async Task RestartStreamingRecognitionAfterLimit()
		{
			await StopStreamingRecognition();

			_checkOnMicAndRunStreamRoutine = StartCoroutine(CheckOnMicrophoneAndRunStream());
		}

		/// <summary>
		/// Requests microphone permission in a coroutine and then runs recognition stream
		/// </summary>
		/// <returns></returns>
		private IEnumerator CheckOnMicrophoneAndRunStream()
		{
			while (!HasConnectedMicrophoneDevices())
			{
				RequestMicrophonePermission();
				yield return null;
			}

			RunStreamingRecognition();

			_checkOnMicAndRunStreamRoutine = null;
		}

		/// <summary>
		/// Starts speech recognition stream
		/// </summary>
		private async void RunStreamingRecognition()
		{
			if (isRecording)
			{
				StreamingRecognitionFailedEvent?.Invoke("Already recording");
				return;
			}

			if (!StartRecording())// todo handle
			{
				StreamingRecognitionFailedEvent?.Invoke("Cannot start recording");
				return;
			}

			_streamingRecognizeStream = _speechClient.StreamingRecognize();

			var recognitionConfig = new RecognitionConfig()
			{
				Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
				SampleRateHertz = SampleRate,
				LanguageCode = _currentLanguageCode.Parse(),
				MaxAlternatives = config.maxAlternatives,
			};

			if (_currentRecogntionContext != null)
			{
				SpeechContext speechContext;
				foreach (var phrases in _currentRecogntionContext)
				{
					if (phrases != null)
					{
						speechContext = new SpeechContext();
						foreach (var phrase in phrases)
						{
							speechContext.Phrases.Add(phrase);
						}

						recognitionConfig.SpeechContexts.Add(speechContext);
					}
				}
			}

			StreamingRecognitionConfig streamingConfig = new StreamingRecognitionConfig()
			{
				Config = recognitionConfig,
				InterimResults = config.interimResults,
			};

			try
			{
				await _streamingRecognizeStream.WriteAsync(new StreamingRecognizeRequest()
				{
					StreamingConfig = streamingConfig
				});
			}
			catch(RpcException ex)
			{
				StopRecording();

				_streamingRecognizeStream = null;

				StreamingRecognitionFailedEvent?.Invoke($"Cannot start recognition due to: {ex.Message}");
				return;
			}

			_recognition = true;

			StreamingRecognitionStartedEvent.Invoke();

			_cancellationToken = new CancellationTokenSource();

			HandleStreamingRecognitionResponsesTask();
		}

		/// <summary>
		/// Starts microphone device recording
		/// </summary>
		/// <returns></returns>
		public bool StartRecording()
		{
			if (string.IsNullOrEmpty(microphoneDevice))
				return false;

			_workingClip = CustomMicrophone.Start(microphoneDevice, true, 3, SampleRate);

			_currentAudioSamples = new float[_workingClip.samples];

			_currentRecordedSamples = new List<byte>();

			isRecording = true;
			_maxVoiceFrame = 0;

			return true;
		}

		/// <summary>
		/// stops microphone device recording and lceaning recording data
		/// </summary>
		public void StopRecording()
		{
			if (!isRecording)
				return;

			if (string.IsNullOrEmpty(microphoneDevice))
				return;

			CustomMicrophone.End(microphoneDevice);

			MonoBehaviour.Destroy(_workingClip);

			_currentRecordedSamples.Clear();

			isRecording = false;
		}

		/// <summary>
		/// Handles all speech recognition response asynchronously
		/// </summary>
		private async void HandleStreamingRecognitionResponsesTask()
		{
			try
			{
				while (await _streamingRecognizeStream.GetResponseStream().MoveNextAsync(_cancellationToken.Token))
				{
					var current = _streamingRecognizeStream.GetResponseStream().Current;

					if (current == null)
						return;

					var results = _streamingRecognizeStream.GetResponseStream().Current.Results;

					if (results.Count <= 0)
						continue;

					StreamingRecognitionResult result = results[0];
					if (result.Alternatives.Count <= 0)
						continue;

					if (result.IsFinal)
					{
						FinalResultDetectedEvent.Invoke(result.Alternatives[0].Transcript.Trim());
					}
					else
					{
						if (config.interimResults)
						{
							for (int i = 0; i < config.maxAlternatives; i++)
							{
								if (i >= result.Alternatives.Count)
									break;

								InterimResultDetectedEvent.Invoke(result.Alternatives[i].Transcript.Trim());
							}
						}
					}
				}
			}
			catch (Exception ex) 
			{
				if (LogExceptions)
				{
					Debug.LogException(ex);
				}
			}
		}

		/// <summary>
		/// Writes recordign data from Microphone device to buffer for sending to service
		/// </summary>
		private void HandleRecordingData()
		{
			if (!isRecording)
				return;

			_currentSamplePosition = CustomMicrophone.GetPosition(microphoneDevice);

			if (CustomMicrophone.GetRawData(ref _currentAudioSamples, _workingClip))
			{
				if (_previousSamplePosition > _currentSamplePosition)
				{
					for (int i = _previousSamplePosition; i < _currentAudioSamples.Length; i++)
					{
						if (_currentAudioSamples[i] > _maxVoiceFrame)
							_maxVoiceFrame = _currentAudioSamples[i];

						_currentRecordedSamples.AddRange(FloatToBytes(_currentAudioSamples[i]));
					}

					_previousSamplePosition = 0;
				}

				for (int i = _previousSamplePosition; i < _currentSamplePosition; i++)
				{
					if (_currentAudioSamples[i] > _maxVoiceFrame)
						_maxVoiceFrame = _currentAudioSamples[i];

					_currentRecordedSamples.AddRange(FloatToBytes(_currentAudioSamples[i]));
				}

				_previousSamplePosition = _currentSamplePosition;
			}
		}

		/// <summary>
		/// Sends samples asynchronously in stream
		/// </summary>
		private async void WriteDataToStream()
		{
			if (_streamingRecognizeStream == null)
				return;

			ByteString chunk;
			List<byte> samplesChunk = null;

			if (isRecording || (_currentRecordedSamples != null && _currentRecordedSamples.Count > 0))
			{
				if (_currentRecordedSamples.Count >= AudioChunkSize * 2)
				{
					samplesChunk = _currentRecordedSamples.GetRange(0, AudioChunkSize * 2);
					_currentRecordedSamples.RemoveRange(0, AudioChunkSize * 2);
				}
				else if(!isRecording)
				{
					samplesChunk = _currentRecordedSamples.GetRange(0, _currentRecordedSamples.Count);
					_currentRecordedSamples.Clear();
				}

				if (samplesChunk != null && samplesChunk.Count > 0)
				{
					chunk = ByteString.CopyFrom(samplesChunk.ToArray(), 0, samplesChunk.Count);

					try
					{
						await _streamingRecognizeStream.WriteAsync(new StreamingRecognizeRequest() { AudioContent = chunk });
					}
					catch(RpcException ex)
					{
						StreamingRecognitionFailedEvent?.Invoke($"Cannot proceed recognition due to: {ex.Message}");

						_streamingRecognizeStream = null;

						await StopStreamingRecognition();
					}
				}
			}
		}

		/// <summary>
		/// Initializes speech client for future requests to service
		/// </summary>
		private void Initialize()
		{
			string credentialJson;

            if (config.googleCredentialLoadFromResources)
            {
                if (string.IsNullOrEmpty(config.googleCredentialFilePath) || string.IsNullOrWhiteSpace(config.googleCredentialFilePath))
                {
					Debug.LogException(new Exception("The googleCredentialFilePath is empty. Please fill path to file."));
					return;
                }

				TextAsset textAsset = Resources.Load<TextAsset>(config.googleCredentialFilePath);

				if(textAsset == null)
                {
					Debug.LogException(new Exception($"Couldn't load file: {config.googleCredentialFilePath} ."));
					return;
				}

				credentialJson = textAsset.text;
			}
            else
            {
				credentialJson = config.googleCredentialJson;
			}

			if (string.IsNullOrEmpty(credentialJson) || string.IsNullOrWhiteSpace(credentialJson))
			{
				Debug.LogException(new Exception("The Google service account credential is empty."));
				return;
			}

			try
			{
#pragma warning disable CS1701
				_speechClient = new SpeechClientBuilder
				{
					ChannelCredentials = GoogleCredential.FromJson(credentialJson).ToChannelCredentials()
				}.Build();
#pragma warning restore CS1701
				_initialized = true;
			}
			catch(Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		private byte[] FloatToBytes(float sample)
		{
			return System.BitConverter.GetBytes((short)(sample * 32767));
		}
	}
}