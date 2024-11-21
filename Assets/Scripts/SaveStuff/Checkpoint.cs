using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    //[HideInInspector] public int Number;
    public bool active = false;
    public GameObject activeEffect;
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

        }
        
    }



}
