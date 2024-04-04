using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region singleton 
    public static GameManager instance; 
    private void Awake()
    {
        if(instance == null)
        {
            instance = this; 
        }
    }
    #endregion 
    public List<Item> itemList = new List<Item>();
    public List<Item> craftingRecipes = new List<Item>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Item newItem = itemList[Random.Range(0, itemList.Count)];
            Inventory.instance.AddItem(Instantiate(newItem));
        }
    }
    public void OnStatItemUse(StatItemType itemType, int amount)
    {
        Debug.Log("Eating "+ itemType + " Add amount: "+ amount);
    }
}