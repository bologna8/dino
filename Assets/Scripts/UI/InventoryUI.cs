using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

//Referencing the UI for Inventory System 
public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    public bool inventoryOpen = false;
    public GameObject inventoryParent;
    //public GameObject craftingTab;
    public GameObject book;

    private List<ItemSlot> itemSlotList = new List<ItemSlot>();

    public GameObject inventorySlotPrefab;
    //public GameObject craftingSlotPrefab;

    public Transform inventoryItemTransform;
    //public Transform craftingItemTransform;
    public Transform[] hotbarTransforms = new Transform[4];

    // Highlight 
    public int selectedIndex = 0;

    private PlayerControls playerControls;
    private EventSystem eventSystem;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

private void Start()
{

    inventoryOpen = false;
    if (Inventory.instance != null)
    {
        Inventory.instance.onItemChange += UpdateInventoryUI;
    }

    UpdateInventoryUI();
    //SetUpCraftingRecipes();

    playerControls = FindObjectOfType<PlayerControls>();

    if (Gamepad.all.Count > 0 && itemSlotList.Count > 0)
    {
        EventSystem.current.SetSelectedGameObject(itemSlotList[0].gameObject);
    }
}


    public void OpenInventory(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                if(BookController.Instance!=null && BookController.Instance.isJournalOpen)
                {
                    BookController.Instance.CloseJournal();
                }
                OpenInventory();
            }
        }
    }

public void OpenInventory()
{
    inventoryOpen = true;
    inventoryParent.SetActive(true);

    if (playerControls != null)
    {
        playerControls.enabled = false;
    }

    if (Gamepad.all.Count > 0 && itemSlotList.Count > 0)
    {
        EventSystem.current.SetSelectedGameObject(itemSlotList[0].gameObject);
    }
}


    public void CloseInventory()
    {
        inventoryOpen = false;
        inventoryParent.SetActive(false);
        if (book != null)
        {
            book.SetActive(false);
        }

        if (playerControls != null)
        {
            playerControls.enabled = true;
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.DestroyItemInfo();
        }

        if (BookController.Instance && BookController.Instance.isJournalOpen)
        {
            BookController.Instance.CloseJournal();
        }
    }

/*
    public void UpdateCraftingUI(CraftingRecipe recipe)
    {
        if (recipe == null) return;

        GameObject recipeSlot = Instantiate(craftingSlotPrefab, craftingItemTransform);
        ItemSlot recipeItemSlot = recipeSlot.GetComponent<ItemSlot>();
        if (recipeItemSlot != null)
        {
            recipeItemSlot.AddItem(recipe);
        }

        foreach (var ingredient in recipe.ingredients)
        {
            for (int i = 0; i < ingredient.amount; i++)
            {
                GameObject ingredientSlot = Instantiate(craftingSlotPrefab, craftingItemTransform);
                ItemSlot ingredientItemSlot = ingredientSlot.GetComponent<ItemSlot>();
                if (ingredientItemSlot != null)
                {
                    ingredientItemSlot.AddItem(ingredient.item);
                }
            }
        }
    }

    private void SetUpCraftingRecipes()
    {
        List<Item> craftingRecipes = GameManager.instance.craftingRecipes;

        foreach (CraftingRecipe recipe in craftingRecipes)
        {
            UpdateCraftingUI(recipe);
        }
    }
*/
public void UpdateInventoryUI()
{
    if (inventoryItemTransform == null) { return; }

    itemSlotList.Clear();

    foreach (Transform child in inventoryItemTransform)
    {
        Destroy(child.gameObject);
    }

    foreach (Transform hotbarTransform in hotbarTransforms)
    {
        if (hotbarTransform != null)
        {
            foreach (Transform child in hotbarTransform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    foreach (var item in Inventory.instance.inventoryItemList)
    {
        //if (item is HotbarItem hotbarItem && hotbarItem.HotBarIndex >= 0 && hotbarItem.HotBarIndex < hotbarTransforms.Length)
        if (item.coreItemPosition > 0)
        {
            AddItemSlot(item, hotbarTransforms[item.coreItemPosition -1]);
        }
        else
        {
            AddItemSlot(item, inventoryItemTransform);
        }
    }
}


    private void AddItemSlot(Item item, Transform parentTransform)
    {
        if (inventorySlotPrefab != null && parentTransform != null)
        {
            GameObject go = Instantiate(inventorySlotPrefab, parentTransform);
            ItemSlot newSlot = go.GetComponent<ItemSlot>();
            if (newSlot != null)
            {
                newSlot.AddItem(item);
                itemSlotList.Add(newSlot);
            }
        }
    }

    private void AddItemSlot(Item item)
    {
        AddItemSlot(item, inventoryItemTransform);
    }
}
