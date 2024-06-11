using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private InventoryUi inventoryUi; 
    void Start()
    {
        Cursor.visible = false;
        
        //Why lock the mouse? It is needed to aim 
        //Cursor.lockState = CursorLockMode.Locked;

        if(inventoryUi.InventoryOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
