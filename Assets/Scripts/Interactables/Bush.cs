using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : LayerCheck, IInteractable
{

    private Core touchingCore;

    public void Interact(GameObject interacter)
    {
        if (touchingCore)
        {
            touchingCore.hidden = !touchingCore.hidden;
        }
    }

    public override void ExtraEnterOperations(Collider2D collision)
    {
        var coreCheck = collision.gameObject.GetComponent<Core>();
        if (coreCheck && !touchingCore)
        {
            touchingCore = coreCheck;
            touchingCore.hidingSpots ++;
        }
    }

    public override void ExtraExitOperations(Collider2D collision)
    {
        if (touchingCore)
        {
            touchingCore.hidingSpots --;
            if (touchingCore.hidingSpots <= 0) { touchingCore.hidden = false; }
            touchingCore = null;
        }
        
    }

}
