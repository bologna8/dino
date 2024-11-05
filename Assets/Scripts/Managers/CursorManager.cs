using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    private InventoryUI inventoryUi; 
    void Start()
    {

        #if !UNITY_EDITOR
        Cursor.visible = false;
        #endif
        
        //Why lock the mouse? It is needed to aim 
        //Cursor.lockState = CursorLockMode.Locked;

        if(inventoryUi.inventoryOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
