using UnityEngine;
using UnityEngine.InputSystem;
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
    public bool isLanternActive = false;
    private bool isLanternInInventory = false;
    private Inventory inventory;

    public Item lanternItem; 

    private float lastClickTime = 0f; 
    public float doubleClickTime = 0.3f;


    //Fields for controling the appearance of the lantern's light.
    [SerializeField]
    private Light2D lanternLight;

    [Header("The color of light the lantern has when at\nfull energy and and nearly empty energy.")]
    [SerializeField]
    private Color maxLightColor = Color.white;
    [SerializeField]
    private Color minLightColor = Color.black;
    private Color currentLightColor;

    [Header("Parameters for the size of the light.")]
    [SerializeField]
    private float maxLanternRadius = 10;
    [SerializeField]
    private float minLanternRadius = 3;
    private float currentLanternRadius;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        lantern.SetActive(true); //switch to false after VS
        isLanternActive=true;
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
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleLanternClick();
            }
        }

        
        if (isLanternActive)
        {
            DepleteEnergy();
        }


        UpdateLanternLight();
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

    void UpdateLanternLight()
    {
        float currentEnergyRatio = currentEnergy / maxEnergy;

        float radiusMod = (maxLanternRadius - minLanternRadius) * currentEnergyRatio;
        currentLanternRadius = minLanternRadius + radiusMod;
        lanternLight.pointLightOuterRadius = currentLanternRadius;


        currentLightColor = maxLightColor * currentEnergyRatio + minLightColor * (1 - currentEnergyRatio);
        lanternLight.color = currentLightColor;

        if (currentEnergy*2 < Random.Range(0, Mathf.Pow(Random.value, 4) * maxEnergy / currentEnergy))
        {
            lanternLight.color = Color.black;
        }
    }
}
