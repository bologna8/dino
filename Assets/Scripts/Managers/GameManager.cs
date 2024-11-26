using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public static GameManager instance;

  public Item[] AllItems;

  public Image fadeSprite; 
  private bool fading;
  public Transform CheckpointParent;
  [HideInInspector] public List<Checkpoint> AllCheckpoints;
  //[HideInInspector] public Checkpoint LatestCheckpoint;

  public GameObject player;
  [HideInInspector] public Health playerHealth;
  [HideInInspector] public Inventory playerInventory;
  public HealthUI playerHealthUI;
  public Text InteractionText;

  public List<Item> AllRecipes;
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

      if (!playerInventory) { playerInventory = player.GetComponent<Inventory>(); }
           
    }

    if (CheckpointParent) 
    {
      foreach (Transform child in CheckpointParent) 
      { 
        var check = child.GetComponent<Checkpoint>();
        if (check) { AllCheckpoints.Add(check); }
      }
      
      //PlayerPrefs.DeleteAll();
      Load();
      StartCoroutine(Fade(Color.black, 1));
    }
    
  }

  void Update()
  {
    if (player)
    {
      if (!player.activeSelf) 
      {
        StartCoroutine(Fade(Color.black, 1, true));
      }
      //{ SceneManager.LoadScene(SceneManager.GetActiveScene().name); } //Reload scene if player is dead
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

  IEnumerator Fade(Color fadeColor, float fadeDuration = 1, bool fadeOut = false)
  {
    if (fading) { yield break; }

    if (!fadeSprite) 
    { 
      if (fadeOut) { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
      yield break; 
    }

    var startAlpha = 1; 
    var endAlpha = 0;
    if (fadeOut) { startAlpha = 0; endAlpha = 1; }

    fading = true;
    float elapsedTime = 0f;

    while (elapsedTime < fadeDuration)
    {
        elapsedTime += Time.deltaTime;
        var a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
        fadeSprite.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, a);  
        yield return null;
    }

    fading = false;
    if (fadeOut) { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
  }

  public void Save(Checkpoint latestCheckpoint)
  {
    for (int i = 0; i < AllCheckpoints.Count; i ++)
    {
      if (AllCheckpoints[i] == latestCheckpoint) { PlayerPrefs.SetInt("Current Checkpoint", i); }
      else { AllCheckpoints[i].active = false; }
    }


    int[] saveInventoryArray = new int[AllItems.Length];
    for (int i = 0; i < saveInventoryArray.Length; i++) { saveInventoryArray[i] = 0; }

    foreach (var item in playerInventory.inventoryItemList) 
    {
      for(int i = 0; i < AllItems.Length; i++) 
      {
        if (item == AllItems[i]) { saveInventoryArray[i]++; }
      }
    }

    string s = "";
    foreach (int i in saveInventoryArray) { s += "" +i; }

    PlayerPrefs.SetString("Saved Inventory", s);
    //Debug.Log("Saved: "+s);

  }

  public void Load()
  {
    if (!player) { return; }
    player.transform.position = AllCheckpoints[PlayerPrefs.GetInt("Current Checkpoint", 0)].transform.position;

    if (!playerInventory) { return; }
    playerInventory.inventoryItemList.Clear();

    var defaultString = "1"; //Start with shovel and lantern at least
    for (int i = 1; i < AllItems.Length; i++)
    { defaultString += "0"; }

    var inventoryString = PlayerPrefs.GetString("Saved Inventory", defaultString);
    //Debug.Log("Load: " +inventoryString);

    for(int i = 0; i < AllItems.Length; i ++)
    {
      var amount = char.GetNumericValue(inventoryString[i]);      
      for (int n = 0; n < amount; n++)
      {
        playerInventory.AddItem(AllItems[i]);
      }
      
    }

  }

  public void Reset()
  {
    PlayerPrefs.DeleteAll();
    StartCoroutine(Fade(Color.black, 1, true));
  }


}