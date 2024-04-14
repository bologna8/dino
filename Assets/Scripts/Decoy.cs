using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour
{
    public bool drawAttention = false;
    public bool overrideAgro = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var ai = other.GetComponent<AI>();
        if(ai)
        {
            if (ai.currentState == AI.State.chase) 
            {
                if (overrideAgro) { ai.Agro(transform); }
            }
            else
            {
                if (drawAttention) { ai.Agro(transform); }
            }
      
        }
    }
}
