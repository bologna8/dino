using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour
{
    public bool drawAttention = false;
    public bool overrideAgro = false;
    public bool bait = false;

    private List<AI> alerted = new List<AI>();

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
            if (!alerted.Contains(ai))
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

    void OnTriggerExit2D(Collider2D other)
    {
        var ai = other.GetComponent<AI>();
        if(ai && bait)
        {
            if (alerted.Contains(ai)) 
            { ai.frenzied = false; alerted.Remove(ai); }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var ai = other.GetComponent<AI>();
        if(ai && bait)
        {   
            //if (ai.carnivore && bait) { ai.Agro(transform); ai.frenzied = true; }
            if (!alerted.Contains(ai)) { alerted.Add(ai); }
        }
    }

    void OnDestroy()
    {
        if (alerted.Count > 0 && bait)
        {
            foreach (var ai in alerted)
            { ai.frenzied = false; }
        }
    }
}
