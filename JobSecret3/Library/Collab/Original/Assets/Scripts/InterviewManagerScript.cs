using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
namespace InterviewManagerNamespace
{
    public class InterviewManagerScript : MonoBehaviour
    {

        private AudioClip firstSound;
        private AudioClip secondSound;
        private AudioClip thirdSound;

        public AudioSource audio;
        public static bool fiveSec;

        private bool play1 = true;
        private bool play2 = true;
        private bool play3 = true;
        private bool play4 = true;
        private bool play5 = true;

        // Start is called before the first frame update
        void Start()
        {
            firstSound = (AudioClip)Resources.Load("GreetingMale");
            secondSound = (AudioClip)Resources.Load("FirstQMale");

        }

    //    IEnumerator loadQuestions()
    //    {
    //        string path = "file:" + Application.persistentDataPath + "/anything.wav";
    //
    //        WWW www = new WWW(path);
    //        yield return www;
    //        thirdSound = www.GetAudioClip(false, false, AudioType.WAV);
    //    }

        // Update is called once per frame
        void Update()
        {
            if (play1)
            {
                audio.PlayOneShot(firstSound);
                play1 = false;
            }else if (play2 && !audio.isPlaying)
            {
                audio.PlayOneShot(secondSound);
                play2 = false;
            }
            else if (play3)
            {
               // audio.PlayOneShot(thirdSound);
                play3 = false;
            }
        }
    }
}
