using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private SpriteRenderer mySprite;
    [HideInInspector] public bool playerTouching = false;
    [HideInInspector] public PlayerControls player;

    [HideInInspector] public bool valid = true; //Able to be interacted with currently

    // Start is called before the first frame update
    void OnEnable()
    {
        if (!mySprite) { mySprite = GetComponentInChildren<SpriteRenderer>(); }
        if (mySprite) { mySprite.color = new Color(0.9f,0.9f,0.9f,1f); }
        valid = true;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public virtual void OnTriggerStay2D(Collider2D other)
    {
        if (!player) { EnterCheck(other); }
        
    }

    public bool EnterCheck(Collider2D other)
    {
        var checkPlayer = other.gameObject.GetComponent<PlayerControls>();
        if (checkPlayer)
        {
            if (mySprite) { mySprite.color = Color.white; }
            player = checkPlayer;
            player.interactablesTouched.Add(this);
            return true;
        }

        return false;
    }



    public virtual void OnTriggerExit2D(Collider2D other)
    {
        if (player) { if (ExitCheck(other)) { player = null; } } 
        //remember to unasaign player like this if changing trigger exit to ass more funtionality such as bushes
        
    }

    public bool ExitCheck(Collider2D other)
    {
        var checkPlayer = other.gameObject.GetComponent<PlayerControls>();
        if (checkPlayer) 
        {
            if (mySprite) { mySprite.color =  new Color(0.9f,0.9f,0.9f,1f); }
            player.interactablesTouched.Remove(this);
            return true;
        }

        return false;
    }

    public virtual void OnDisable()
    {
        if (player) { player.interactablesTouched.Remove(this); }
        valid = false;
    }


    public virtual void Interacted(Core interCore)
    {
        Debug.Log("Interacted with :" + interCore.gameObject.name);
    }

}
