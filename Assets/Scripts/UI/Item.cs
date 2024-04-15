using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Item", menuName ="Item/baseItem")]
public class Item : ScriptableObject
{
    new public string name = "Default Item";
    public Sprite icon = null;
    public string itemDescription = "Used for crafting";

    // Event triggered when the item is used
    public delegate void OnItemUsed();
    public event OnItemUsed ItemUsed;

    // Method to use the item
    public virtual void Use()
    {
        Debug.Log("Using " + name);
        // Trigger the event when the item is used
        ItemUsed?.Invoke();
    }

    // Method to check if the item can be used
    public virtual bool CanUse()
    {
        return true; // By default, all items can be used
    }

    // Method to get the item description
    public virtual string GetItemDescription()
    {
        return itemDescription;
    } 
}
