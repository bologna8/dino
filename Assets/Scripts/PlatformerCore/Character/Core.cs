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
    public float turnTime = 0.1f;
    [HideInInspector] public bool turning;

    [HideInInspector] public bool hidden;
    [HideInInspector] public int hidingSpots;
    private SpriteRenderer mySprite;

    //Necessary components to connect too
    public GameObject aimingPrefab;
    [HideInInspector] public Aim myAim;
    public Transform WeaponPivot;
    private Vector3 weaponPivotStart;

    //[Tooltip("Distance from pivot in diraction of aim, X is right hand, Y is left.")] public Vector2 HandPositions;
    public Transform RightHand;
    public Transform RightHandle;
    public Transform LeftHand;
    public Transform LeftHandle;

    
    private AI myAI;


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
        //mySelf = transform.Find("Self").gameObject;
        if (!mySpawn) { mySpawn = GetComponent<Spawned>(); }
        if (mySpawn) 
        { 
            if (mySpawn.team != 0) { team = mySpawn.team; }
            else { team = gameObject.layer; mySpawn.team = team; }
        }

        if(!myMove) { myMove = GetComponent<Movement>(); }
        if (myMove) { myMove.myCore = this; }

        if (aimingPrefab) { myAim = Instantiate(aimingPrefab, transform).GetComponent<Aim>(); }

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

        if (WeaponPivot) { weaponPivotStart = WeaponPivot.localPosition; }

        if (!myHealth) { myHealth = GetComponentInChildren<Health>(); }
        if (myHealth) { myHealth.myCore = this; }

        hidden = false;
        hidingSpots = 0;
        if (!mySprite && CharacterArt) { mySprite = CharacterArt.GetComponent<SpriteRenderer>(); }
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
            
        }

        for (int i = 0; i < myWeapons.Length; i++) 
        { myWeapons[i].attackHeld = attackInput[i]; }

        if (myAim)
        {
            var relativeForward = transform.rotation.eulerAngles.z;
            relativeForward = relativeForward % 360;

            if (myMove && !myMove.movingRight) { relativeForward = 180; }

            myAim.forwardAngle = relativeForward;

            if (myAim.transform.position.x >= transform.position.x) 
            { if(!lookingRight) { Turn(); } }
            else if (lookingRight) { Turn(); }


            if (WeaponPivot) 
            { 
                if (Mathf.Abs(myAim.currentAng) < 90) 
                { 
                    WeaponPivot.eulerAngles = new Vector3(0, 0, myAim.currentAng);
                    WeaponPivot.localPosition = weaponPivotStart;
                }
                else //Flip Weapon if on left side
                { 
                    WeaponPivot.eulerAngles = new Vector3(180, 0, 360 - myAim.currentAng);
                    WeaponPivot.localPosition = new Vector3(-weaponPivotStart.x, weaponPivotStart.y, weaponPivotStart.z);
                }
                myAim.offset = WeaponPivot.localPosition;



                if (RightHand && RightHandle) 
                { 
                    RightHand.position = RightHandle.position;
                    RightHand.rotation = RightHandle.rotation;
                }
                if (LeftHand && LeftHandle) 
                { 
                    LeftHand.position = LeftHandle.position;
                    LeftHand.rotation = LeftHandle.rotation;
                }
                 
            }
        }

        if (hidden) { mySprite.sortingOrder = -1; }
        else { mySprite.sortingOrder = 1;}

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
            }
        }


    }




    public void Turn() //Every now and then
    {
        if (!turning)
        {
            lookingRight = !lookingRight;
            if (CharacterArt) { StartCoroutine(TurnAround()); }
        } 
    }

    private IEnumerator TurnAround() //bright eyes
    {
        turning = true;

        var fromAngle = CharacterArt.rotation;
        var toAngle = Quaternion.Euler(CharacterArt.eulerAngles + new Vector3(0, 180, 0));

        if (turnTime <= 0f) { yield return null; }
        else //turn over a set period of time, or instantly if 0
        {
            for (var t = 0f; t <= 1; t += Time.deltaTime / turnTime)
            {
                CharacterArt.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
                yield return null;
            }
        }

        CharacterArt.rotation = toAngle;
        turning = false;
    }

    public void Stun(float stunTime)
    {
        if (myHealth) 
        { 
            if (myHealth.stunTime <= stunTime) { myHealth.stunTime = stunTime;}
        }
    }


    //Recieve Inputs

    public void HandleJump()
    {
        myMove.JumpStart();
    }

    public void HandleMovement(float input)
    {
        myMove.moveInput = input;
    }

    public void HandleVertical(float input)
    {
        myMove.verticalInput = input;
        myAim.verticalInput = input;
    }

    public void HandleDash()
    {
        myMove.Dash();
    }

    public void HandleAttack(int attackNumber)
    {
        if (myWeapons.Length < attackNumber) { return; }

        var alreadyAttacking = false;
        foreach(var weapon in myWeapons)
        {
            if (!weapon.attackReady) { alreadyAttacking = true; }
        }
        
        if (alreadyAttacking) { return; }

        myWeapons[attackNumber].TryAttack();
        if (myHealth.attackingResetsBuffer) { myHealth.currentRegenBuffer = myHealth.regenBufferTime; }
    }




}
