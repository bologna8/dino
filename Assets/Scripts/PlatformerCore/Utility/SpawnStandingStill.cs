using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStandingStill : MonoBehaviour
{
    [Tooltip("Spawn an object once decoy is done moving")] public GameObject spawnOnceStill;
    public Vector3 spawnOffset;
    public float standStillTime = 0.1f;
    private float currentStillTime;
    private Rigidbody2D myBod;
    private bool spawned = false;
    void OnEnable()
    {
        if (!myBod) { myBod = GetComponentInParent<Rigidbody2D>(); }
        spawned = false;
    }

    void LateUpdate()
    {
        if (myBod && spawnOnceStill && !spawned)
        {
            if (myBod.velocity.magnitude <= 0) 
            {
                currentStillTime += Time.deltaTime;
                if (currentStillTime >= standStillTime)
                {
                    if (PoolManager.Instance) { PoolManager.Instance.Spawn(spawnOnceStill, transform.position + spawnOffset, transform.rotation); }
                    else { Instantiate(spawnOnceStill, transform.position, Quaternion.identity); }
                    spawned = true;
                }
                
            }
            else { currentStillTime = 0f; }
        }
    }
}
