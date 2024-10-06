using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimultaneousSpawn : MonoBehaviour
{
    public GameObject thingToSpawn;
    void OnEnable()
    {
        
        if(thingToSpawn != null) PoolManager.Instance.Spawn(thingToSpawn, transform.position, transform.rotation, null);
    }

  
}
