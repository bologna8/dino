using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedSpawn : MonoBehaviour
{
    public GameObject EffectToSpawn;
    public double DelayUntilSpawn = 0.5;

    private bool spawned = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DelayUntilSpawn -= Time.deltaTime;
        if(DelayUntilSpawn <= 0 && !spawned){
            if (PoolManager.Instance != null) { PoolManager.Instance.Spawn(EffectToSpawn, transform.position); }
            else { Instantiate(EffectToSpawn, transform.position, transform.rotation); }
            spawned = true;
        }
    }
}
