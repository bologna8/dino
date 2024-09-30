using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigSFX : MonoBehaviour
{
    public Buried buriedScript;

    public GameObject SFXToSpawn;

    private bool previous;
    void Start()
    {
        previous = buriedScript.digging;
    }

    // Update is called once per frame
    void Update()
    {
        if(previous != buriedScript.digging){
            if(buriedScript.digging){
                PoolManager.Instance.Spawn(SFXToSpawn, transform.position, transform.rotation);
            }

            previous = buriedScript.digging;

        }
    }
}
