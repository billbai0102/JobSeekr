using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// This class creates 3 .wav files from the job.txt file that stores webscraped + Azure CS keyword questions
/// </summary>
public class TTSLoad : MonoBehaviour
{
    //Name of the 3 files
    public string fileName = "/question";

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Debug.Log(Application.dataPath);

        //Creates an array of string that stores each of the lines in jobs.txt
        string[] TTSArray = File.ReadAllLines("jobs.txt");
        //For loop that stores a TTS line in a specific location
        for (int x = 0; x < 3; ++x)
        {
            //Creates the real file name, that is distinguishable
            string fileNameFull = fileName + (x+1).ToString();
            UnityEngine.Debug.Log(TTSArray[x] + " - " + fileNameFull);
            //Creates a TTS of the line at x, with the filename mentioned earlier with voice GuyNeural
            //And then stores it in the Resources folder in Assets
            TTS_Creator.TextToSpeech.createTTS(TTSArray[x], fileNameFull, "GuyNeural").Wait();
        }
    }

    // Update is called once per frame
    // Update is not used.
    void Update()
    {
    }
}
