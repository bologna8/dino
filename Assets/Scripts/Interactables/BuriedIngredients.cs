using System.Collections;
using UnityEngine;

public class BuriedIngredients : MonoBehaviour
{
    public GameObject pickupEffectPrefab; // Prefab for the pick-up effect
    public Item[] buriedIngredientItems; // Array of buried ingredient items
    public float timeToDig = 1f;
    [HideInInspector] public bool playerTouching = false;
    private Inventory inventory; // Reference to the player's inventory script

    void Start()
    {
        inventory = FindObjectOfType<Inventory>(); // Find the player's inventory script
        if (inventory == null)
        {
            Debug.LogError("Inventory script not found in the scene!");
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerControls>();
        if (player)
        {
            playerTouching = true;
            if (player.interacting)
            {
                player.Dig(timeToDig);
                StartCoroutine(delayedDig(timeToDig));
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerControls>();
        if (player && playerTouching) { playerTouching = false; }
    }

    IEnumerator delayedDig(float digTime)
    {
        yield return new WaitForSeconds(digTime);
        if (playerTouching)
        {
            foreach (var item in buriedIngredientItems)
            {
                if (item)
                {
                    // Add the buried ingredient item to the player's inventory
                    inventory.AddItem(item);



                    // Show the pick-up effect
                    if (pickupEffectPrefab)
                    {
                        Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
                    }

                    // Destroy the buried ingredients handler object after digging them up
                    Destroy(gameObject);
                }
            }
        }
    }
}
