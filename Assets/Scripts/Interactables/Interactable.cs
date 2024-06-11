using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private SpriteRenderer mySprite;
    //[HideInInspector] public bool playerTouching = false;
    [HideInInspector] public PlayerControls player;

    // Start is called before the first frame update
    void Start()
    {
        mySprite = GetComponentInChildren<SpriteRenderer>();
        mySprite.color = new Color(0.9f,0.9f,0.9f,1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var checkPlayer = other.gameObject.GetComponent<PlayerControls>();
        if (checkPlayer)
        {
            mySprite.color = Color.white;
            //playerTouching = true;
            player = checkPlayer;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var checkPlayer = other.gameObject.GetComponent<PlayerControls>();
        if (checkPlayer) 
        {
            mySprite.color =  new Color(0.9f,0.9f,0.9f,1f);
            //playerTouching = false;
            player = null;
        }
    }
}
