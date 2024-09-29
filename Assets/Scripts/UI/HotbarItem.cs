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


    public void AddToHotbar()
    {
        if (Hotbar.instance != null)
        {

        }
    }


    public void ReplaceInHotbar(int index)
    {
        if (Hotbar.instance != null)
        {
            
        }
    }

}
