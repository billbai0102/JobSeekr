using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TTSTest : MonoBehaviour
{
    public InputField field;
    public Button button;
    
    public string fileName = "/question";

    // Start is called before the first frame update
    void Start()
    {
        string[] TTSArray = File.ReadAllLines("jobs.txt");
        for (int x = 0; x < 3; x++)
        {
            string fileNameFull = fileName + x.ToString();
            tts_sample.Program.createTTS(TTSArray[x], Application.persistentDataPath + fileNameFull, "GuyNeural");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
