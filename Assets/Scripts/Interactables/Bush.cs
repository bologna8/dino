using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    [HideInInspector] public bool playerTouching = false;
    private PlayerControls player;

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
        var findPlayer = other.gameObject.GetComponent<PlayerControls>();
        if (findPlayer && player == null)
        { 
            player = findPlayer;
            player.bushesTouched.Add(this);
            playerTouching = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var findPlayer = other.gameObject.GetComponent<PlayerControls>();
        if (findPlayer && player) 
        { 
            player.bushesTouched.Remove(this);
            player = null;
            playerTouching = false;
        }
    }

    void OnDestroy()
    {
        if (player) { player.bushesTouched.Remove(this); }
    }


}
