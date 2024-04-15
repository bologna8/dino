using UnityEngine;

public class LanternController : MonoBehaviour
{
    // Reference to the lantern GameObject
    public GameObject lantern;

    // Check if the lantern is currently active
    private bool isLanternActive = false;

    // Check if the lantern is in the player's inventory
    private bool isLanternInInventory = false;

    // Reference to the Inventory script
    private Inventory inventory;

    // Reference to the lantern item
    public Item lanternItem;

    // Subscribe to events or callbacks from the inventory system
    void Start()
    {
        // Find the Inventory script in the scene
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory script not found in the scene!");
        }
        else
        {
            // Subscribe to the event that notifies when the inventory changes
            inventory.onItemChange += UpdateLanternStatus;
        }
    }

    // Unsubscribe from events when the object is destroyed
    void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (inventory != null)
        {
            inventory.onItemChange -= UpdateLanternStatus;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the lantern is in the inventory
        if (isLanternInInventory)
        {
            // Check for input to toggle the lantern
            if (Input.GetKeyDown(KeyCode.L))
            {
                ToggleLantern();
            }
        }
        else
        {
           return;
        }
    }

    // Function to toggle the lantern on/off
    void ToggleLantern()
    {
        // If the lantern is currently active, deactivate it
        if (isLanternActive)
        {
            lantern.SetActive(false);
            isLanternActive = false;
        }
        // If the lantern is currently inactive, activate it
        else
        {
            lantern.SetActive(true);
            isLanternActive = true;
        }
    }

    // Function to update the lantern status based on the inventory
    void UpdateLanternStatus()
    {
        // Check if the lantern is in the player's inventory
        isLanternInInventory = inventory.ContainsItem(lanternItem, 1);
    }
}
