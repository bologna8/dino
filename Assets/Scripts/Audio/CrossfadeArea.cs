using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossfadeArea : MonoBehaviour
{
    
    public MusicPlayer mp;

    public MusicPlayer.Track track;

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.gameObject.tag == "Player"){
            if(!mp.doingCrossfade){
                mp.trackToBringOut = track;
                mp.doingCrossfade = true;
            }
        }
    }
}
