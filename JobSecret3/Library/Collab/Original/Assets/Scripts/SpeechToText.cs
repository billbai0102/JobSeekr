using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class SpeechToText : MonoBehaviour
{
    public Text outputText;

    SpeechRecognizer recognizer;
    SpeechConfig config;

    private System.Object threadLocker = new System.Object();
    private string message;

    private bool micPermissionGranted = false;

#if PLATFORM_ANDROID
    // Required to manifest microphone permission, cf.
    // https://docs.unity3d.com/Manual/android-manifest.html
    private Microphone mic;
#endif

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

        config = SpeechConfig.FromSubscription("e7da0995b44a4dc19f7182da2ae79f19", "eastus");
        recognizer = new SpeechRecognizer(config);
        recognizer.Recognizing += RecognizingHandler;

        StartRecognizing();
    }

    //This is the update method. It will keep
    void Update()
    {
#if PLATFORM_ANDROID
        if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            micPermissionGranted = true;
        }
#endif

        lock (threadLocker)
        {
            if (outputText != null)
            {
                outputText.text = message;
            }
        }
    }
}