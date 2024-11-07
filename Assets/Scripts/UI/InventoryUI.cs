using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    public bool inventoryOpen = true;
    //public bool InventoryOpen => inventoryOpen;

    public GameObject inventoryParent;
    public GameObject craftingTab;
    public GameObject itemDescriptionPanel;
    public GameObject book;

    private List<ItemSlot> itemSlotList = new List<ItemSlot>();
    public List<ItemSlot> journalSlotList = new List<ItemSlot>();

    public GameObject inventorySlotPrefab;
    public GameObject craftingSlotPrefab;

    public Transform inventoryItemTransform;
    public Transform craftingItemTransform;

    // Highlight 
    public int selectedIndex = 0;

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
            book.SetActive(true);
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
        UIStateTracker.Instance.SetActiveScreen(UIStateTracker.UIScreen.Inventory);
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

        if (BookController.Instance) { if (BookController.Instance.isJournalOpen) { BookController.Instance.CloseJournal(); }}
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
            journalSlotList.Add(recipeItemSlot); // Ensure you're adding to the correct list
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
                    journalSlotList.Add(ingredientItemSlot); // Ensure you're adding to the correct list
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

    public void UpdateInventoryUI()
    {
        int currentItemCount = Inventory.instance.inventoryItemList.Count;

        // Adjust the number of slots as needed
        while (itemSlotList.Count < currentItemCount)
        {
            AddItemSlot();
        }

        // Update existing slots and remove excess ones
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
                i--; // Adjust index after removal
            }
        }
    }

    private void AddItemSlot()
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

    private void ChangeCursorState(bool lockCursor)
    {
        if (lockCursor)
        {
            //Cursor.lockState = CursorLockMode.Locked;
#if !UNITY_EDITOR
            Cursor.visible = false;
#endif
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
/*
    public void HighlightItem(int index)
    {
        // Unhighlight all slots 
        foreach (ItemSlot slot in itemSlotList)
        {
            slot.SetHighlight(false);
        }

        // Highlight the selected slot
        if (index >= 0 && index < itemSlotList.Count)
        {
            itemSlotList[index].SetHighlight(true);
        }
    }

    public void NavigateInventory(InputAction.CallbackContext context)
    {
        if (context.performed && inventoryOpen)
        {
            Vector2 navigationInput = context.ReadValue<Vector2>();
            if (navigationInput.y > 0)  // Up
            {
                selectedIndex = Mathf.Max(0, selectedIndex - 1);
            }
            else if (navigationInput.y < 0)  // Down
            {
                selectedIndex = Mathf.Min(itemSlotList.Count - 1, selectedIndex + 1);
            }
            else if (navigationInput.x > 0)
            {
                selectedIndex = Mathf.Min(itemSlotList.Count-1, selectedIndex +1);
            }
            else if(navigationInput.x <0)
            {
                selectedIndex = Mathf.Max(0,selectedIndex -1);
            }

            HighlightItem(selectedIndex);
        }
    }
}
*/
}