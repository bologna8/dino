using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public GameObject craftingUI; // Reference to the crafting UI GameObject
    public CraftingTable craftingTable; // Reference to the crafting table object

    private void Update()
    {
        // Check if the player is within range of the crafting table
        if (craftingTable != null && craftingTable.IsPlayerInRange())
        {
            // Enable the crafting UI when the player is near the crafting table
            craftingUI.SetActive(true);
        }
        else
        {
            // Disable the crafting UI when the player is not near the crafting table
            craftingUI.SetActive(false);
        }
    }
}
