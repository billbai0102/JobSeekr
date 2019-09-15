using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWalk : MonoBehaviour
{
    public static int speed = 4;
    bool goingUp = false;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.z > -14.5)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (transform.position.y < 1.7 & goingUp)
                transform.Translate(Vector3.up * 1 * Time.deltaTime);
            else
            {
                goingUp = false;
                if (transform.position.y < 1.5)
                    goingUp = true;
                transform.Translate(Vector3.down * 1 * Time.deltaTime);
            }
        }
        else
        {
            SceneManager.LoadScene("InterviewMain");
        }
        float x = transform.position.x;
        //Debug.Log(transform.position.z);
    }

    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("InterviewMain");
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Pro :" + asyncOperation.progress);
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            Debug.Log("Loading progress: " + (asyncOperation.progress * 100) + "%");
            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                //Wait to you press the space key to activate the Scene
                if (transform.position.z < -15.4) {
                //asyncOperation.allowSceneActivation = true;
                Debug.Log("SWITCH");
                }
            }

            yield return null;
        }
    }
}
