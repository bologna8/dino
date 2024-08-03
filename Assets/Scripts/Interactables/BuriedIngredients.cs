using System.Collections;
using UnityEngine;

public class BuriedIngredients : MonoBehaviour
{
    public GameObject pickupEffectPrefab; 
    public Item[] buriedIngredientItems; 
    public float timeToDig = 1f;
    [HideInInspector] public bool playerTouching = false;
    private Inventory inventory; 

    void Start()
    {
        inventory = FindObjectOfType<Inventory>(); 
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
                    inventory.AddItem(item);



                    if (pickupEffectPrefab)
                    {
                        Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}
