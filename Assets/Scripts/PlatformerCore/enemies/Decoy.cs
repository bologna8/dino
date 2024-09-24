using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour
{
    [Tooltip("Alerts enemies that have not seen you yet")] public bool drawAttention = false;
    [Tooltip("Takes the agro if enemy is currently agro on something else")] public bool overrideAgro = false;
    [Tooltip("Causes foolish enemies to go attack in a way that can damage teammates")] public bool causeFrenzy = false;

    private List<AI> alerted = new List<AI>();


    void OnTriggerStay2D(Collider2D other)
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

                if (causeFrenzy && ai.foolish) { ai.frenzied = true; }

                alerted.Add(ai);
            }
            else //Keep agroing those that linger
            {
                if (ai.chasing != transform && overrideAgro) 
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
    }

    void OnDisable()
    {
        if (alerted.Count > 0 && causeFrenzy)
        {
            foreach (var ai in alerted) 
            { ai.frenzied = false; }
            alerted.Clear();
        }
    }
}
