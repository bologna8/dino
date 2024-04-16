using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUi : MonoBehaviour
{
    private bool inventoryOpen = false;
    public bool InventoryOpen => inventoryOpen;

    public GameObject inventoryParent;
    public GameObject inventoryTab;
    public GameObject craftingTab;
    public GameObject itemDescriptionPanel; // Reference to the item description panel GameObject

    private List<ItemSlot> itemSlotList = new List<ItemSlot>();

    public GameObject inventorySlotPrefab;
    public GameObject craftingSlotPrefab;

    public Transform invetoryItemTransform;
    public Transform craftingItemTranform;
    
    private void Start()
    {
        PlayerControls.onRecipeUnlocked += UpdateCraftingUI;
        Inventory.instance.onItemChange += UpdateInventoryUI;
        UpdateInventoryUI();
        SetUpCraftingRecipes();
        
        // Subscribe to the ItemUsed event of all item slots
        foreach (ItemSlot slot in itemSlotList)
        {
            slot.ItemUsed += DeactivateItemDescriptionPanel;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
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
    }

    private void CloseInventory()
    {
        ChangeCursorState(true);
        inventoryOpen = false;
        Time.timeScale = 1f; 
        inventoryParent.SetActive(false);
        DeactivateItemDescriptionPanel(); // Deactivate item description panel when the inventory closes
    }

    private void DeactivateItemDescriptionPanel()
    {
        if (itemDescriptionPanel != null)
        {
            itemDescriptionPanel.SetActive(false);
        }
    }

    private void UpdateCraftingUI(CraftingRecipe recipe)
    {
        GameObject go = Instantiate(craftingSlotPrefab, craftingItemTranform);
        ItemSlot slot = go.GetComponent<ItemSlot>();
        slot.AddItem(recipe);
    }

    private void SetUpCraftingRecipes()
    {
        List<Item> craftingRecipes = GameManager.instance.craftingRecipes;

        foreach(Item recipe in craftingRecipes)
        {
            GameObject Go = Instantiate(craftingSlotPrefab, craftingItemTranform);
            ItemSlot slot = Go.GetComponent<ItemSlot>();
            slot.AddItem(recipe);
        }
    }

    private void UpdateInventoryUI()
    {
        int currentItemCount = Inventory.instance.inventoryItemList.Count;

        if(currentItemCount > itemSlotList.Count)
        {
            AddItemSlots(currentItemCount);
        }

        for(int i = 0; i < itemSlotList.Count; ++i)
        {
            if(i < currentItemCount)
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

        for(int i = 0; i < amount; ++i)
        {
            GameObject GO = Instantiate(inventorySlotPrefab, invetoryItemTransform);
            ItemSlot newSlot = GO.GetComponent<ItemSlot>();
            itemSlotList.Add(newSlot);
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
