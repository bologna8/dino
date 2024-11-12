using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{

    //public enum State { standing, jumping, falling, edge, climbing, dashing, stunned }
    //public State currentState;
    [HideInInspector] public Spawned mySpawn;
    [HideInInspector] public int team;

    //Turning Variables
    //[HideInInspector] public bool faceRight = true;
    //[Tooltip("Always face direction of aim if possible or flip with movement")] public bool aimToTurn = true;
    [HideInInspector] public bool lookingRight = true;
    [HideInInspector] public bool movingRight;

    //private GameObject mySelf; //Transform child to turn


    public Transform CharacterArt;
    [HideInInspector] public Animator myAnim;
    private AnimatorOverrideController myAnimOverride;
    public float turnTime = 0.1f;
    [HideInInspector] public bool turning;

    [HideInInspector] public bool hidden;
    public int hidingSpots;
    [HideInInspector] public bool dashing;

    [HideInInspector] public List<IInteractable> interactables = new List<IInteractable>();
    /*
    public LayerMask interactableLayers;
    public float interactRange = 5f;
    */

    [HideInInspector] public SpriteRenderer mySprite;

    //Necessary components to connect too
    //public GameObject aimingPrefab;
    [HideInInspector] public Aim myAim;
    /*
    public Transform WeaponPivot;
    private Vector3 weaponPivotStart;

    //[Tooltip("Distance from pivot in diraction of aim, X is right hand, Y is left.")] public Vector2 HandPositions;
    public Transform RightHand;
    public Transform RightHandle;
    public Transform LeftHand;
    public Transform LeftHandle;
    */

    
    [HideInInspector] public AI myAI;


    private Weapon[] myWeapons;
    [HideInInspector] public bool[] attackInput; //Used to see if attack buttons still pressing


    [HideInInspector] public Movement myMove;
    [HideInInspector] public Health myHealth;


    [HideInInspector] public bool jumpHeld; //jump button still being pressed
    [HideInInspector] public bool canMove; //Character can move and jump
    [HideInInspector] public bool canAttack; //Character can use weapon attacks
    [HideInInspector] public bool canInteract; //Character can interact with interactable environment stuff


    // Start is called before the first frame update
    void Awake()
    {
        //if (aimingPrefab) { myAim = Instantiate(aimingPrefab, transform).GetComponent<Aim>(); }
        if (!myAim) { myAim = GetComponentInChildren<Aim>(); }

        //mySelf = transform.Find("Self").gameObject;
        if (!mySpawn) { mySpawn = GetComponent<Spawned>(); }
        if (mySpawn) 
        { 
            if (mySpawn.team != 0) { team = mySpawn.team; }
            else { team = gameObject.layer; mySpawn.team = team; }

            if (myAim) { mySpawn.source = myAim.transform; }
        }

        if(!myMove) { myMove = GetComponent<Movement>(); }
        if (myMove) { myMove.myCore = this; }

        if (!myAI) { myAI = GetComponent<AI>(); }

        if(myWeapons == null) { myWeapons = GetComponents<Weapon>(); }
        foreach(Weapon weapon in myWeapons) 
        { 
            weapon.myCore = this;
            if (myAim) 
            { 
                weapon.myAim = myAim;
                if (myAI) { myAI.myAim = myAim; }
            }
        }
        attackInput = new bool[myWeapons.Length];

        //if (WeaponPivot) { weaponPivotStart = WeaponPivot.localPosition; }

        if (!myHealth) { myHealth = GetComponentInChildren<Health>(); }
        if (myHealth) { myHealth.myCore = this; }

        hidden = false;
        hidingSpots = 0;
        if (CharacterArt) 
        { 
            if (!mySprite) { mySprite = CharacterArt.GetComponent<SpriteRenderer>(); }
            if (!myAnim) { myAnim = CharacterArt.GetComponentInChildren<Animator>(); }
            if (!myAnimOverride && myAnim) 
            { 
                myAnimOverride = new AnimatorOverrideController(myAnim.runtimeAnimatorController);
                myAnim.runtimeAnimatorController = myAnimOverride; 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myMove) 
        { 
            myMove.jumpInput = jumpHeld;
            movingRight = myMove.movingRight;

            if (myMove.momentum == 0f)
            {
                if (lookingRight && !movingRight) { myMove.movingRight = true; }
                if (!lookingRight && movingRight) { myMove.movingRight = false; }
            }
            
            if (hidden) { myMove.slowPercent = 0.25f; }
            else { myMove.slowPercent = 1f;}

        }

        for (int i = 0; i < myWeapons.Length; i++) 
        { 
            if (canAttack) { myWeapons[i].attackHeld = attackInput[i]; }
            else { myWeapons[i].attackHeld = false; }
        }

        if (myAim)
        {
            var relativeForward = transform.rotation.eulerAngles.z;

            if (myMove && !myMove.movingRight) { relativeForward += 180; }

            //if (Attacking()) { relativeForward += }

            relativeForward = relativeForward % 360;
            myAim.forwardAngle = relativeForward;


            if (myAim.myAimType != Aim.AimType.Simple)
            {
                if (myAim.transform.position.x >= transform.position.x) 
                { if(!lookingRight) { Turn(); } }
                else if (lookingRight) { Turn(); }
            }
            else if (movingRight != lookingRight) { Turn(); }

        }
        
        if (mySprite)
        {
            if (hidden) { mySprite.sortingOrder = -1; }
            else { mySprite.sortingOrder = 1; }
        }


    }


    void LateUpdate()
    {
        if (myHealth)
        {
            if (myHealth.stunTime > 0)
            {
                canMove = false;
                canAttack = false;
                canInteract = false;
            }
            else
            {
                canMove = true;
                canAttack = true;
                canInteract = true;

                if (myMove) 
                { 
                    if (myMove.onEdge) { canAttack = false; canInteract = false; }
                }
                
            }
        }

        if (myAnim) { AnimationStation(); }

    }




    public void Turn() //Every now and then
    {
        if (!turning && !Attacking())
        {
            turning = true;
            lookingRight = !lookingRight;
            if (CharacterArt) { StartCoroutine(TurnAround()); }
        } 
    }

    private IEnumerator TurnAround() //bright eyes
    {
        //turning = true;

        var fromAngle = CharacterArt.rotation;
        var toAngle = Quaternion.Euler(CharacterArt.eulerAngles + new Vector3(0, 180, 0));

        var startX = CharacterArt.localPosition.x;
        var flippedX = -startX;


        if (turnTime <= 0f) { yield return null; }
        else //turn over a set period of time, or instantly if 0
        {
            for (var t = 0f; t <= 1; t += Time.deltaTime / turnTime)
            {
                CharacterArt.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
                CharacterArt.localPosition = new Vector3(Mathf.Lerp(startX, flippedX, t), CharacterArt.localPosition.y, CharacterArt.localPosition.z);
                yield return null;
                
            }
        }
        

        CharacterArt.rotation = toAngle;
        CharacterArt.localPosition = new Vector3(flippedX, CharacterArt.localPosition.y, CharacterArt.localPosition.z);
        turning = false;

    }


    public void InteractCheck()
    {
        if (interactables.Count <= 0) { return; }

        interactables[0].Interact(gameObject);

        /*
        Collider[] interactableColliders = Physics.OverlapSphere(transform.position, interactRange, interactableLayers, QueryTriggerInteraction.Collide);
        Debug.Log("ding");

        if (interactableColliders.Length == 0) { return; }

        Debug.Log("dong");

        var interactable = interactableColliders[0].GetComponent<IInteractable>();
        if (interactable != null) { interactable.Interact(gameObject); }

        Debug.Log("dang");
        */

    }


    public void Stun(float stunTime, AnimationClip setAnim = null)
    {
        if (myHealth) 
        { 
            if (myHealth.stunTime <= stunTime) { myHealth.stunTime = stunTime; }
        }

        if (myAnim) { StartCoroutine(StunAnimation(stunTime, setAnim)); }

    }

    public IEnumerator StunAnimation(float time, AnimationClip newAnim = null)
    {
        myAnim.SetBool("interacting", true);

        if (newAnim)
        {
            var newSpeed = newAnim.length / time;
            if (time > newAnim.length) { newSpeed = time / newAnim.length; }
            myAnim.SetFloat("interactingAnimationSpeed", newSpeed);
        }
        

        if (myAnimOverride && newAnim) { myAnimOverride["blankInteract"] = newAnim; }

        yield return new WaitForSeconds(time);

        if (myAnimOverride && newAnim) { myAnimOverride["blankInteract"] = null; }
        myAnim.SetBool("interacting", false);
    }

    public bool Attacking()
    {
        foreach(var weapon in myWeapons)
        {
            if (!weapon.attackReady) { return true; }
        }

        return false;
    }


    //Recieve Inputs

    public void HandleJump()
    {
        if (canMove) { myMove.JumpStart(); }
        else { myMove.jumpInput = false; }
    }

    public void HandleMovement(float input)
    {
        if (canMove) { myMove.moveInput = input; }
        else { myMove.moveInput = 0; }
    }

    public void HandleVertical(float input)
    {
        if (!canMove) { return; }

        myMove.verticalInput = input;
        myAim.verticalInput = input;

        foreach(var weapon in myWeapons)
        {
            weapon.verticalInput = input;
        }
    }

    public void HandleDash()
    {
        if (!canMove) { return; }
        myMove.Dash();
    }

    public void HandleAttack(int attackNumber)
    {
        if (!canAttack) { return; }

        if (myWeapons.Length < attackNumber || Attacking()) { return; }

        myWeapons[attackNumber].TryAttack();

        if (myHealth.attackingResetsBuffer) //For health regen
        { myHealth.currentRegenBuffer = myHealth.regenBufferTime; }
    }

    void AnimationStation()
    {
        if (myMove)
        {
            myAnim.SetBool("onGround", myMove.onGround);
            myAnim.SetFloat("momentum", myMove.momentum);
            myAnim.SetBool("jumping", myMove.justJumped);

            if (myMove.canWallSlide) { myAnim.SetBool("onWall", myMove.wallSliding); }
            else { myAnim.SetBool("onWall", false); }
            
            myAnim.SetBool("onEdge", myMove.onEdge);

            var climbing = false;
            if (myMove.climbing != null) { climbing = true; }
            myAnim.SetBool("climbing", climbing);

            dashing = false;
            if (myMove.dashing != null) { dashing = true; }
            myAnim.SetBool("dashing", dashing);

            myAnim.SetBool("crouching", hidden);
        }

        myAnim.SetBool("attacking", Attacking());

        
    }

    public void ChangeAttackAnimation(AnimationClip newAnim, float animationSpeedMultiplier = 1)
    {
        if (myAnimOverride) { myAnimOverride["blankAttack"] = newAnim; }
        
        if (myAnim) { myAnim.SetFloat("AttackAnimationSpeed", animationSpeedMultiplier); }
    }

    public void ChangeWeapons(int index, GameObject newAttack, GameObject newAttackUp = null, GameObject newAttackDown = null)
    {
        if (index < myWeapons.Length)
        {
            myWeapons[index].setNewAttacks(newAttack, newAttackUp, newAttackDown);
        }
    }


}
