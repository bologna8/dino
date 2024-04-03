using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region singleton
    
    public static Inventory instance; 
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this; 
        }
    }

    public delegate void OnItemChange();
    public OnItemChange onItemChange = delegate{};

    public List<Item> inventoryItemList = new List<Item>();
    public List<Item> hotbarItemList = new List<Item>();
    public HotbarController hotbarController;

    public void SwitchHotbarInventory(Item item)
    {
        foreach (Item i in inventoryItemList)
        {
            if (i == item)
            {
                if (hotbarItemList.Count >= hotbarController.HotbarSlotSize)
                {
                    Debug.Log("No more slots available");
                }
                else
                {
                    hotbarItemList.Add(item);
                    inventoryItemList.Remove(item);
                    onItemChange.Invoke();
                }
                return;
            }
        }

        // hotbar to Inventory
        foreach (Item i in hotbarItemList)
        {
            if (i == item)
            {
                hotbarItemList.Remove(item);
                inventoryItemList.Add(item);
                onItemChange.Invoke();
                return;
            }
        }
    }

    public void AddItem(Item item)
    {
        inventoryItemList.Add(item);
        onItemChange.Invoke();
    }

    public void RemoveItem(Item item)
    {
        if (inventoryItemList.Contains(item))
        {
            inventoryItemList.Remove(item);
        }
        else if (hotbarItemList.Contains(item))
        {
            hotbarItemList.Remove(item);
        }
    }

    #endregion 
}
