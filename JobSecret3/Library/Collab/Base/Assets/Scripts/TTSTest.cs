using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTSTest : MonoBehaviour
{
    public InputField field;
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(click);
    }

    public void click()
    {
        tts_sample.Program.hello(field.text, "anything", "GuyNeural").Wait();
        field.text = "Done.";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
