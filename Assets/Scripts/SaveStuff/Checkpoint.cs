using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour, IInteractable
{
    //[HideInInspector] public int Number;
    public bool active = false;
    public GameObject activeEffect;

    private Core touchingCore;
    //public static GameObject spawnPlayer;
    //public float respawnOffset = 1;

    // Start is called before the first frame update
    void Start()
    {
        //Save.allPoints.Add(this);
        //Number = Save.allPoints.Count -1;

        //if (!spawnPlayer && playerPrefab) { spawnPlayer = playerPrefab; }
        //if (GameManager.instance) { GameManager.instance.AllCheckpoints.Add(this); }
    }

    // Update is called once per frame
    void Update()
    {
        if (activeEffect) { activeEffect.SetActive(active); }

        //currentActive = allPoints[PlayerPrefs.GetInt("CurrentCheckpoint")];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerControls>();
        if (player)
        { 
            if (GameManager.instance) { GameManager.instance.Save(this); }
            active = true;
            if (!touchingCore) 
            { 
                touchingCore = player.transform.GetComponent<Core>();
                if (touchingCore) { touchingCore.interactables.Add(this); }
            }
            

        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (touchingCore) 
        {
            if (other.transform == touchingCore.transform) 
            { 
                touchingCore.interactables.Remove(this);
                touchingCore = null; 
            }
        }
    }

    public void Interact(GameObject interacter)
    {
        if (GameManager.instance) { GameManager.instance.Save(this); GameManager.instance.Load(); }
    }



}
