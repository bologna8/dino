using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public Image icon;
    private Item item;

    public delegate void OnItemUsed();
    public event OnItemUsed ItemUsed;

    private void Start()
    {
        if (item != null)
        {
            item.ItemUsed += OnItemUsedEventHandler;
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

    // Mouse hover 
    public void OnPointerEnter(PointerEventData eventData)
    {
        DisplayItemInfo();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideItemInfo();
    }

    // Controller navigation 
    public void OnSelect(BaseEventData eventData)
    {
        DisplayItemInfo();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        HideItemInfo();
    }

    private void DisplayItemInfo()
    {
        if (item != null)
        {
            UIManager.instance.DisplayItemInfo(item.name, item.GetItemDescription(), transform.position);
        }
    }

    private void HideItemInfo()
    {
        UIManager.instance.DestroyItemInfo();
    }

    private void OnItemUsedEventHandler()
    {
        // Handle item used event if needed
    }

    public void SetIconColor(Color color)
    {
        if (icon != null)
        {
            icon.color = color;
        }
    }
}
