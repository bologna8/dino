using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "StatItem", menuName = "Item/StatItem")]
public class StatItem : Item
{
    public StatItemType itemType;
    public int amount;

    public override void Use()
    {
        base.Use();
        if (itemType == StatItemType.FoodItem && GameManager.instance.playerHealth.currentHP < GameManager.instance.playerHealth.maxHP) 
        {
            GameManager.instance.OnStatItemUse(itemType, amount);
            Inventory.instance.RemoveItem(this);
        }
        else
        {
            Debug.Log("Health is full! Cant consume item");
        }
    }
}

public enum StatItemType
{
    HealthItem,
    FoodItem
}
