//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System.Threading.Tasks;
using System.Globalization;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif


/// <summary>
/// This is the Speech to Text class - Uses microphone to get audio, sends to AzureCS servers, and receives it in text form. 
/// This class is also responsible for playing the TTS wave files at the appropriate times.
/// </summary>
namespace SpeechTranscripter
{
    public class SpeechRecognition : MonoBehaviour
    {

        public static string transcript; //Transcript of the text. Accesible by other classes without instantiation

        private System.Object threadLocker = new System.Object();

        // Speech recognition key, required
        [Tooltip("Connection string to Cognitive Services Speech.")]
        public string SpeechServiceAPIKey = "[redacted API Key] - Contact billbai0102@gmail.com for key, or use your own.";
        [Tooltip("Region for your Cognitive Services Speech instance (must match the key).")]
        public string SpeechServiceRegion = "eastus";

        // Cognitive Services Speech objects used for Speech Recognition
        private SpeechRecognizer recognizer;
        // The current language of origin is locked to English-US in this sample. Change this
        // to another region & language code to use a different origin language.
        // e.g. fr-fr, es-es, etc.
        string fromLanguage = "en-us";

        //int stores current question number for the interview
        private int questionNum = 0;
        //Array of AudioClips to store each audio clip
        private AudioClip[] clips = new AudioClip[6];
        //Array of booleans to represent each AudioClip that has or hasn't been played yet
        private bool[] canPlay = new bool[6];
        //This is the source that will be playing the audio
        public AudioSource source;

        //boolean that represents whether mic permission is granted or not
        private bool micPermissionGranted = false;

        //Android microphone
#if PLATFORM_ANDROID
        // Required to manifest microphone permission, cf.
        // https://docs.unity3d.com/Manual/android-manifest.html
        private Microphone mic;
#endif

        //Start method, called on instantiation
        private void Start()
        {
#if PLATFORM_ANDROID
            // Request to use the microphone, cf.
            // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#else
        micPermissionGranted = true;
#endif
            //Loads in all the AudioClip questions from the Resources folder. 
            //All the canPlay bools are false, except for [0], as it will
            //be played soon.
            clips[0] = (AudioClip)Resources.Load("GreetingMale");
            canPlay[0] = true;
            clips[1] = (AudioClip)Resources.Load("FirstQMale");
            canPlay[1] = false;
            clips[2] = (AudioClip)Resources.Load("question1");
            canPlay[2] = false;
            clips[3] = (AudioClip)Resources.Load("question2");
            canPlay[3] = false;
            clips[4] = (AudioClip)Resources.Load("question3");
            canPlay[4] = false;
            clips[5] = (AudioClip)Resources.Load("EndMale");
            canPlay[5] = false;
        }

        /// <summary>
        /// Attach to button component used to launch continuous recognition (with or without translation)
        /// </summary>
        public void StartContinuous()
        {
            if (micPermissionGranted)
            {
                StartContinuousRecognition();
            }
        }

        /// <summary>
        /// Creates a class-level Speech Recognizer for a specific language using Azure credentials
        /// and hooks-up lifecycle & recognition events
        /// </summary>
        void CreateSpeechRecognizer()
        {
            if (SpeechServiceAPIKey.Length == 0 || SpeechServiceAPIKey == "YourSubscriptionKey")
            {
                return;
            }
            UnityEngine.Debug.LogFormat("Creating Speech Recognizer.");

            if (recognizer == null)
            {
                SpeechConfig config = SpeechConfig.FromSubscription(SpeechServiceAPIKey, SpeechServiceRegion);
                config.SpeechRecognitionLanguage = fromLanguage;
                recognizer = new SpeechRecognizer(config);

                if (recognizer != null)
                {
                    //Adds event handler methods to all the events.
                    recognizer.Recognizing += RecognizingHandler;
                    recognizer.Recognized += RecognizedHandler;
                    recognizer.SpeechStartDetected += SpeechStartDetectedHandler;
                    recognizer.SpeechEndDetected += SpeechEndDetectedHandler;
                    recognizer.Canceled += CanceledHandler;
                    recognizer.SessionStarted += SessionStartedHandler;
                    recognizer.SessionStopped += SessionStoppedHandler;
                }
            }
            UnityEngine.Debug.LogFormat("CreateSpeechRecognizer exit");
        }

        /// <summary>
        /// Initiate continuous speech recognition from the default microphone.
        /// </summary>
        private async void StartContinuousRecognition()
        {
            UnityEngine.Debug.LogFormat("Starting Continuous Speech Recognition.");
            CreateSpeechRecognizer();

            if (recognizer != null)
            {
                UnityEngine.Debug.LogFormat("Starting Speech Recognizer.");
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                UnityEngine.Debug.LogFormat("Speech Recognizer is now running.");
            }
            UnityEngine.Debug.LogFormat("Start Continuous Speech Recognition exit");
        }

