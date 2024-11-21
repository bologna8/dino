using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutArea : MonoBehaviour
{
    private NewMusicPlayer mp;

    public float fadeTime = 3f;

    private bool isFadingOut = false;

    // Update is called once per frame

    void Start(){
        mp = GameObject.FindWithTag("MusicPlayer").GetComponent<NewMusicPlayer>();
        if(mp == null){
            Debug.Log("Error: No Music Player detected in scene");
        }
    }

    void Update(){

        if(isFadingOut && mp != null){
            bool b = mp.FadeOut(fadeTime);
            isFadingOut = !b;
        }

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.gameObject.tag == "Player"){
            isFadingOut = true;
        }
    }
}
