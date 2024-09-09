using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buried : Interactable
{
    public float timeToDig = 1f;
    public List<GameObject> possiblePickupPrefabs;

    public override void Interacted(Core interCore)
    {
        if (possiblePickupPrefabs.Count > 0)
        {
            interCore.Stun(timeToDig);
            StartCoroutine(DelayedDig());
        }
        
    }

    public IEnumerator DelayedDig()
    {
        yield return new WaitForSeconds(timeToDig);

        if (player)
        {
            var r = Random.Range(0, possiblePickupPrefabs.Count);
            var chosen = possiblePickupPrefabs[r];
            PoolManager.Instance.Spawn(chosen, transform.position);
            possiblePickupPrefabs.RemoveAt(r); 
        }
    }
}
