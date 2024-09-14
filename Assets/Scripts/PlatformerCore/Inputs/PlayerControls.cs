using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private Core myCore; //Pass inputs to your core
    private Controls myControls;
    public LayerMask interactableLayers;

    void Awake()
    {
        myCore = GetComponent<Core>();
        CameraFollow.target = transform;
    }

    void OnEnable()
    {
        if (myControls == null) { myControls = new Controls(); }
        myControls.Enable();
    }

    void OnDisable()
    {
        myControls.Disable();
    }


    public void Update()
    {
        myCore.jumpHeld = myControls.Base.Jump.IsPressed();

        myCore.attackInput[0] = myControls.Base.Primary.IsPressed();
        myCore.attackInput[1] = myControls.Base.Secondary.IsPressed();

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
        RaycastHit2D hitObjects = Physics2D.Raycast(transform.position, transform.forward, 5f, interactableLayers);
        if (hitObjects.collider != null)
        {
            Debug.Log("Hit " + hitObjects.collider.name);
            hitObjects.collider.GetComponent<IInteractable>().Interact(gameObject);
        }
    }

}