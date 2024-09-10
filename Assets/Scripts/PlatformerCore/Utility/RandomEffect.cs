using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEffect : MonoBehaviour
{
    public GameObject[] possibleEffects;
    [Tooltip("If there is more than 1 of the same option, choose new effect each time")] public bool allowRepeats;
    private string lastEffectName;

    void OnEnable()
    {
        if (possibleEffects.Length > 0)
        {
            var chosen = possibleEffects[0];
            if (possibleEffects.Length > 1)
            {
                var r = Random.Range(0, possibleEffects.Length);
                chosen = possibleEffects[r];
                
                if (!allowRepeats) 
                {
                    while (lastEffectName == chosen.name)
                    {
                        r = Random.Range(0, possibleEffects.Length);
                        chosen = possibleEffects[r];
                    }
                    lastEffectName = chosen.name;
                }

            }

            
            if (PoolManager.Instance != null) { PoolManager.Instance.Spawn(chosen, transform.position); }
            else { Instantiate(chosen, transform.position, transform.rotation); }
        }

        

    }
}
