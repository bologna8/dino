using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipSequenceAdvanceTransition : MonoBehaviour
{
    public AudioClipSequence audioClipSequence;

    public int loopIndex;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.transform.gameObject.tag == "Player"){
            audioClipSequence.BreakThruLoop(loopIndex);
        }
    }
}
