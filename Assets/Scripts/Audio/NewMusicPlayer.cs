using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class NewMusicPlayer : MonoBehaviour
{
  [Header("References")]
    public AudioSource Primary;
    public List<AudioSource> Secondary;

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
            Debug.Log("Error: there is no primary and secondary audio sources assigned to music player");
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
    private bool FadeOut(List<string> allVolumes, float fadeTime){  // Fades Out all Music Tracks. returns true when complete;
        
        if(FadeOutDuration >= fadeTime){
            FadeOutDuration = 0f;
            return true;
        }
        
        FadeOutDuration += Time.deltaTime;

        float logValue = (float) FadeOutDuration / fadeTime;
        float invlogValue = 1 - logValue;
        if(invlogValue == 0){
            invlogValue = 0.001f;
        }

        foreach(string str in allVolumes){
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
        
        if(FadeInDuration >= fadeTime){
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


}
