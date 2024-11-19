using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Bush : LayerCheck, IInteractable
{

    private Core touchingCore;

    private bool isGamepad;

    public GameObject bushEnterEffect;

    private Animator myAnim;

    public int setHideLayer;
    public int defaultPlayerLayer; 

    public Text interactionPrompt; //UI Text

    private void Awake()
    {
        if(interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        interactionPrompt = GameManager.instance.InteractionText;
    }

    private void Update()
    {
        isGamepad = Gamepad.current != null;

        if(interactionPrompt != null && touchingCore)
        {
            interactionPrompt.text = isGamepad ? "Press [X] to hide" : "Press [E] to hide";
        }
    }

    public void Interact(GameObject interacter)
    {
        if (touchingCore)
        {
            if (touchingCore.myMove.dashCheck && touchingCore.hidden) 
            { if (touchingCore.myMove.dashCheck.touching) { return; } }

            touchingCore.hidden = !touchingCore.hidden;

            if (myAnim) { myAnim.SetTrigger("rustled"); }

            if (touchingCore.hidden)
            { 
                if (bushEnterEffect) { PoolManager.Instance.Spawn(bushEnterEffect, transform.position); }
                interacter.layer = setHideLayer;
            }
            else { interacter.layer = defaultPlayerLayer; }
        }
    }

    public override void ExtraEnterOperations(Collider2D collision)
    {
        var coreCheck = collision.gameObject.GetComponent<Core>();
        if (coreCheck && !touchingCore)
        {
            touchingCore = coreCheck;
            touchingCore.hidingSpots ++;
            touchingCore.interactables.Add(this);   

            if(interactionPrompt !=null)
            {
                interactionPrompt.gameObject.SetActive(true);
            }

            if (myAnim) { myAnim.SetTrigger("rustled"); }
        }
    }

    public override void ExtraExitOperations(Collider2D collision)
    {
        if (touchingCore)
        {
            touchingCore.hidingSpots --;
            touchingCore.interactables.Remove(this);
            
            if (touchingCore.hidingSpots <= 0) 
            { 
                touchingCore.hidden = false;
                if (!touchingCore.dashing) { touchingCore.gameObject.layer = defaultPlayerLayer; }
            }

            touchingCore = null;

            if(interactionPrompt != null)
            {
                interactionPrompt.gameObject.SetActive(false);
            }

            //if (myAnim) { myAnim.SetTrigger("rustled"); }


        }
        
    }


}
