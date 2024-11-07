using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region singleton
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    public List<Item> itemList = new List<Item>();
    public List<Item> craftingRecipes = new List<Item>();

    public List<Item> recipePrefabs = new List<Item>();

    
    public Transform canvas;
    public GameObject itemInfoPrefab;
    private GameObject currentItemInfo = null;

    public float moveX = 0f;
    public float moveY = 0f;

    public Health playerHealth;
    public HealthUI playerHealthUI;

    public void OnStatItemUse(StatItemType itemType, int amount)
    {
      if(itemType == StatItemType.FoodItem)
      {
        playerHealth.GainHealth(amount);
        playerHealthUI.UpdateHealthUI();
        Debug.Log("Consuming " + itemType + " Add amount: " + amount);
      }
    }

    public void Start()
    {
      foreach(Item recipePrefab in recipePrefabs)
      {
        if(recipePrefab != null){
          craftingRecipes.Add(recipePrefab);
        }
      }
    }

  public void DisplayItemInfo(string itemName, string itemDescription, Vector2 buttonPos)
{
   // if (currentItemInfo == null)
  //  {
    //    buttonPos.x -= 200;
   //     buttonPos.y += 50;
//
  //     currentItemInfo = Instantiate(itemInfoPrefab, buttonPos, Quaternion.identity, canvas);
//currentItemInfo.GetComponent<ItemInfo>().SetUp(itemName, itemDescription);
 //   }
//    else
 ////   {
     //   currentItemInfo.GetComponent<ItemInfo>().SetUp(itemName, itemDescription);
     //   currentItemInfo.transform.position = buttonPos;
   // }
}

    public void DestroyItemInfo()
    {
        if(currentItemInfo != null)
        {
            Destroy(currentItemInfo.gameObject);
        }
    }

}