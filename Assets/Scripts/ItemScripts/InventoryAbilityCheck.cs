using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAbilityCheck : MonoBehaviour
{
    private Movement myMove;
    private Core myCore;

    public HotbarItem wallJumpItem;

    public HotbarItem airDashItem;

    public bool canMoveWhileInInventory;

    // Start is called before the first frame update
    void Start()
    {
        myMove = GetComponent<Movement>();
        myCore = GetComponent<Core>();
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

        if (myCore && !canMoveWhileInInventory)
        {
            if (InventoryUI.Instance && BookController.Instance) 
            {
                if (InventoryUI.Instance.inventoryOpen || BookController.Instance.isJournalOpen)
                {
                    myCore.canMove = false; myCore.canAttack = false;
                    if (Hotbar.instance) { Hotbar.instance.canChangeWeapons = false; }
                }
                else if (Hotbar.instance) { Hotbar.instance.canChangeWeapons = true; }

            }

            
        }

    }
}
