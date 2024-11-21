using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton

    public static Inventory instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    public delegate void OnItemChange();
    public OnItemChange onItemChange = delegate { };

    public List<Item> inventoryItemList = new List<Item>();


public void AddItem(Item item)
{
   if (item.isStackable)
{
    Item existingItem = inventoryItemList.Find(i => i.itemName == item.itemName);

    if (existingItem != null)
    {
        if (existingItem.stackCount < existingItem.maxStack)
        {
            existingItem.stackCount++;
        }
        else
        {
            Debug.Log($"Stack is full for item: {item.itemName}. Cannot add more.");
        }
    }
    else
    {
        Item newItem = Instantiate(item); // Create a new instance 
        newItem.stackCount = 1;
        newItem.name = item.itemName; 
        inventoryItemList.Add(newItem);
    }
}
    else
    {
        // Non-stackable item, always add a new entry
        inventoryItemList.Add(item);
    }
 
    onItemChange.Invoke();

    if (item is HotbarItem hotbarItem)
    {
        hotbarItem.AddToHotbar();
    }

    PopupManager popupManager = FindObjectOfType<PopupManager>();
    if (popupManager != null)
    {
        popupManager.ShowPopup(item.itemName);
    }
}

    public void RemoveItem(Item item)
    {
        if (inventoryItemList.Contains(item))
        {
            if (item.isStackable && item.stackCount > 1)
            {
                item.stackCount--;
            }
            else
            {
                inventoryItemList.Remove(item);
            }
        }

        onItemChange.Invoke();
    }

    public void RemoveItems(Item item, int amount)
    {
        if (!inventoryItemList.Contains(item))
        {
            Debug.LogWarning("Item not found in inventory.");
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            if (item.isStackable && item.stackCount > 1)
            {
                item.stackCount--;
            }
            else
            {
                inventoryItemList.Remove(item);
                break;
            }
        }

        onItemChange.Invoke();
    }

    public bool ContainsItem(Item item, int amount)
    {
        if (!item.isStackable)
        {
            return inventoryItemList.FindAll(i => i.itemName == item.itemName).Count >= amount;
        }

        Item existingItem = inventoryItemList.Find(i => i.itemName == item.itemName);
        return existingItem != null && existingItem.stackCount >= amount;
    }
}