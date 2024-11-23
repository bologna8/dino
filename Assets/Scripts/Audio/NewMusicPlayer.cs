using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class NewMusicPlayer : MonoBehaviour
{
  [Header("References")]

    public List<GameObject> musicTrackObjects;
    private AudioSource Primary;
    private List<AudioSource> Secondary = new List<AudioSource> {};

    public AudioMixer Mixer;
    
    private const string track1 = "T1Vol";
    private const string track2 = "T2Vol";
    private const string track3 = "T3Vol";
    private const string track4 = "T4Vol";

    private static readonly List<string> tracks = new List<string> {track1, track2, track3, track4};

    

    //public bool doingCrossfade = false;
    public enum Track {track1, track2, track3, track4}

    [Header("Crossfade Properties")]
    public Track trackToBringOut = 0;

    [HideInInspector] public double CrossfadeTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        //if(Mixer != null) Mixer.SetFloat(track1, 0); //Temporary

        /*
        Primary = musicTrackObjects[0].GetComponent<AudioSource>();

        for(int i = 1; i < musicTrackObjects.Count; i++){
            Secondary.Add(musicTrackObjects[i].GetComponent<AudioSource>());
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        Sync();

        
    }

    private void Sync(){

        if(Primary != null && Secondary != null){
            foreach(AudioSource source in Secondary){
                source.timeSamples = Primary.timeSamples;       
            }
        }else{
            //Debug.Log("Error: there is no primary and secondary audio sources assigned to music player");
        }

    }

/*
    void Crossfade(string volA, List<string> allVolumes, float fadeTime){  // Makes one track turn on, and all others turn off.
        
        if(CrossfadeTime > fadeTime){
            doingCrossfade = false;
            CrossfadeTime = 0;
            return;   
        }else{

            CrossfadeTime += Time.deltaTime;
            
            float logValue = (float) CrossfadeTime / fadeTime;
            float invlogValue = 1 - logValue;
            if(invlogValue == 0){
                invlogValue = 0.001f;
            }

            foreach(string str in allVolumes){
                if(Mixer != null){
                    float x; 
                    Mixer.GetFloat(str, out x);
                    if(str == volA){
                        if(x < Mathf.Log10(logValue) * 26){
                            Mixer.SetFloat(str, Mathf.Log10(logValue) * 26);
                        }
                    }else{
                        if(x > Mathf.Log10(invlogValue) * 26){
                            Mixer.SetFloat(str, Mathf.Log10(invlogValue) * 26);
                        }
                    }
                }else{
                    Debug.Log("Error: No Mixer assigned to music player");
                }
            }

        }

        
    }
*/

    private float FadeOutDuration = 0f;
    public bool FadeOut(float fadeTime){  // Fades Out all Music Tracks. returns true when complete;
        
        FadeInDuration = -2;

        if(FadeOutDuration >= fadeTime){
            FadeOutDuration = 0f;
            FadeInDuration = 0f;

            foreach(string str in tracks){
                if(Mixer != null){
                    Mixer.SetFloat(str, -80);  
                }else{
                    Debug.Log("Error: No Mixer assigned to music player");
                }
            }

            return true;
        }
        
        FadeOutDuration += Time.deltaTime;

        float logValue = (float) FadeOutDuration / fadeTime;
        float invlogValue = 1 - logValue;
        if(invlogValue == 0){
            invlogValue = 0.001f;
        }

        foreach(string str in tracks){
            if(Mixer != null){
                float x; 
                Mixer.GetFloat(str, out x);
                if(x > Mathf.Log10(invlogValue) * 26){
                    Mixer.SetFloat(str, Mathf.Log10(invlogValue) * 26);
                }    
            }else{
                Debug.Log("Error: No Mixer assigned to music player");
            }
        }

        return false;
    }

    private float FadeInDuration = 0f;
    public bool FadeIn(Track track, float fadeTime){  // Fades In specified Music Track. returns true when complete;
        
        if(FadeInDuration >= fadeTime || FadeInDuration <= -1f){
            FadeInDuration = 0f;
            return true;
        }
        
        FadeInDuration += Time.deltaTime;

        float logValue = (float) FadeInDuration / fadeTime;
        
        if(Mixer != null){
            float x; 
            Mixer.GetFloat(tracks[(int)track], out x);
            if(x < Mathf.Log10(logValue) * 26){
                Mixer.SetFloat(tracks[(int)track], Mathf.Log10(logValue) * 26);
                //Debug.Log(tracks[(int)track] + " volume = " + Mathf.Log10(logValue) * 26);
            }    
        }else{
            Debug.Log("Error: No Mixer assigned to music player");
        }
        

        return false;
    }

    public void PlayMusic(Track track){
        AudioSource a = musicTrackObjects[(int)track].GetComponent<AudioSource>();
        AudioClipSequence acs = musicTrackObjects[(int)track].GetComponent<AudioClipSequence>();

        if(acs != null){
            acs.PlayFirstClip();
        }
        if(a != null && !a.isPlaying){
            a.Play();
        }
    }

    public void StopMusic(){
        foreach(GameObject g in musicTrackObjects){
            
            if(g.GetComponent<AudioSource>() != null){
                g.GetComponent<AudioSource>().Stop();
            }

            if(g.GetComponent<AudioClipSequence>() != null){
                g.GetComponent<AudioClipSequence>().ResetAudioSequence();
            }

        }
    }

}
