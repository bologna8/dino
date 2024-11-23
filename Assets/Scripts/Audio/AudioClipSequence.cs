using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioClipSequence : MonoBehaviour
{

    //
    //  This script adapted from here: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AudioSource.PlayScheduled.html
    //
    public List<AudioClip> audioClips;

    public List<bool> loopClip;

    public AudioMixerGroup Output;

    public bool debugNextClip = false;

    private int currentClip = 0;

    private int previousClip = 0;

    private double nextEventTime;
    private int flip = 0;
    private AudioSource[] audioSources = new AudioSource[2];
    private bool running = false;
    void Start()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject child = new GameObject("AudioSource");
            child.transform.parent = gameObject.transform;
            audioSources[i] = child.AddComponent<AudioSource>();
            audioSources[i].outputAudioMixerGroup = Output;
        }

        //running = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!running)
        {
            return;
        }

       if(debugNextClip){
            LoadNextClip();
            debugNextClip = false;
        } 

        

        double time = AudioSettings.dspTime;

        if (time + 2.0f > nextEventTime){
            if(!loopClip[currentClip]){
                LoadNextClip();
            }
        }

        if (time + 1.0f > nextEventTime)
        {
            
            
            audioSources[flip].clip = audioClips[currentClip];
            audioSources[flip].PlayScheduled(nextEventTime);
            

            
            Debug.Log("Scheduled source " + flip + " to start at time " + nextEventTime);

            
            nextEventTime += audioClips[currentClip].length;

            flip = 1 - flip;

            previousClip = currentClip;
        }
    }
    

    private void LoadNextClip(){
        if(previousClip == currentClip){
            currentClip++;
        }
    }

    public void PlayFirstClip(){
        
        bool aSourceIsPlaying = false;

        foreach(AudioSource audSrc in audioSources){
            if(audSrc.isPlaying){aSourceIsPlaying = true;}
        }

        if(!aSourceIsPlaying){
            audioSources[flip].clip = audioClips[currentClip];
            audioSources[flip].Play();
            flip = 1 - flip;
            nextEventTime = AudioSettings.dspTime + audioClips[currentClip].length;
            Debug.Log("Scheduled source " + flip + " to start at time " + nextEventTime);
        }

        running = true;
    }

    public void ResetAudioSequence(){
        currentClip = 0;
        previousClip = 0;
        flip = 0;
        foreach(AudioSource audSrc in audioSources){
            audSrc.Stop();
            audSrc.clip = null;
        }
    }

    public void BreakThruLoop(int loopIndex){
        if(loopIndex == currentClip){
            currentClip++;
        }
    }
}
