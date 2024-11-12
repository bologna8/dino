using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopAtPoint : MonoBehaviour
{
    
    public AudioSource source;

    public float loopStartTime;
    private int samples;
    
    void Start()
    {
        source.playOnAwake = false;
        samples = source.clip.samples;
        //source.time = 75;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(samples + " : " + source.timeSamples);
        if(source.timeSamples >= samples){
            source.Stop();
            source.time = loopStartTime;
            source.Play();
        }
    }
}
