using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon;
    private Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = newItem.icon;
    }

    public void ClearSlot()
    {
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
        }
    }

    public void DestroySlot()
    {
        Destroy(gameObject);
    }

    public void OnCursorEnter()
    {
        if (item == null) return;

    }

    public void OnCursorExit()
    {
        if (item == null) return;

    }
}