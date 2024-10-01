using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEMO_FearBoost : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D player)
    {
        if(player.tag == "Player")
        {
            FearLevel fear = player.gameObject.GetComponentInChildren<FearLevel>();
            fear.fear += 30;
        }
    }
}
