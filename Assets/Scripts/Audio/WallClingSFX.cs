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
        if(movementScript != null) previous = movementScript.wallSliding;
    }

    // Update is called once per frame
    void Update()
    {
        if(movementScript != null){
            if(previous != movementScript.wallSliding){
                if(movementScript.wallSliding){
                    if(SFXToSpawn != null){
                        PoolManager.Instance.Spawn(SFXToSpawn, transform.position, transform.rotation);
                    }else{
                        Debug.Log("Error: No SFX attached to wallclingsfx");
                    }
                }

                previous = movementScript.wallSliding;

            }
        }else{
            Debug.Log("Error: No movement script attached to wallclingsfx");
        }
    }
}
