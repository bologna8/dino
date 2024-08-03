using UnityEngine;

//if the player(with the player tag) is in the range of the crafting table it can be used 
public class CraftingTable : MonoBehaviour
{
    private bool playerInRange = false;

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        playerInRange = true;
    }
}

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public bool IsPlayerInRange()
    {
        return playerInRange;
    }
}
