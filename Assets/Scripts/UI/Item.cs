using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "Item/baseItem")]
public class Item : ScriptableObject
{
    new public string name = "Default Item";  
    public Sprite icon = null;  
    public string itemDescription = "Description";  

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
        return itemDescription;
    }
}
