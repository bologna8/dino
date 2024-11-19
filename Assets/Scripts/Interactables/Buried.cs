using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  
using UnityEngine.UI;       
   
public class Buried : LayerCheck, IInteractable
{
    public float timeToDig = 1f;
    public List<GameObject> possiblePickupPrefabs;
    public AnimationClip diggingAnimation;
    public Text interactionPrompt;  //UI text

    [HideInInspector] public bool digging;
    private Core touchingCore;
    private bool isGamepad;

    private void Awake()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        GameObject textGO = GameObject.FindGameObjectWithTag("Interaction Text");
        interactionPrompt = textGO.GetComponent<Text>();
    }

    private void Update()
    {
        isGamepad = Gamepad.current != null;

        if (interactionPrompt != null && touchingCore)
        {
            interactionPrompt.text = isGamepad ? "Press [X] to interact" : "Press [E] to interact";
        }
    }

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

            if (interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(true);
            }
        }
    }

    public override void ExtraExitOperations(Collider2D collision)
    {
        if (touchingCore)
        {
            touchingCore.interactables.Remove(this);
            touchingCore = null;

            if (interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator DelayedDig()
    {
        digging = true;
        yield return new WaitForSeconds(timeToDig);
        digging = false;

        if (touching)
        {
            var r = Random.Range(0, possiblePickupPrefabs.Count);
            var chosen = possiblePickupPrefabs[r];
            PoolManager.Instance.Spawn(chosen, transform.position);
            possiblePickupPrefabs.RemoveAt(r);
        }
        Destroy(gameObject);
    }
}
