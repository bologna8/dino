using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public float activeDistanceFromCamera;
    private Spawned mySpawn;
    private Core myCore; //Pass inputs to your core
    public enum State { idle, patrol, chase, flee };
    public State currentState;

    private Movement myMovement; 
    private Weapon[] myWeapons;
    public bool chooseAttacksRandomly; //Otherwise, prioritise melee primary when close 
    private Health myHealth;

    [Tooltip("Random range for how long enemy sits still before patroling")] public Vector2 idleTime = new Vector2(1,2);
    private float idleCurrent; //how long to remain idle until switch to patrol
    [Tooltip("Random range for how long enemy patrols for")] public Vector2 patrolTime = new Vector2(2,4);
    private float patrolCurrent; //how long to patrol before return to idle
    [Tooltip("Time to wait between your turns")] public Vector2 turnCooldownTime = new Vector2(3,6);
    [HideInInspector] public float turnCooldownCurrent;

    public GameObject curiousIcon;

    public float attackRange = 5f;
    public float proximityRange = 1f;

    [Tooltip("How long must keep in sights before attacking")] public Vector2 attackAimTime = new Vector2(1,2);
    private float currentAimTime;
    //public float sightRange = 10f;
    //public LayerMask sightLayers;
    [Tooltip("How long look at max range before scanning to look other way")] public Vector2 scanPauseTime = new Vector2(1,2);
    public GameObject angryIcon;
    public AnimationClip emoteAnimation;
    public float emoteTime;
    public Vector2 emoteCooldown = new Vector2(1,2);
    private float emoteCooldownCurrent;


    [HideInInspector] public Transform chasing;
    [Tooltip("Random range for how long it takes to forget about current target")] public Vector2 agroMemoryTime = new Vector2(3,6);
    private float memoryCurrent;

    //public bool carnivore = true;
    [Tooltip ("Jumps while chasing a target")] public bool canJump;
    [Tooltip("Agro instantly, or only once attacked")] public bool attackOnSight;
    [Tooltip("Leap before attacking")] public bool jumpAttack;
    [Tooltip("If see a friend start to chase, go agro on that target as well")] public bool packAttack;
    [Tooltip("Will retreat after attacking")] public bool skirmish;
    [Tooltip("Will retreat when too far from their start, and attack if too close")] public bool territorial;
    [Tooltip("Will go into a frenzy over certain distractions and damage teammates with attacks")] public bool foolish;
    [HideInInspector] public bool frenzied;

    [System.Flags]
    public enum Type { none = 0, carnivore = 1 << 1, herbivore = 1 << 2 }
    public Type myType;

    [HideInInspector] public Aim myAim;


    // Start is called before the first frame update
    void OnEnable()
    {
        if (!mySpawn) { mySpawn = GetComponent<Spawned>(); }
        if (!myCore) { myCore = GetComponent<Core>(); }
        if (!myMovement) { myMovement = GetComponent<Movement>(); }
        if (myWeapons == null) { myWeapons = GetComponents<Weapon>(); }
        if (!myHealth) { myHealth = GetComponentInChildren<Health>(); }


        if (myAim) { myAim.waitToRotate = scanPauseTime; }
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main && activeDistanceFromCamera >= 0)
        {
            if (Vector3.Distance(Camera.main.transform.position, transform.position) > activeDistanceFromCamera) { Chill(); return; }
        }
        

        if (myAim)
        {
            if (myAim.lastSeen)
            {
                var checkCore = myAim.lastSeen.GetComponent<Core>();
                if (checkCore && myCore) 
                { 
                    if (!checkCore.hidden) 
                    {
                        if (checkCore.team == myCore.team) 
                        { 
                            if (frenzied) { Agro(myAim.lastSeen); }
                            else if (packAttack && checkCore.myAI)
                            {
                                if (checkCore.myAI.currentState == AI.State.chase)
                                {
                                    Agro(checkCore.myAI.chasing);
                                }
                            }
                        }
                        else if (attackOnSight)  { Agro(myAim.lastSeen); }
                    }
                    
                }
            }
        }


        if (currentState == State.chase) 
        {
            memoryCurrent -= Time.deltaTime; //They forgor
            if (memoryCurrent <= 0) { Chill(); }
        }


        if (myHealth)
        {
            if (myHealth.stunTime > 0) { if(myMovement) { myMovement.moveInput = 0; } }
            else
            {
                if (currentState == State.idle) { Idle(); }
                if (currentState == State.patrol) { Patrol(); }

                if (currentState == State.chase) { Chase(); }
                if (currentState == State.flee) { Flee(); }
            }
        }

        if (turnCooldownCurrent > 0) { turnCooldownCurrent -= Time.deltaTime; }
        if (emoteCooldownCurrent > 0) { emoteCooldownCurrent -= Time.deltaTime; }

        if (myMovement) { if (myMovement.onGround && myMovement.jumpInput) { myMovement.jumpInput = false; } }

    }

    void Idle()
    {
        if (myMovement) { myMovement.moveInput = 0; }

        idleCurrent -= Time.deltaTime;
        if (idleCurrent <= 0 && patrolTime != Vector2.zero) 
        { 
            currentState = State.patrol; 
            patrolCurrent = Random.Range(patrolTime.x, patrolTime.y); 
        }
        
    }

    void Patrol()
    {
        patrolCurrent -= Time.deltaTime;

        if (patrolCurrent <= 0 && idleTime != Vector2.zero) { Chill(); }
        else
        {
            if (myMovement && myCore) 
            {

                bool canGoForward = true;

                if (myMovement.moveInput > 0 && myMovement.rightHipCheck) 
                {
                    if (myMovement.rightHipCheck.touching) { canGoForward = false; }
                    else if (myMovement.onGround && !myMovement.rightForwardCheck.touching) { canGoForward = false; }
                }

                if (myMovement.moveInput < 0 && myMovement.leftHipCheck)
                { 
                    if (myMovement.leftHipCheck.touching) { canGoForward = false; } 
                    else if (myMovement.onGround && !myMovement.leftForwardCheck.touching) { canGoForward = false; }
                }


                if (canGoForward)
                {
                    if (myCore.lookingRight) { myMovement.moveInput = 1; }
                    else { myMovement.moveInput = -1; }  
                }
                else { TryToTurn(); }
                
            }
        }

        
    }

    void Chill()
    {
        if (myMovement) { myMovement.moveInput = 0; }

        if (currentState == State.idle) { return; } 

        if(currentState == State.chase)
        {
            if (curiousIcon) { Emote(curiousIcon); }
        }
        chasing = null;

        if (myAim) { myAim.AutoAimAt = null; }
    
        idleCurrent = Random.Range(idleTime.x, idleTime.y);
        currentState = State.idle; 
                
    }

    public void Agro(Transform target)
    {
        if (target != chasing)
        {
            var detectDecoy = target.GetComponent<Decoy>();
            if (detectDecoy)
            {
                if (detectDecoy.causeFrenzy) { if(angryIcon) { Emote(angryIcon); } }
                else { if (curiousIcon) { Emote(curiousIcon); } }
            }
            else
            {
                var checkCore = target.GetComponent<Core>();
                if(checkCore) { if(angryIcon) { Emote(angryIcon); } }
                else { if (curiousIcon) { Emote(curiousIcon); } }
            }

            
            if (myAim) 
            {
                currentAimTime = Random.Range(attackAimTime.x, attackAimTime.y);
                myAim.AutoAimAt = target;
            }

            chasing = target;

            /*
            if (myCore) 
            {
                if (chasing.position.x > transform.position.x && !myCore.lookingRight) { TryToTurn(); }

                if (chasing.position.x < transform.position.x && myCore.lookingRight) { TryToTurn(); }
            }
            */
        }

        //Debug.Log(gameObject.name + " agrod onto: " + target.gameObject.name);

        currentState = State.chase; 
        memoryCurrent = Random.Range(agroMemoryTime.x, agroMemoryTime.y);
        
    }

    void Emote(GameObject emoteEffect)
    {
        if (emoteCooldownCurrent <= 0)
        {
            emoteCooldownCurrent = Random.Range(emoteCooldown.x, emoteCooldown.y);
            myCore.Stun(emoteTime, emoteAnimation);
        }        

        if (PoolManager.Instance) { PoolManager.Instance.Spawn(emoteEffect, transform.position, Quaternion.identity, transform); }
        else { Instantiate(emoteEffect, transform); }
    }

    void Chase()
    {
        if (chasing)
        {
            bool inRange = false;
            if (Vector3.Distance(chasing.position, transform.position) < attackRange) { inRange = true; }

            if (myMovement)
            {
                if (myCore)
                {
                    if (chasing.position.x > transform.position.x && !myCore.lookingRight) { TryToTurn(); } 

                    if (chasing.position.x < transform.position.x && myCore.lookingRight) { TryToTurn(); }  

                    //Reset memory while target is close
                    if (Vector3.Distance(chasing.position, transform.position) < proximityRange) 
                    { memoryCurrent = Random.Range(agroMemoryTime.x, agroMemoryTime.y); myMovement.moveInput = 0; }
                    else if (myCore.lookingRight) { myMovement.moveInput = 1; }
                    else { myMovement.moveInput = -1; }

                    if (canJump) 
                    { 
                        var shouldJump = false;

                        if (myMovement.rightHipCheck && myMovement.movingRight) 
                        { if (myMovement.rightHipCheck.touching) { shouldJump = true; } }

                        if (myMovement.leftHipCheck && !myMovement.movingRight) 
                        { if (myMovement.leftHipCheck.touching) { shouldJump = true; } }

                        if (chasing.transform.position.y > (transform.position.y + myMovement.myCollider.bounds.extents.y))
                        { shouldJump = true; }

                        if (shouldJump) { myMovement.JumpStart(); myMovement.jumpInput = true; }
                        //else if (myMovement.onGround) { myMovement.jumpInput = false; }

                    }
                    

                }
                else //pre core no turn cooldown crap code
                {
                    if (chasing.position.x > transform.position.x) 
                    { myMovement.moveInput = 1; }
                    else { myMovement.moveInput = -1; }
                }

            }
            

            bool attacked = false;
            if(myAim) 
            { 
                myAim.AutoAimAt = chasing;
                if (myAim.lookLockTime >= currentAimTime && inRange) { attacked = Attack(); }
            }
            else if (inRange) { attacked = Attack(); }

            if (attacked) 
            { 
                if (jumpAttack && myMovement) { myMovement.JumpStart(); myMovement.jumpInput = true; }
                if (skirmish) { Retreat(chasing); }
            }

        }
        else { Chill(); }
    }

    public void Retreat(Transform target = null)
    {
        //var 
        if (target)
        {

        }
        else 
        {
            //if (mySpawn) { = mySpawn.origin; }
        }

        currentState = State.flee;
    }

    void Flee()
    {

    }



    public void TryToTurn()
    {
        if (turnCooldownCurrent <= 0 && myCore) 
        {
            myCore.Turn();
            if (myMovement) { myMovement.momentum = 0; myMovement.moveInput = 0; }
            turnCooldownCurrent = Random.Range(turnCooldownTime.x, turnCooldownTime.y);
            //else if (myMovement) { myMovement.moveInput = 0; }
        }
        else if (myMovement) { myMovement.momentum = 0; myMovement.moveInput = 0; }
    }

    bool Attack()
    {
        if (myWeapons.Length > 0)
        {
            var chosenAttack = myWeapons[0];

            if (myWeapons.Length > 1)
            {
                foreach (var weapon in myWeapons) { if (!weapon.attackReady) { return false; } } // don't start an attack if already doing another attack

                if (chooseAttacksRandomly) { chosenAttack = myWeapons[Random.Range(0, myWeapons.Length)]; }
                else if (Vector3.Distance(chasing.position, transform.position) > proximityRange) //use other attacks at a range
                {
                    chosenAttack = myWeapons[Random.Range(1, myWeapons.Length)];
                }
            }
            
            chosenAttack.ignoreTeams = frenzied;
            
            if (chosenAttack.TryAttack()) { return true; }
        }

        return false;
    }


}
