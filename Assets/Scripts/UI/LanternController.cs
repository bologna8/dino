using UnityEngine;
using UnityEngine.InputSystem; 

public class LanternController : MonoBehaviour
{
    public GameObject lantern; 

    private bool isLanternActive = false;  
    private bool isLanternInInventory = false; 
    private Inventory inventory;  
    public Item lanternItem;  
    void Start()
    {
        inventory = FindObjectOfType<Inventory>();

        lantern.SetActive(false); 
        if (inventory == null)
        {
            Debug.LogError("Inventory script not found in the scene!");
        }
        else
        {
            inventory.onItemChange += UpdateLanternStatus;  
        }
    }

    void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.onItemChange -= UpdateLanternStatus;  
        }
    }

    void Update()
    {
        if (isLanternInInventory && InventoryUI.Instance.InventoryOpen)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ToggleLantern();
            }
        }
    }

    void ToggleLantern()
    {
        if (isLanternActive)
        {
            lantern.SetActive(false);  
            isLanternActive = false;
        }
        else
        {
            lantern.SetActive(true); 
            isLanternActive = true;
        }
    }

    void UpdateLanternStatus()
    {
        isLanternInInventory = inventory.ContainsItem(lanternItem, 1);

        if (!isLanternInInventory)
        {
            lantern.SetActive(false); 
            isLanternActive = false;
        }
    }
}
