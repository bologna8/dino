using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDrops : MonoBehaviour
{
    public List<GameObject> potentialDrops = new List<GameObject>();
    public int minDrops = 0;
    public int maxDrops = 1;

    // Start is called before the first frame update
    void Start()
    {
        var numberOfDrops = Random.Range(minDrops, maxDrops +1);

        for (int i = 0; i < numberOfDrops; i++)
        {
            var drop = potentialDrops[Random.Range(0, potentialDrops.Count)];
            Instantiate(drop, transform.position, Quaternion.identity);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
