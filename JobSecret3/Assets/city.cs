using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class city : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //GameObject.Find("Main Camera").transform.position = new Vector3(0f, 0f, 2f);
    }

    public Vector3 myPos;
    public Transform myPlay;

    public void Update()
    {
        //transform.position = myPlay.position + myPos;
    }
}
