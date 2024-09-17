using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
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

    
    

    [Header("Crossfade Properties")]
    public bool doingCrossfade = false;
    public enum Track {track1, track2, track3, track4}
    public Track trackToBringOut = 0;

    [HideInInspector] public double CrossfadeTime = 0;

    // Start is called before the first frame update
    void Start()
    {
         Mixer.SetFloat(track1, 0); //Temporary
    }

    // Update is called once per frame
    void Update()
    {
        foreach(AudioSource source in Secondary){
            source.timeSamples = Primary.timeSamples;       
        }

        if(doingCrossfade){
            Crossfade(tracks[(int)trackToBringOut], tracks, 3f);
        }
    }

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
            }

        }

        
    }
}
