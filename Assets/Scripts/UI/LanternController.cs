using UnityEngine;
using UnityEngine.InputSystem;

public class LanternController : MonoBehaviour
{
    public GameObject lantern;
    public Item oilItem; 
    public float maxEnergy = 100f; 
    public float currentEnergy; 
    public float depletionRate = 5f; 
    public float refillAmount = 50f; 

    private bool isLanternActive = false;
    private bool isLanternInInventory = false;
    private Inventory inventory;

    public Item lanternItem; 

    private float lastClickTime = 0f; 
    public float doubleClickTime = 0.3f; 

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        lantern.SetActive(false);
        currentEnergy = maxEnergy;

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
                HandleLanternClick();
            }
        }

        
        if (isLanternActive)
        {
            DepleteEnergy();
        }
    }

    void HandleLanternClick()
    {
        float currentTime = Time.time;

        
        if (currentTime - lastClickTime <= doubleClickTime)
        {
            
            RefillLantern(); //if lantern is double clicked and there is oil in the inventory, refill the lantern 
        }
        else
        {
            ToggleLantern(); 
        }

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
            if (currentEnergy > 0) 
            {
                lantern.SetActive(true);
                isLanternActive = true; 
            }
            else
            {
                Debug.Log("Not enough energy to turn on the lantern!");
            }
        }
    }

    void DepleteEnergy()
    {
        currentEnergy -= depletionRate * Time.deltaTime;
        if (currentEnergy <= 0)
        {
            currentEnergy = 0; 
            lantern.SetActive(false); 
            isLanternActive = false; 
            Debug.Log("Lantern is out of energy!");
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

    public void RefillLantern()
    {
        if (inventory.ContainsItem(oilItem, 1)) 
        {
            currentEnergy += refillAmount; 
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy); 
            inventory.RemoveItem(oilItem); 
            Debug.Log("Lantern refilled! Current energy: " + currentEnergy);
        }
        else
        {
            Debug.Log("Not enough oil to refill the lantern!");
        }
    }
}
