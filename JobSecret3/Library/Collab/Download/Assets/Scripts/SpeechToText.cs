using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;

public class SpeechToText : MonoBehaviour
{
    public Text outputText;

    SpeechRecognizer recognizer;
    SpeechConfig config;

    private object threadLocker = new object();
    private string message;

    private bool micPermissionGranted = false;
    private void RecognizingHandler(object sender, SpeechRecognitionEventArgs e)
    {
        lock (threadLocker)
        {
            message = e.Result.Text;
        }
    }
    public async void StartRecognizing()
    {
        await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
    }

    void Start()
    {
        config = SpeechConfig.FromSubscription("e7da0995b44a4dc19f7182da2ae79f19", "eastus");
        recognizer = new SpeechRecognizer(config);
        recognizer.Recognizing += RecognizingHandler;

        StartRecognizing();
    }

    //This is the update method. It will keep
    void Update()
    {
        lock (threadLocker)
        {
            if (outputText != null)
            {
                outputText.text = message;
            }
        }
    }
}