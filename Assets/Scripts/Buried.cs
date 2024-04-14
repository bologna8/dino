using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buried : MonoBehaviour
{
    public GameObject digEffect;
    public GameObject[] BuriedTreasures;
    public float timeToDig = 1f;
    [HideInInspector] public bool playerTouching = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerControls>();
        if (player)
        { 
            playerTouching = true;
            if (player.interacting) 
            { 
                player.Dig(timeToDig);
                StartCoroutine(delayedDig(timeToDig));
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerControls>();
        if (player && playerTouching) { playerTouching = false; }
    }

    IEnumerator delayedDig(float digTime)
    {
        yield return new WaitForSeconds(digTime);
        if (playerTouching) 
        {
            var r = Random.Range(0, BuriedTreasures.Length);
            var treasure = BuriedTreasures[r];
            if (treasure) { Instantiate(treasure, transform.position, transform.rotation); }
            if (digEffect) { Instantiate(digEffect, transform.position, transform.rotation); }
        }
    }
}
