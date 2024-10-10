using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  

public class Hotbar : MonoBehaviour
{
    public Core playerCore;

    public static Hotbar instance;

    private Controls myControls;

    public List<HotbarItem> equippedItems;
    public int currentlyEquipped;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Debug.Log("duplicate hotbar somehow"); }

        playerCore = GetComponentInParent<Core>();

        var pc = GetComponentInParent<PlayerControls>(); 
        if (pc) { myControls = pc.myControls; }
    }

    void OnMouseMove()
    {

    }

    void OnControllerStick(InputValue val)
    {
        
    }

    void OnNextWeapon()
    {
        currentlyEquipped ++;
        if (currentlyEquipped >= equippedItems.Count) 
        { currentlyEquipped = 0; }

        SelectItem(currentlyEquipped);
    }

    void OnPreviousWeapon()
    {
        currentlyEquipped --;
        if (currentlyEquipped < 0) 
        { currentlyEquipped = equippedItems.Count - 1; }

        SelectItem(currentlyEquipped);
    }

    

    void SelectItem(int index)
    {
        if (index < equippedItems.Count) 
        {
            currentlyEquipped = index;

            if (playerCore)
            {
                playerCore.ChangeWeapons(0, equippedItems[index].attackPrefab, equippedItems[index].attackUp, equippedItems[index].attackDown);
            }
        }
    }

    void OnOne()
    {
        if (currentlyEquipped != 0) { SelectItem(0); }
    }

    void OnTwo()
    {
        if (currentlyEquipped != 1) { SelectItem(1); }
    }
    void OnThree()
    {
        if (currentlyEquipped != 2) { SelectItem(2); }
    }
    void OnFour()
    {
        if (currentlyEquipped != 3) { SelectItem(3); }
    }
    void OnFive()
    {
        if (currentlyEquipped != 4) { SelectItem(4); }
    }

}
