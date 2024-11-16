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

    //public List<Item> itemList = new List<Item>();
    public List<Item> craftingRecipes = new List<Item>();

    //public List<Item> recipePrefabs = new List<Item>();

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
      /*
      foreach(Item recipePrefab in recipePrefabs)
      {
        if(recipePrefab != null){
          craftingRecipes.Add(recipePrefab);
        }
      }
      */
    }
}