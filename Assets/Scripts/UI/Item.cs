using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "Item/baseItem")]
public class Item : ScriptableObject
{
    [Header("Basic Properties")]
    public string itemName = "Default Item";  
    public Sprite icon = null;  
    public string itemDescription = "Description";  
    public int coreItemPosition = 0;

    [Header("Stacking")]
    public bool isStackable = true; // Determines if the item can stack
    public int stackCount = 1;      // Tracks the number of items in the stack
    public int maxStack = 9;        // Maximum stack size for stackable items

    public delegate void OnItemUsed();
    public event OnItemUsed ItemUsed;

    public virtual void Use()
    {
        ItemUsed?.Invoke();
    }

    public virtual bool CanUse()
    {
        return true; 
    }

    public virtual string GetItemDescription()
    {
        if (isStackable)
        {
            return $"{itemDescription} (Stack: {stackCount}/{maxStack})";
        }
        else
        {
            return itemDescription;
        }
    }
}
