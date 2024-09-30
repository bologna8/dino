using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClingSFX : MonoBehaviour
{
    public Movement movementScript;

    public GameObject SFXToSpawn;

    private bool previous;
    void Start()
    {
        previous = movementScript.wallSliding;
    }

    // Update is called once per frame
    void Update()
    {
        if(previous != movementScript.wallSliding){
            if(movementScript.wallSliding){
                PoolManager.Instance.Spawn(SFXToSpawn, transform.position, transform.rotation);
            }

            previous = movementScript.wallSliding;

        }
    }
}
