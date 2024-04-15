using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon;
    private Item item;

    public delegate void OnItemUsed();
    public event OnItemUsed ItemUsed;

    private void Start()
    {
        if (item != null)
        {
            // Subscribe to the ItemUsed event of the item
            item.ItemUsed += OnItemUsedEventHandler;
        }
    }

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = newItem.icon;

        // Subscribe to the ItemUsed event of the item
        item.ItemUsed += OnItemUsedEventHandler;
    }

    public void ClearSlot()
    {
        // Unsubscribe from the ItemUsed event before clearing the slot
        if (item != null)
        {
            item.ItemUsed -= OnItemUsedEventHandler;
        }

        item = null;
        icon.sprite = null;
    }

    public void UseItem()
    {
        if (item == null) return;

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            Debug.Log("Trying to switch");
            Inventory.instance.SwitchHotbarInventory(item);
        }
        else
        {
            item.Use();
            ItemUsed?.Invoke(); // Trigger the ItemUsed event of the item slot
        }
    }   

    public void DestroySlot()
    {
        // Unsubscribe from the ItemUsed event before destroying the slot
        if (item != null)
        {
            item.ItemUsed -= OnItemUsedEventHandler;
        }

        Destroy(gameObject);
    }

    public void OnRemoveButtonClicked()
    {
        if(item != null)
        {
            Inventory.instance.RemoveItem(item);
        }
    }

    public void OnCursorEnter()
    {
        if (item == null) return;

        //display item info
        GameManager.instance.DisplayItemInfo(item.name, item.GetItemDescription(), transform.position);
    }

    public void OnCursorExit()
    {
        if (item == null) return;

        GameManager.instance.DestroyItemInfo();
    }

    private void OnItemUsedEventHandler()
    {
        // Empty method, used only to handle event subscription
    }
}
