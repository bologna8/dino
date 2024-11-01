using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon;  
    public Text itemNameText; 
    private Item item;  

    public delegate void OnItemUsed();
    public event OnItemUsed ItemUsed;  
    
    private void Start()
    {
        if (item != null)
        {
            item.ItemUsed += OnItemUsedEventHandler;
            UpdateItemName();
        }
    }

    public void AddItem(Item newItem)
    {
        if (item != null)
        {
            item.ItemUsed -= OnItemUsedEventHandler;
        }

        item = newItem;
        icon.sprite = newItem.icon; 
        icon.enabled = true;         
        
        UpdateItemName();

        item.ItemUsed += OnItemUsedEventHandler;  
    }

    public void ClearSlot()
    {
        if (item != null)
        {
            item.ItemUsed -= OnItemUsedEventHandler;
        }

        item = null;
        icon.sprite = null;
        icon.enabled = false;  

        itemNameText.text = ""; 
    }

    private void UpdateItemName()
    {
        if (item != null)
        {
            itemNameText.text = item.name; 
        }
    }

    public void UseItem()
    {
        if (item == null) return;

        item.Use();
        ItemUsed?.Invoke();  
    }

    public void DestroySlot()
    {
        if (item != null)
        {
            item.ItemUsed -= OnItemUsedEventHandler;
        }

        Destroy(gameObject);
    }

    public void OnRemoveButtonClicked()
    {
        if (item != null)
        {
            Inventory.instance.RemoveItem(item);
        }
    }

    public void OnCursorEnter()
    {
        if (item == null) return;

        // GameManager.instance.DisplayItemInfo(item.name, item.GetItemDescription(), transform.position);
    }

    public void OnCursorExit()
    {
        if (item == null) return;

        // GameManager.instance.DestroyItemInfo();
    }

    private void OnItemUsedEventHandler()
    {
        
    }

    public void SetIconColor(Color color)
    {
        if (icon != null)
        {
            icon.color = color; 
        }
    }

    public void SetHighlight(bool highlight)
    { // Adjust highlight color when navigating items
        icon.color = highlight ? Color.yellow : Color.white;
    }
}
