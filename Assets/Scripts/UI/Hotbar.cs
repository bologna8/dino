using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;  

public class Hotbar : MonoBehaviour
{
    private Core playerCore;

    public static Hotbar instance;

    public GameObject WeaponWheel;
    private Vector2 wheelDirection;
    private bool usingMouse;
    //public LayerMask wheelSliceLayer;
    public MultiSpriteHandler[] wheelSlices;
    [Range (0, 1)] public float selectedSliceAlpha;
    [Range (0, 1)] public float unSelectedSliceAlpha;

    private Controls myControls;

    public HotbarItem[] equippedItems;
    public int currentlyEquipped;

    [HideInInspector] public bool canChangeWeapons;

    //public bool consumableEquipped;
    private void Start()
    {
        if (instance == null) { instance = this; }
        else { Debug.Log("duplicate hotbar somehow"); }

        playerCore = GetComponentInParent<Core>();

        var pc = GetComponentInParent<PlayerControls>(); 
        if (pc) { myControls = pc.myControls; }

        if (WeaponWheel)
        { 
            WeaponWheel.SetActive(true);
            wheelSlices = WeaponWheel.GetComponentsInChildren<MultiSpriteHandler>(); 
        }

        if (Inventory.instance != null)
        {
            foreach (var item in Inventory.instance.inventoryItemList)
            {
                if(item is HotbarItem hotbarItem)
                {
                    hotbarItem.AddToHotbar();
                }
            }
        }
    }

    void OnMouseMove()
    {
        usingMouse = true;
    }

    void OnControllerStick(InputValue val)
    {
        usingMouse = false;
        wheelDirection = val.Get<Vector2>();
    }

    void Update()
    {
        UpdateWeaponWheel();
    }

    void OnNextWeapon()
    {
        if (!canChangeWeapons) { return; }
        
        currentlyEquipped ++;
        if (currentlyEquipped > equippedItems.Length) 
        { currentlyEquipped = 0; }

        SelectItem(currentlyEquipped);
    }

    void OnPreviousWeapon()
    {
        if (!canChangeWeapons) { return; }

        currentlyEquipped --;
        if (currentlyEquipped < 0) 
        { currentlyEquipped = equippedItems.Length; }

        SelectItem(currentlyEquipped);
    }
    

    void SelectItem(int index)
    {
        if (index > equippedItems.Length) { return; }
        if (equippedItems[index] == null) { return; }
        if (playerCore == null) { return; }
        if (!canChangeWeapons) { return; }
        
        currentlyEquipped = index;

        playerCore.ChangeWeapons(0, equippedItems[index].attackPrefab, equippedItems[index].attackUp, equippedItems[index].attackDown);
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

    public void removeItem(int index, bool consumable = false)
    {
        if (index > equippedItems.Length) { return; }

        if (wheelSlices.Length >= index)
        { wheelSlices[index].changeSprite(wheelSlices[index].startSprites[1], 1); }

        if (Inventory.instance != null && consumable) 
        { Inventory.instance.RemoveItem(equippedItems[index]); }

        equippedItems[index] = null;

        if (index == currentlyEquipped)
        { SelectItem(currentlyEquipped ++); }
    }

    void UpdateWeaponWheel()
    {
        if (WeaponWheel == null) { return; }

        if (myControls.Aiming.WeaponWheel.IsPressed() && canChangeWeapons) { WeaponWheel.SetActive(true); }
        else { WeaponWheel.SetActive(false); return; }

        if (usingMouse)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos.z = Camera.main.nearClipPlane;
            var mouseScreenPos = Camera.main.ScreenToWorldPoint(mousePos);  mouseScreenPos.z = 0f;

            wheelDirection = (mouseScreenPos - transform.parent.position).normalized;
        }


        foreach (var slice in wheelSlices) 
        { 
            slice.gameObject.SetActive(true);
            slice.changeAlpha(unSelectedSliceAlpha);
        }

        RaycastHit2D checkSlice = Physics2D.Raycast(WeaponWheel.transform.position, wheelDirection);
        if (checkSlice)
        {
            var selectedSlice = checkSlice.transform.gameObject.GetComponent<MultiSpriteHandler>();
            if (selectedSlice) 
            { 
                for(int i = 0; i < wheelSlices.Length; i++)
                {
                    if (wheelSlices[i] == selectedSlice && equippedItems[i] != null)
                    {
                        selectedSlice.changeAlpha(selectedSliceAlpha);
                        SelectItem(i);
                    }
                }
            }
        }
    
    }

}
