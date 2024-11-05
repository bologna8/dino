using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buried : LayerCheck, IInteractable
{
    public float timeToDig = 1f;
    public List<GameObject> possiblePickupPrefabs;

    public AnimationClip diggingAnimation;

    [HideInInspector] public bool digging;

    private Core touchingCore;

    public void Interact(GameObject interacter)
    {
        if (possiblePickupPrefabs.Count > 0 && touchingCore)
        {
            touchingCore.Stun(timeToDig, diggingAnimation);
            StartCoroutine(DelayedDig());
        }
        
    }

    public override void ExtraEnterOperations(Collider2D collision)
    {
        var coreCheck = collision.gameObject.GetComponent<Core>();
        if (coreCheck && !touchingCore)
        {
            touchingCore = coreCheck;
            touchingCore.interactables.Add(this);
        }
    }

    public override void ExtraExitOperations(Collider2D collision)
    {
        if (touchingCore)
        {
            touchingCore.interactables.Remove(this);
            touchingCore = null;
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
