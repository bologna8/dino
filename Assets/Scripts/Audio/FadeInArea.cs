using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInArea : MonoBehaviour
{
    private NewMusicPlayer mp;

    public NewMusicPlayer.Track track;

    public float fadeTime = 3f;

    private bool isFadingIn = false;

    // Update is called once per frame

    void Start(){
        mp = GameObject.FindWithTag("MusicPlayer").GetComponent<NewMusicPlayer>();
        if(mp == null){
            Debug.Log("Error: No Music Player detected in scene");
        }
    }

    void Update(){

        if(isFadingIn && mp != null){
            bool b = mp.FadeIn(track,3f);
            isFadingIn = !b;
        }

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.gameObject.tag == "Player"){
            isFadingIn = true;
        }
    }
}
