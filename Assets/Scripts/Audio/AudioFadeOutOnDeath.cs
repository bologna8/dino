using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeOutOnDeath : MonoBehaviour
{
    // Start is called before the first frame update

    public Health hp;

    private NewMusicPlayer mp;

    private bool isFadingOut = false;

    public float fadeTime = 0.3f;
    void Start()
    {
        mp = GameObject.FindWithTag("MusicPlayer").GetComponent<NewMusicPlayer>();
        if(mp == null){
            Debug.Log("Error: No Music Player detected in scene");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(isFadingOut && mp != null){
            bool b = mp.FadeOut(fadeTime);
            if(b){
                mp.StopMusic();
            }
            isFadingOut = !b;
        }

        if(hp.currentHP <= 0){
            isFadingOut = true;
        }
        
    }
}
