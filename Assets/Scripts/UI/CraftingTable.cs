using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    private bool playerInRange = false;

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
      //  Debug.Log("Player entered crafting table trigger zone");
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
