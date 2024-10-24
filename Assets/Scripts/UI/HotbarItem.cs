using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "HotbarItem", menuName = "Item/HotbarItem")]
public class HotbarItem : Item
{
    public GameObject attackPrefab;
    public GameObject attackUp;
    public GameObject attackDown;

    public bool consumable;

    public override void Use()
    {
        base.Use();
        AddToHotbar();
    }

    public void AddToHotbar()
    {
        if (Hotbar.instance == null) { return; }

        if (consumable && Hotbar.instance.consumableEquipped)
        {
            ReplaceInHotbar(Hotbar.instance.equippedItems.Count);
            return;
        }

        Hotbar.instance.equippedItems.Add(this);
    }


    public void ReplaceInHotbar(int index)
    {
        if (Hotbar.instance == null) { return; }
        
        Hotbar.instance.equippedItems[index] = this;
    }

}
