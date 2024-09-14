using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buried : LayerCheck, IInteractable
{
    public float timeToDig = 1f;
    public List<GameObject> possiblePickupPrefabs;

    public void Interact(GameObject interacter)
    {
        if (possiblePickupPrefabs.Count > 0)
        {
            var coreCheck = interacter.GetComponent<Core>();
            if (coreCheck)
            {
                coreCheck.Stun(timeToDig);
                StartCoroutine(DelayedDig());
            }
        }
        
    }

    public IEnumerator DelayedDig()
    {
        yield return new WaitForSeconds(timeToDig);

        if (touching) //This is why layercheck inhereted
        {
            var r = Random.Range(0, possiblePickupPrefabs.Count);
            var chosen = possiblePickupPrefabs[r];
            PoolManager.Instance.Spawn(chosen, transform.position);
            possiblePickupPrefabs.RemoveAt(r); 
        }
    }
}
