using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public GameObject craftingUI; // Reference to the crafting UI GameObject
    public CraftingTable[] craftingTables; // Array of crafting table objects

    private void Update()
    {
        // Check if any of the crafting tables are within range of the player
        bool playerNearTable = false;
        foreach (CraftingTable table in craftingTables)
        {
            if (table != null && table.IsPlayerInRange())
            {
                playerNearTable = true;
                break;
            }
        }

        // Enable or disable the crafting UI based on player's proximity to any crafting table
        craftingUI.SetActive(playerNearTable);
    }
}
