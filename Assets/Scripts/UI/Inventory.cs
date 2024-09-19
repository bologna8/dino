using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region singleton

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
        inventoryItemList.Add(item);
        onItemChange.Invoke();

        PopupManager popupManager = FindObjectOfType<PopupManager>();
        if (popupManager != null)
        {
            popupManager.ShowPopup(item.name); 
        }
    }

    public void RemoveItem(Item item)
    {
        if (inventoryItemList.Contains(item))
        {
            inventoryItemList.Remove(item);
        }

        onItemChange.Invoke();
    }

    public bool ContainsItem(Item item, int amount)
    {
        int itemCounter = 0;

        foreach (Item i in inventoryItemList)
        {
            if (i == item)
            {
                itemCounter++;
            }
        }

        return itemCounter >= amount;
    }

    public void RemoveItems(Item item, int amount)
    {
        if (!inventoryItemList.Contains(item))
        {
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            if (!inventoryItemList.Contains(item))
            {
                break;
            }

            RemoveItem(item);
        }
    }
}
