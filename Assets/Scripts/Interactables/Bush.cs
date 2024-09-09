using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : Interactable
{
    public static int bushesTouched;

    public override void Interacted(Core interCore)
    {
        if (!interCore.hidden) { interCore.hidden = true; }
        else { interCore.hidden = false; }

    }

    public override void OnTriggerStay2D(Collider2D other)
    {
        if (!player)
        {
            if (EnterCheck(other))
            {
                player.myCore.hidingSpots ++;
            }
        }
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        if (player) 
        { 
            if (ExitCheck(other)) 
            {
                player.myCore.hidingSpots --;

                if (player.myCore.hidingSpots <= 0) 
                { player.myCore.hidden = false; }

                player = null;
            } 
        }
        
    }

    public  override void OnDisable()
    {
        if (player) { player.interactablesTouched.Remove(this); player.myCore.hidingSpots --; }
        valid = false;
    }

}
