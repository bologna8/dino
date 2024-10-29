using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAbilityCheck : MonoBehaviour
{
    private Movement myMove;

    public HotbarItem wallJumpItem;

    public HotbarItem airDashItem;

    // Start is called before the first frame update
    void Start()
    {
        myMove = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myMove && Inventory.instance != null)
        {
            if (!myMove.canWallJump)
            {
                if (Inventory.instance.ContainsItem(wallJumpItem, 1))
                { myMove.canWallJump = true; myMove.canWallSlide = true; }
            }


            if (myMove.airDashes < 1)
            {
                if (Inventory.instance.ContainsItem(airDashItem, 1))
                { myMove.airDashes = 1; }
            }
        }
    }
}
