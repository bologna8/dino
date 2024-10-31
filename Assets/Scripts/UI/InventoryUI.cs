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

    //Highlight 
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

        Time.timeScale = 0;
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
        Time.timeScale = 0;

        if (book != null)
        {
            book.SetActive(true);
        }

        //Set Inventory as the focus screen
        UIStateTracker.Instance.SetActiveScreen(UIStateTracker.UIScreen.Inventory);

    }

    private void CloseInventory()
    {
        ChangeCursorState(true);
        inventoryOpen = false;
        inventoryParent.SetActive(false);
        Time.timeScale = 1;
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

    public void HighlightItem(int index)
    {
        //unhighlight all slots 
        foreach (ItemSlot slot in itemSlotList)
        {
            slot.SetHighlight(false);
        }

        //highlight the selected slot
        if (index >= 0 && index < itemSlotList.Count)
        {
            itemSlotList[index].SetHighlight(true);
        }
    }
    public void NavigateInventory(InputAction.CallbackContext context)
    {
        if (context.performed && UIStateTracker.Instance.GetActiveScreen() == UIStateTracker.UIScreen.Inventory)
        //if (context.performed)
        {
                Vector2 navigationInput = context.ReadValue<Vector2>();

                // Only move if the inventory is open
                if (!inventoryOpen) return;


                if (navigationInput.y > 0)  // Up
                {
                    selectedIndex = Mathf.Max(0, selectedIndex - 1);
                }
                else if (navigationInput.y < 0)  // Down
                {
                    selectedIndex = Mathf.Min(itemSlotList.Count - 1, selectedIndex + 1);
                }
                if (navigationInput.x > 0)  // Right
                {
                    selectedIndex = Mathf.Min(itemSlotList.Count - 1, selectedIndex + 1);
                }
                else if (navigationInput.x < 0)  // Left
                {
                    selectedIndex = Mathf.Max(0, selectedIndex - 1);
                }

                HighlightItem(selectedIndex);
            }

        }

}