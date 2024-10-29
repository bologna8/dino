using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buried : LayerCheck, IInteractable
{
    public float timeToDig = 1f;
    public List<GameObject> possiblePickupPrefabs;

    public AnimationClip diggingAnimation;

    [HideInInspector] public bool digging;

    public void Interact(GameObject interacter)
    {
        if (possiblePickupPrefabs.Count > 0)
        {
            var coreCheck = interacter.GetComponent<Core>();
            if (coreCheck)
            {
                coreCheck.Stun(timeToDig, diggingAnimation);
                StartCoroutine(DelayedDig());
            }
        }
        
    }

    public IEnumerator DelayedDig()
    {
        digging = true;
        yield return new WaitForSeconds(timeToDig);
        digging = false;

        if (touching) //This is why layercheck inhereted
        {
            var r = Random.Range(0, possiblePickupPrefabs.Count);
            var chosen = possiblePickupPrefabs[r];
            PoolManager.Instance.Spawn(chosen, transform.position);
            possiblePickupPrefabs.RemoveAt(r); 
        }
        Destroy(gameObject);
    }
}
