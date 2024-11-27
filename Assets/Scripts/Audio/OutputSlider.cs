using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputSlider : MonoBehaviour
{
    private NewMusicPlayer mp;

    public UnityEngine.UI.Slider slider;
    public NewMusicPlayer.Output output;

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
        
    }

    public void AdjustVolume(){
        mp.AdjustVolume(slider, output);
    }
}
