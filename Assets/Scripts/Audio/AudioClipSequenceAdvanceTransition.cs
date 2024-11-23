using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipSequenceAdvanceTransition : MonoBehaviour
{
    public AudioClipSequence audioClipSequence;

    public int loopIndex;

    private bool willBreakThru = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(willBreakThru){
            bool b = audioClipSequence.BreakThruLoop(loopIndex);
            willBreakThru = !b;
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.transform.gameObject.tag == "Player"){
            willBreakThru = true;
        }
    }
}
