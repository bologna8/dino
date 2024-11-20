using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems; // For detecting UI interactions
using UnityEngine.Rendering.Universal;

public class LanternController : MonoBehaviour
{
    public GameObject lantern;
    public Item oilItem; 
    public float maxEnergy = 100f; 
    public float currentEnergy; 
    public float depletionRate = 5f; 
    public float refillAmount = 50f;

    [HideInInspector]
    public bool isLanternActive = true;
    private bool isLanternInInventory = false;
    private Inventory inventory;

    public Item lanternItem; 

    private float lastClickTime = 0f; 
    public float doubleClickTime = 0.3f;

    [SerializeField]
    private Light2D lanternLight;

    [Header("The color of light the lantern has when at\nfull energy and nearly empty energy.")]
    [SerializeField]
    private Color maxLightColor = Color.white;
    [SerializeField]
    private Color minLightColor = Color.black;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        lantern.SetActive(true); 
        isLanternActive = true; 
        currentEnergy = maxEnergy;

        if (inventory == null)
        {
            Debug.LogError("Inventory script not found in the scene!");
        }
        else
        {
            inventory.onItemChange += UpdateLanternStatus;
        }

        lanternLight = lantern.GetComponent<Light2D>();
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
        if (isLanternInInventory && InventoryUI.Instance.inventoryOpen)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUI())
            {
                HandleLanternClick();
            }
        }

        UpdateLanternLight();
    }

    /// <summary>
    /// Toggles the lantern state on click, only if clicked directly in inventory.
    /// </summary>
    void HandleLanternClick()
    {
        float currentTime = Time.time;

        // Toggle lantern state
        ToggleLantern();

        lastClickTime = currentTime; 
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

    /// <summary>
    /// Updates whether the lantern is in the inventory.
    /// </summary>
    void UpdateLanternStatus()
    {
        isLanternInInventory = inventory.ContainsItem(lanternItem, 1);

        if (!isLanternInInventory)
        {
            lantern.SetActive(false); 
            isLanternActive = false; 
        }
    }

    /// <summary>
    /// Prevents unwanted toggling when clicking on other UI elements.
    /// </summary>
    /// <returns>True if pointer is over a UI element; otherwise false.</returns>
    private bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    void UpdateLanternLight()
    {
        // Light update logic (optional, omitted here for brevity)
    }
}
