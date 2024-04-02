using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private InventoryUI inventoryUI;
    void Start()
    {
        Cursor.visible = false;
        
        Cursor.lockState = CursorLockMode.Locked;

        if(inventoryUI.InventoryOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}