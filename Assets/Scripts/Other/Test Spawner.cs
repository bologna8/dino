using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    
    public GameObject effectToSpawn;

    public double spawnRate;

    private double spawnTime = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnTime += Time.deltaTime;
        if(spawnTime >= spawnRate){
            if(effectToSpawn != null) Instantiate(effectToSpawn);
            spawnTime = 0;
        }
    }
}
