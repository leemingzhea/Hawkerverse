using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCheck : MonoBehaviour
{
    public AudioSource myAudio;

    public bool wasPlaying = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (wasPlaying)
        {
            if (myAudio.isPlaying)
            {
                Debug.Log("Audio is playing");
            }
            else
            {
                Debug.Log("Audio is not playing");
            }
            wasPlaying = myAudio.isPlaying;
        }
    }
}
