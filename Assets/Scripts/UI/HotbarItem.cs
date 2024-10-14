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

    public override void Use()
    {
        base.Use();
        AddToHotbar();
    }

    public void AddToHotbar()
    {
        if (Hotbar.instance != null)
        {
            Debug.Log("Item added to hotbar");
        }
    }


    public void ReplaceInHotbar(int index)
    {
        if (Hotbar.instance != null)
        {
            Debug.Log("Igtem replaced in hotbar at index" + index);
        }
    }

}
