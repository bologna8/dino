using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private Core myCore; //Pass inputs to your core
    public enum State { idle, patrol, chase, flee };
    public State currentState;

    private Movement myMovement; 
    private Weapon[] myWeapons;
    private Health myHealth;

    [Tooltip("Random range for how long enemy sits still before patroling")] public Vector2 idleTime = new Vector2(1,2);
    private float idleCurrent; //how long to remain idle until switch to patrol
    [Tooltip("Random range for how long enemy patrols for")] public Vector2 patrolTime = new Vector2(2,4);
    private float patrolCurrent; //how long to patrol before return to idle
    [Tooltip("Time to wait between your turns")] public Vector2 turnCooldownTime = new Vector2(3,6);
    [HideInInspector] public float turnCooldownCurrent;
    public GameObject curiousIcon;

    public float attackRange = 1f;
    [Tooltip("How long must keep in sights before attacking")] public Vector2 attackAimTime = new Vector2(1,2);
    private float currentAimTime;
    //public float sightRange = 10f;
    //public LayerMask sightLayers;
    [Tooltip("How long look at max range before scanning to look other way")] public Vector2 scanPauseTime = new Vector2(1,2);
    public GameObject angryIcon;

    [HideInInspector] public Transform chasing;
    [Tooltip("Random range for how long it takes to forget about current target")] public Vector2 agroMemoryTime = new Vector2(3,6);
    private float memoryCurrent;

    //public bool carnivore = true;
    [Tooltip("Agro instantly, or only once attacked")] public bool attackOnSight;
    [Tooltip("If see a friend start to chase, go agro on that target as well")] public bool packAttack;
    [Tooltip("Will go into a frenzy over certain distractions and damage teammates with attacks")] public bool foolish;
    [HideInInspector] public bool frenzied;


    [HideInInspector] public Aim myAim;


    // Start is called before the first frame update
    void Start()
    {
        myCore = GetComponent<Core>();
        myMovement = GetComponent<Movement>();
        myWeapons = GetComponents<Weapon>();
        myHealth = GetComponentInChildren<Health>();

        if (myAim) { myAim.waitToRotate = scanPauseTime; }
    }

    // Update is called once per frame
    void Update()
    {

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
            }
        }

        if (turnCooldownCurrent > 0) { turnCooldownCurrent -= Time.deltaTime; }
        

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

                if (myMovement.moveInput > 0) 
                {
                    if (myMovement.rightWallCheck.touching) { canGoForward = false; }
                    else if (myMovement.onGround && !myMovement.rightForwardCheck.touching) { canGoForward = false; }
                }

                if (myMovement.moveInput < 0)
                { 
                    if (myMovement.leftWallCheck.touching) { canGoForward = false; } 
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
        if(currentState == State.chase)
        {
            if (curiousIcon) { Emote(curiousIcon); }
        }

        if (myAim) { myAim.AutoAimAt = null; }

        currentState = State.idle; 
        idleCurrent = Random.Range(idleTime.x, idleTime.y);
        chasing = null;
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

            if (myCore) 
            {
                if (chasing.position.x > transform.position.x && !myCore.lookingRight) { TryToTurn(); }

                if (chasing.position.x < transform.position.x && myCore.lookingRight) { TryToTurn(); }
            }
        }

        Debug.Log(gameObject.name + " agrod onto: " + target.gameObject.name);

        currentState = State.chase; 
        memoryCurrent = Random.Range(agroMemoryTime.x, agroMemoryTime.y);
        
    }

    void Emote(GameObject emoteEffect)
    {
        myHealth.TakeDamage(0, new Vector2(0.1f, 0), Vector2.zero);
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
                if (inRange) { myMovement.moveInput = 0; }
                else
                {
                    if (myCore)
                    {
                        if (myCore.lookingRight) { myMovement.moveInput = 1; }
                        else { myMovement.moveInput = -1; }

                        if (chasing.position.x > myCore.myAim.transform.position.x && !myCore.lookingRight) { TryToTurn(); } 

                        if (chasing.position.x < myCore.myAim.transform.position.x && myCore.lookingRight) { TryToTurn(); } 
   
                    }
                    else //pre core no turn cooldown crap code
                    {
                        if (chasing.position.x > transform.position.x) 
                        { myMovement.moveInput = 1; }
                        else { myMovement.moveInput = -1; }
                    }

                }
            }
            

            if(myAim) 
            { 
                myAim.AutoAimAt = chasing;
                if (myAim.lookLockTime > currentAimTime && inRange) { Attack(); }

            }
            else if (inRange) { Attack(); }

        }
        else { Chill(); }
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

    void Attack()
    {
        if (myWeapons.Length > 0)
        {
            var randomAttack = myWeapons[Random.Range(0, myWeapons.Length)];
            randomAttack.ignoreTeams = frenzied;
            randomAttack.TryAttack();
        }
    }


}
