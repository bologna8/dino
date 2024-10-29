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
    public int HotBarIndex;

    public override void Use()
    {
        base.Use();
        AddToHotbar();
    }

    public void AddToHotbar()
    {
        if (Hotbar.instance == null) { return; }

        Hotbar.instance.equippedItems[HotBarIndex] = this;
        Hotbar.instance.wheelSlices[HotBarIndex].changeSprite(icon, 1);

    }



}
