using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public GameObject respawnThis;
    public GameObject respawningImage;

    public float respawnTime;
    private float currentRespawnTime;

    public Vector3 respawnOffset;
    private Spawned mySpawn;

    [Tooltip("If the nest is visible on screen, won't respawn")] public bool resetOnScreen;

    public Item requiredItem;
    [Tooltip("If player has this much of an item or less, won't respawn")] public int amountOfItem;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (!mySpawn) { mySpawn = GetComponent<Spawned>(); }
        if (mySpawn) { respawnThis = mySpawn.source.gameObject; }

        currentRespawnTime = respawnTime;

    }

    // Update is called once per frame
    void Update()
    {
        if (respawnThis)
        {
            if (!respawnThis.activeSelf)
            {
                if (respawningImage) { respawningImage.SetActive(true); }
                currentRespawnTime -= Time.deltaTime;

                if (resetOnScreen)
                {
                    var view = Camera.main.WorldToViewportPoint(transform.position);
                    if (view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1) { } 
                    else { currentRespawnTime = respawnTime; } 
                }

                if (requiredItem && Inventory.instance)
                {
                    if (Inventory.instance.ContainsItem(requiredItem, amountOfItem +1)) 
                    { currentRespawnTime = respawnTime; }
                }

                if (currentRespawnTime <= 0) 
                { 
                    currentRespawnTime = respawnTime;
                    respawnThis.SetActive(true);
                    respawnThis.transform.position = transform.position + respawnOffset;
                }

            }
            else if (respawningImage) { respawningImage.SetActive(false); }
        }
        
    }
}