        #region Speech Recognition event handlers
        //Called when session starts
        private void SessionStartedHandler(object sender, SessionEventArgs e)
        {
            UnityEngine.Debug.LogFormat($"\n    Session started event. Event: {e.ToString()}.");
        }

        //Called when session stopped
        private void SessionStoppedHandler(object sender, SessionEventArgs e)
        {
            UnityEngine.Debug.LogFormat($"\n    Session event. Event: {e.ToString()}.");
            UnityEngine.Debug.LogFormat($"Session Stop detected. Stop the recognition.");
        }

        //Called when speech is detected
        private void SpeechStartDetectedHandler(object sender, RecognitionEventArgs e)
        {
            UnityEngine.Debug.LogFormat($"SpeechStartDetected received: offset: {e.Offset}.");
        }

        //Called when speech end is detected
        private void SpeechEndDetectedHandler(object sender, RecognitionEventArgs e)
        {
            UnityEngine.Debug.LogFormat($"SpeechEndDetected received: offset: {e.Offset}.");
            UnityEngine.Debug.LogFormat($"Speech end detected.");
        }

        // "Recognizing" events are fired every time we receive interim results during recognition (i.e. hypotheses)
        private void RecognizingHandler(object sender, SpeechRecognitionEventArgs e)
        {
            if (e.Result.Reason == ResultReason.RecognizingSpeech)
            {
                UnityEngine.Debug.LogFormat($"HYPOTHESIS: Text={e.Result.Text}");
            }
        }

        // "Recognized" events are fired when the utterance end was detected by the server
        private void RecognizedHandler(object sender, SpeechRecognitionEventArgs e)
        {

            //This if statement creates the transcript :)
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                UnityEngine.Debug.LogFormat($"RECOGNIZED: Text={e.Result.Text}");

                //Will eliminate false speech pick-ups from transcript
                if (e.Result.Text.Contains(" "))
                {
                    transcript += (e.Result.Text + "\n");
                }

                //Will accept valid answer as long as text length > 5
                if (e.Result.Text.Length > 5)
                {
                    questionNum++; //Goes to next question
                    canPlay[questionNum] = true; //This (new) question's canPlay value is now true, meaning it will be played.
                }
                UnityEngine.Debug.Log(transcript);
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                UnityEngine.Debug.LogFormat($"NOMATCH: Speech could not be recognized.");
            }
        }

        // "Canceled" events are fired if the server encounters some kind of error.
        // This is often caused by invalid subscription credentials.
        private void CanceledHandler(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            UnityEngine.Debug.LogFormat($"CANCELED: Reason={e.Reason}");
            if (e.Reason == CancellationReason.Error)
            {
                UnityEngine.Debug.LogFormat($"CANCELED: ErrorDetails={e.ErrorDetails}");
                UnityEngine.Debug.LogFormat($"CANCELED: Did you update the subscription info?");
            }
        }
        #endregion

        /// <summary>
        /// Main update loop: Runs every frame
        /// </summary>
        void Update()
        {
            //Gets mic permission on Android
#if PLATFORM_ANDROID
            if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                micPermissionGranted = true;
            }

            if (micPermissionGranted && recognizer == null)
            {
                StartContinuous();
            }
#endif
            // Used to update results on screen during updates
            lock (threadLocker)
            {
                //This wil play the .wav file for Q#<questionNumber>
                if (canPlay[questionNum] == true)
                {
                    source.PlayOneShot(clips[questionNum]);
                    canPlay[questionNum] = false;
                }
                if (source.isPlaying) //Mutes mic when question is being played to avoid false transcript reports
                {
                    Microphone.End(null);
                }
                else //Unmutes mic when question isn't being played to get user response.
                {
                    Microphone.Start(null, true, 10, 44100);
                }

            }
        }

        void OnDisable()
        {
            StopRecognition();
        }

        /// <summary>
        /// Stops the recognition on the speech recognizer or translator as applicable.
        /// Important: Unhook all events & clean-up resources.
        /// </summary>
        public async void StopRecognition()
        {
            if (recognizer != null)
            {
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                recognizer.Recognizing -= RecognizingHandler;
                recognizer.Recognized -= RecognizedHandler;
                recognizer.SpeechStartDetected -= SpeechStartDetectedHandler;
                recognizer.SpeechEndDetected -= SpeechEndDetectedHandler;
                recognizer.Canceled -= CanceledHandler;
                recognizer.SessionStarted -= SessionStartedHandler;
                recognizer.SessionStopped -= SessionStoppedHandler;
                recognizer.Dispose();
                recognizer = null;
                UnityEngine.Debug.LogFormat("Speech Recognizer is now stopped.");
            }
        }
    }
}
