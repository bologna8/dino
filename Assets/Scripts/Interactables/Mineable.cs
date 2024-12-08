using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  
using UnityEngine.UI;

public class Mineable : LayerCheck, IInteractable
{

    public Text interactionPrompt;  //UI text
    public Vector2 numberOfDrops = new Vector2(1, 1);
    private bool isGamepad;
    public List<GameObject> possiblePickupPrefabs;
    public AnimationClip miningAnimation;
    public float mineTime = 1f;
    public Item requiredToMine;
    private bool mining;

    private Core touchingCore;

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

        if (interactionPrompt != null && touchingCore)
        {
            interactionPrompt.text = isGamepad ? "Press [X] to interact" : "Press [E] to interact";
        }

    }

    public override void ExtraEnterOperations(Collider2D collision)
    {
        var coreCheck = collision.gameObject.GetComponent<Core>();
        if (coreCheck && !touchingCore)
        {
            touchingCore = coreCheck;
            touchingCore.interactables.Add(this);

            if (Inventory.instance == null) { return; }
            if (interactionPrompt != null && Inventory.instance.ContainsItem(requiredToMine, 1))
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

    public void Interact(GameObject interacter)
    {
        if (Inventory.instance == null) { return; } 
        if (!Inventory.instance.ContainsItem(requiredToMine, 1)) { return; }
        if (!touching || mining) { return; }

        touchingCore.Stun(mineTime, miningAnimation);
        StartCoroutine(DelayedBreak());
    }

    public IEnumerator DelayedBreak()
    {
        mining = true;
        yield return new WaitForSeconds(mineTime);
        mining = false;

        if (touchingCore != null)
        {
            SpawnPickups();
            gameObject.SetActive(false); // Disable or destroy the object after mining
        }
    }

    private void SpawnPickups()
    {
        if (possiblePickupPrefabs.Count > 0)
        {
            int amount = Random.Range((int)numberOfDrops.x, (int)numberOfDrops.y);
            for (int i = 0; i < amount; i++)
            {
                var randomIndex = Random.Range(0, possiblePickupPrefabs.Count);
                var chosenPickup = possiblePickupPrefabs[randomIndex];

                if (PoolManager.Instance != null)
                {
                    PoolManager.Instance.Spawn(chosenPickup, transform.position);
                }
                else
                {
                    Instantiate(chosenPickup, transform.position, Quaternion.identity);
                }
            }
        }
    }

}
