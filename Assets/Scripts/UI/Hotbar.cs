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

    public List<HotbarItem> equippedItems;
    public int currentlyEquipped;

    public bool consumableEquipped;

    private void Start()
    {
        if (instance == null) { instance = this; }
        else { Debug.Log("duplicate hotbar somehow"); }

        playerCore = GetComponentInParent<Core>();

        var pc = GetComponentInParent<PlayerControls>(); 
        if (pc) { myControls = pc.myControls; }

        if (WeaponWheel)
        { wheelSlices = WeaponWheel.GetComponentsInChildren<MultiSpriteHandler>(); }
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
        if (index >= equippedItems.Count) { return; }

        if (playerCore == null) { return; }
        
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

    public void UnEquipItem(int index)
    {
        if (index >= equippedItems.Count) { return; }

        equippedItems.RemoveAt(index);
    }

    public void UseConsumable()
    {
        if (!consumableEquipped) { return; }

        consumableEquipped = false;
        var item = equippedItems[equippedItems.Count];
        equippedItems.RemoveAt(equippedItems.Count);

        if (Inventory.instance == null) { return; }
        Inventory.instance.RemoveItem(item);

    }

    void UpdateWeaponWheel()
    {
        if (WeaponWheel == null) { return; }

        if (usingMouse)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos.z = Camera.main.nearClipPlane;
            var mouseScreenPos = Camera.main.ScreenToWorldPoint(mousePos);  mouseScreenPos.z = 0f;

            wheelDirection = (mouseScreenPos - transform.parent.position).normalized;
        }

        if (wheelDirection == Vector2.zero || !myControls.Aiming.WeaponWheel.IsPressed()) 
        {
            foreach (var slice in wheelSlices) { slice.gameObject.SetActive(false); } 
            return; 
        }


        foreach (var slice in wheelSlices) 
        { 
            slice.gameObject.SetActive(true);
            slice.changeAlpha(unSelectedSliceAlpha, 1);
        }

        RaycastHit2D checkSlice = Physics2D.Raycast(WeaponWheel.transform.position, wheelDirection);
        if (checkSlice)
        {
            var selectedSlice = checkSlice.transform.gameObject.GetComponent<MultiSpriteHandler>();
            if (selectedSlice) 
            { 
                for(int i = 0; i < equippedItems.Count; i++)
                {
                    if (wheelSlices[i] == selectedSlice)
                    {
                        SelectItem(i);
                        selectedSlice.changeAlpha(selectedSliceAlpha, 1);
                    }
                }
            }
        }
    
    }

}
