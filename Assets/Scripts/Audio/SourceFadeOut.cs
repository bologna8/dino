using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceFadeOut : MonoBehaviour
{
    public AudioSource audioSource;

    public double TimeUntilStart;

    public double fadeDuration;

    private double startingVolume;

    private double timeAlive;
    void Start()
    {
        startingVolume = audioSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;

        if(timeAlive >= TimeUntilStart + fadeDuration){
            Destroy(this.gameObject);
        }
        else if(timeAlive >= TimeUntilStart){
            DecreaseVolume();
        }
    }

    private void DecreaseVolume(){
        audioSource.volume -= (float) (Time.deltaTime * startingVolume / fadeDuration);
    }
}
