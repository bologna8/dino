using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  
using UnityEngine.UI;       
   
public class Buried : LayerCheck, IInteractable
{
    public float timeToDig = 1f;
    public List<GameObject> possiblePickupPrefabs;
    public Vector2 numberOfDrops = new Vector2(1,1);
    public AnimationClip diggingAnimation;
    public Item checkInventoryForItem;
    public float resetCooldown;
    private float currentCooldown;
    private bool recharging;
    public Text interactionPrompt;  //UI text

    [HideInInspector] public bool digging;
    private Core touchingCore;
    private bool isGamepad;

    private void Start()
    {
        interactionPrompt = GameManager.instance.InteractionText;
        
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        isGamepad = Gamepad.current != null;

        if (recharging)
        {
            currentCooldown -= Time.deltaTime;
            SetSaturation(0);
            if (currentCooldown <= 0) 
            { recharging = false; SetSaturation(unselectedSaturation); }
        }
        else if (interactionPrompt != null && touchingCore)
        {
            interactionPrompt.text = isGamepad ? "Press [X] to interact" : "Press [E] to interact";
        }
        else if (Inventory.instance && checkInventoryForItem) 
        { 
            if (Inventory.instance.ContainsItem(checkInventoryForItem, 1))
            { gameObject.SetActive(false); }
        }
    }

    public void Interact(GameObject interacter)
    {
        if (possiblePickupPrefabs.Count > 0 && touchingCore && !recharging)
        {
            touchingCore.Stun(timeToDig, diggingAnimation);
            StartCoroutine(DelayedDig());
        }
    }

    public override void ExtraEnterOperations(Collider2D collision)
    {
        var coreCheck = collision.gameObject.GetComponent<Core>();
        if (coreCheck && !touchingCore && !recharging)
        {
            touchingCore = coreCheck;
            touchingCore.interactables.Add(this);

            if (interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(true);
            }
        }
    }

    public override void ExtraExitOperations(Collider2D collision = null)
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
            int amount = Random.Range((int)numberOfDrops.x, (int)numberOfDrops.y);
            for(int i = 0; i < amount; i ++)
            { PoolManager.Instance.Spawn(chosen, transform.position); }
            
            //possiblePickupPrefabs.RemoveAt(r);
        }

        if (resetCooldown > 0) 
        { recharging = true; currentCooldown = resetCooldown; ExtraExitOperations(); }
        else { gameObject.SetActive(false); }
        //Destroy(gameObject);
    }
}
