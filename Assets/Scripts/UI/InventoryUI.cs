using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    private bool inventoryOpen = false;
    public bool InventoryOpen => inventoryOpen;

    public GameObject inventoryParent;
    public GameObject inventoryTab;
    public GameObject craftingTab;
    public GameObject itemDescriptionPanel;
    public GameObject book;

    private List<ItemSlot> itemSlotList = new List<ItemSlot>();

    public GameObject inventorySlotPrefab;
    public GameObject craftingSlotPrefab;

    public Transform inventoryItemTransform;
    public Transform craftingItemTransform;

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
        if (Inventory.instance != null)
        {
            Inventory.instance.onItemChange += UpdateInventoryUI;
        }

        UpdateInventoryUI();
        SetUpCraftingRecipes();

        foreach (ItemSlot slot in itemSlotList)
        {
            slot.ItemUsed += DeactivateItemDescriptionPanel;
        }

        if (book != null)
        {
            book.SetActive(false);
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
                OpenInventory();
            }
        }
    }

    private void OpenInventory()
    {
        ChangeCursorState(false);
        inventoryOpen = true;
        inventoryParent.SetActive(true);

        if (book != null)
        {
            book.SetActive(true);
        }
    }

    private void CloseInventory()
    {
        ChangeCursorState(true);
        inventoryOpen = false;
        inventoryParent.SetActive(false);

        if (book != null)
        {
            book.SetActive(false);
        }

        DeactivateItemDescriptionPanel();
    }

    private void DeactivateItemDescriptionPanel()
    {
        if (itemDescriptionPanel != null)
        {
            itemDescriptionPanel.SetActive(false);
        }
    }

    public void UpdateCraftingUI(CraftingRecipe recipe)
    {
        if (recipe == null) return;


        GameObject recipeSlot = Instantiate(craftingSlotPrefab, craftingItemTransform);
        ItemSlot recipeItemSlot = recipeSlot.GetComponent<ItemSlot>();
        if (recipeItemSlot != null)
        {
            recipeItemSlot.AddItem(recipe);
            recipeItemSlot.icon.color = Color.white; 
        }

        foreach (var ingredient in recipe.ingredients)
        {
            GameObject ingredientSlot = Instantiate(craftingSlotPrefab, craftingItemTransform);
            ItemSlot ingredientItemSlot = ingredientSlot.GetComponent<ItemSlot>();
            if (ingredientItemSlot != null)
            {
                ingredientItemSlot.AddItem(ingredient.item);
                
                if (Inventory.instance.ContainsItem(ingredient.item, ingredient.amount))
                {
                    ingredientItemSlot.icon.color = Color.white; 
                }
                else
                {
                    ingredientItemSlot.icon.color = Color.gray; 
                }
            }
        }
    }

    private void ClearCraftingSlots()
    {
        foreach (Transform child in craftingItemTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetUpCraftingRecipes()
    {
        List<Item> craftingRecipes = GameManager.instance.craftingRecipes;

        foreach (Item recipe in craftingRecipes)
        {
            UpdateCraftingUI((CraftingRecipe)recipe);
        }
    }

    public void UpdateInventoryUI()
    {
        int currentItemCount = Inventory.instance.inventoryItemList.Count;

        if (currentItemCount > itemSlotList.Count)
        {
            AddItemSlots(currentItemCount);
        }

        for (int i = 0; i < itemSlotList.Count; ++i)
        {
            if (i < currentItemCount)
            {
                itemSlotList[i].AddItem(Inventory.instance.inventoryItemList[i]);
            }
            else
            {
                itemSlotList[i].DestroySlot();
                itemSlotList.RemoveAt(i);
            }
        }
    }

    private void AddItemSlots(int currentItemCount)
    {
        int amount = currentItemCount - itemSlotList.Count;

        for (int i = 0; i < amount; ++i)
        {
            if (inventorySlotPrefab != null && inventoryItemTransform != null)
            {
                GameObject go = Instantiate(inventorySlotPrefab, inventoryItemTransform);
                ItemSlot newSlot = go.GetComponent<ItemSlot>();
                if (newSlot != null)
                {
                    itemSlotList.Add(newSlot);
                }
            }
        }
    }

    private void ChangeCursorState(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
