using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, DataPersistence
{
  public static GameManager instance;

  public List<Item> AllRecipes;
  public List<Item> AllItems;

  [HideInInspector] public List<Checkpoint> AllCheckpoints;
  [HideInInspector] public Vector3 LatestCheckpointPosition;

  public GameObject player;
  [HideInInspector] public Health playerHealth;
  public HealthUI playerHealthUI;
  public Text InteractionText;

  public List<Item> craftingRecipes = new List<Item>();

  public void Awake()
  {
    if (instance == null) { instance = this; }

    if (player)
    {
      if (!playerHealth) 
      { 
        playerHealth = player.GetComponentInChildren<Health>();
        if (playerHealth && playerHealthUI) { playerHealthUI.tracking = playerHealth; }
      }
      
    }
  }

  void Update()
  {
    if (player)
    {
      if (!player.activeSelf) { SceneManager.LoadScene(SceneManager.GetActiveScene().name); } //Reload scene if player is dead
    }
  }

  public void OnStatItemUse(StatItemType itemType, int amount)
  {
    if(itemType == StatItemType.FoodItem)
    {
      playerHealth.GainHealth(amount);
      //playerHealthUI.UpdateHealthUI();
      Debug.Log("Consuming " + itemType + " Add amount: " + amount);
    }
  }

  public void LoadData(GameData data)
  {
    if(player) { player.transform.position = LatestCheckpointPosition; }
  }

  public void SaveData(ref GameData data)
  {
    foreach (var point in AllCheckpoints) { point.active = false; }
    data.savedPosition = LatestCheckpointPosition;
  }

}