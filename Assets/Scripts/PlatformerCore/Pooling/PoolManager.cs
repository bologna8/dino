using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public Transform CanvasTransform;

    private List<ObjectPool> allPools = new List<ObjectPool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogWarning("Deleted Duplicate Spawn Pool Manager");
            return;
        }

        Instance = this;
        
        if (!CanvasTransform) { CanvasTransform = GameObject.Find("Canvas").transform; }
    }


    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation = default, Transform spawnSource = null, int teamNumber = 0, bool ignoreTeams = false, bool UIelement = false)
    {
        GameObject spawnedObject;
        
        ObjectPool foundPool = null;
        foreach(var p in allPools) 
        { 
            if (p.name == prefab.name) { foundPool = p; }
        }

        if (foundPool == null) //If no pool for objects exists, make a new one
        { 
            foundPool = new ObjectPool();
            foundPool.name = prefab.name;
            allPools.Add(foundPool); 
        }


        if (foundPool.activeQ.Count >= foundPool.maxSize) //Reset last spawned object if over the spawn limit
        { foundPool.activeQ[0].SetActive(false); }

        var SetParent = transform;
        if (CanvasTransform && UIelement) { SetParent = CanvasTransform; }
        
        if (foundPool.inactiveQ.Count > 0) //Reuse inactive objects from the pool if there are any
        {
            spawnedObject = foundPool.inactiveQ[0];
            foundPool.inactiveQ.RemoveAt(0);
        }
        else //Create new prefab only if needed
        { 
            spawnedObject = Instantiate(prefab, SetParent);
            spawnedObject.SetActive(false); //Start Inactive while setting stats
        }

        //spawnedObject.SetActive(false); 


        foundPool.activeQ.Add(spawnedObject);

        spawnedObject.transform.rotation = rotation;
        spawnedObject.transform.position = position;
        

        var spawnSats = spawnedObject.GetComponent<Spawned>();
        if (spawnSats)
        {
            foundPool.maxSize = spawnSats.maxCount;

            spawnSats.source = spawnSource;
            
            spawnSats.team = teamNumber;
            spawnSats.ignoreTeams = ignoreTeams;
            spawnSats.myPool = foundPool;
            
        } 

        spawnedObject.SetActive(true);
        return spawnedObject;
    }



}
