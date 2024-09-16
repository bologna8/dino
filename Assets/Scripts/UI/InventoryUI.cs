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
        // PlayerControls.onRecipeUnlocked += UpdateCraftingUI;

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
        Time.timeScale = 0f;
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
        Time.timeScale = 1f;
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
        if (craftingSlotPrefab != null && craftingItemTransform != null)
        {
            GameObject go = Instantiate(craftingSlotPrefab, craftingItemTransform);
            ItemSlot slot = go.GetComponent<ItemSlot>();
            if (slot != null)
            {
                slot.AddItem(recipe);
            }
        }
    }

    private void SetUpCraftingRecipes()
    {
        List<Item> craftingRecipes = GameManager.instance.craftingRecipes;

        foreach (Item recipe in craftingRecipes)
        {
            if (craftingSlotPrefab != null && craftingItemTransform != null)
            {
                GameObject go = Instantiate(craftingSlotPrefab, craftingItemTransform);
                ItemSlot slot = go.GetComponent<ItemSlot>();
                if (slot != null)
                {
                    slot.AddItem(recipe);
                }
            }
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