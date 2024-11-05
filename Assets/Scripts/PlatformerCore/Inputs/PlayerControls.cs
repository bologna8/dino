using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private Core myCore; //Pass inputs to your core
    private PlayerInput myInput;
    [HideInInspector] public Controls myControls;

    void Awake()
    {
        if (myControls == null) { myControls = new Controls(); }
        myInput = GetComponent<PlayerInput>();
        myCore = GetComponent<Core>();
        CameraFollow.target = transform;
    }

    void OnEnable()
    {
        myControls.Enable();
        myInput.actions.FindActionMap("Aiming").Enable();
    }

    void OnDisable()
    {
        myControls.Disable();
        myInput.actions.FindActionMap("Aiming").Disable();
    }


    public void Update()
    {
        myCore.jumpHeld = myControls.Base.Jump.IsPressed();

        myCore.attackInput[0] = myControls.Base.Primary.IsPressed();
        //myCore.attackInput[1] = myControls.Base.Secondary.IsPressed();

        // Delete later for EZ:
        Debug.DrawRay(transform.position, transform.forward * 200f, Color.red);
    }

    void OnJump()
    {
        myCore.HandleJump();
    }
    
    void OnMove(InputValue val)
    {
        myCore.HandleMovement(val.Get<float>());
    }

    void OnVertical(InputValue val)
    {
        myCore.HandleVertical(val.Get<float>());
    }

    void OnDash()
    {
        myCore.HandleDash();
    }

    void OnPrimary()
    {
        myCore.HandleAttack(0);
    }

    void OnSecondary()
    {
        myCore.HandleAttack(1);
    }

    public void OnInteract()
    {
        /*
        RaycastHit2D hitObjects = Physics2D.Raycast(transform.position, transform.forward, interactRange, interactableLayers);
        if (hitObjects.collider != null)
        {
            var checkInter = hitObjects.collider.GetComponent<IInteractable>();
            if (checkInter != null) { 
                checkInter.Interact(gameObject);
            }
        }
        */
        myCore.InteractCheck();
    }

}