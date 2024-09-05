using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private Core myCore; //Pass inputs to your core
    private Controls myControls;

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

        if (myCore.attackInput.Length > 0) 
        { myCore.attackInput[0] = myControls.Base.Primary.IsPressed(); }
        if (myCore.attackInput.Length > 1)
        { myCore.attackInput[1] = myControls.Base.Secondary.IsPressed(); }
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

}