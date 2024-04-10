using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private InventoryUi inventoryUi; 
    void Start()
    {
        Cursor.visible = false;
        
        Cursor.lockState = CursorLockMode.Locked;

        if(inventoryUi.InventoryOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
