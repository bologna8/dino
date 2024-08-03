using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public GameObject craftingUI; 
    public CraftingTable[] craftingTables; 

//if the player is by the crafting table, enable the crafting interface 
    private void Update()
    {

        bool playerNearTable = false;
        foreach (CraftingTable table in craftingTables)
        {
            if (table != null && table.IsPlayerInRange())
            {
                playerNearTable = true;
                break;
            }
        }

        craftingUI.SetActive(playerNearTable);
    }
}