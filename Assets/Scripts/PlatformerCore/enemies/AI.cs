using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private Core myCore; //Pass inputs to your core
    public enum State { idle, chase, patrol };
    public State currentState;

    private Movement myMovement; 
    private Weapon[] myWeapons;
    private Health myHealth;

    [Tooltip("Random range for how long enemy sits still before patroling")] public Vector2 idleTime = new Vector2(1,2);
    private float idleCurrent; //how long to remain idle until switch to patrol
    [Tooltip("Random range for how long enemy patrols for")] public Vector2 patrolTime = new Vector2(2,4);
    private float patrolCurrent; //how long to patrol before return to idle
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
    [HideInInspector] public bool frenzied;


    [HideInInspector] public Aim myAim;

    //[HideInInspector] public Budding touchingBud;


    // Start is called before the first frame update
    void Start()
    {
        myCore = GetComponent<Core>();
        myMovement = GetComponent<Movement>();
        myWeapons = GetComponents<Weapon>();
        myHealth = GetComponentInChildren<Health>();
        myHealth.team = gameObject.layer;

        //if (myCore) { if(myCore.myAim) { myAim = myCore.myAim; Debug.Log("yup"); } }
        if (myAim) { myAim.waitToRotate = scanPauseTime; }
    }

    // Update is called once per frame
    void Update()
    {
        if (myHealth)
        {
            //Look Ahead
            //var facing = Vector2.right;
            //if (!myMovement.faceRight) { facing *= -1;}

            //RaycastHit2D hit = Physics2D.Raycast(transform.position, facing, sightRange, sightLayers);

            //if (hit.collider != null) 
            //{
                //var debugColor = Color.yellow;

                /*
                if (carnivore) //Meat eater
                { 
                    var player = hit.transform.gameObject.GetComponent<PlayerControls>();
                    bool seePlayer = false; 
                    //if (player) { if (!player.hidden) { seePlayer = true; } }
                    
                    var decoy = hit.transform.gameObject.GetComponent<Decoy>();
                    bool takeTheBait = false;
                    if (decoy) { if  (decoy.bait) { takeTheBait = true; } }

                    if (seePlayer || takeTheBait) { debugColor = Color.red; Agro(hit.transform); }
                }
                */

                /*
                else //Herbivor stuff
                {
                    if (touchingBud) 
                    { 
                        if (touchingBud.flower) { debugColor = Color.red; Agro (touchingBud.transform); }
                        else if (touchingBud.linked) { debugColor = Color.red; Agro (touchingBud.linked.transform); }
                        else { currentState = State.patrol; }
                    }
                    
                    else
                    {
                        var bud = hit.transform.gameObject.GetComponent<Budding>();
                        if (bud)
                        {
                            if (bud.flower) { debugColor = Color.red; Agro(hit.transform); }
                            else { currentState = State.patrol; }
                        }
                        else { currentState = State.patrol; }
                    }
                    
                }
                */

                //Debug.DrawRay(transform.position, facing * hit.distance, debugColor);
            //}
            //else { Debug.DrawRay(transform.position, facing * sightRange, Color.green); } 
            
            
            if (myAim)
            {
                if (myAim.lastSeen)
                {
                    var isPlayer = myAim.lastSeen.GetComponent<PlayerControls>();
                    if (isPlayer) { Agro(myAim.lastSeen); }
                }
            }


            if (currentState == State.chase) 
            {
                memoryCurrent -= Time.deltaTime; //They forgor
                if (memoryCurrent <= 0) { Chill(); }
            }
            

            if (myHealth.stunTime > 0) { if(myMovement) { myMovement.moveInput = 0; } }
            else
            {
                if (currentState == State.idle) { Idle(); }
                if (currentState == State.patrol) { Patrol(); }

                if (currentState == State.chase) { Chase(); }
            }
        }
        

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

                if (myCore.lookingRight) 
                { 
                    if (myMovement.rightWallCheck.touching) { myCore.Turn(); }
                    if (myMovement.onGround && !myMovement.rightForwardCheck.touching) { myCore.Turn(); }
                }
                else 
                { 
                    if (myMovement.leftWallCheck.touching) { myCore.Turn(); } 
                    if (myMovement.onGround && !myMovement.leftForwardCheck.touching) { myCore.Turn(); }
                }
                
                if (myCore.lookingRight) { myMovement.moveInput = 1; }
                else { myMovement.moveInput = -1; }  
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
            var player = target.GetComponent<PlayerControls>();
            if(player) { if(angryIcon) { Emote(angryIcon); } }
            else { if (curiousIcon) { Emote(curiousIcon); } }

            if (myAim) 
            {
                currentAimTime = Random.Range(attackAimTime.x, attackAimTime.y);
                myAim.AutoAimAt = target;
            }

            chasing = target;
        }

        currentState = State.chase; 
        memoryCurrent = Random.Range(agroMemoryTime.x, agroMemoryTime.y);
        
    }

    void Emote(GameObject emoteEffect)
    {
        myHealth.TakeDamage(0, new Vector2(0.1f, 0), Vector2.zero);
        Instantiate(emoteEffect, transform);
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
                    if (chasing.position.x > transform.position.x) 
                    { myMovement.moveInput = 1; }
                    else { myMovement.moveInput = -1; }
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

    void Attack()
    {
        if (myWeapons.Length > 0 && myHealth.stunTime <= 0)
        {
            var randomAttack = myWeapons[Random.Range(0, myWeapons.Length)];
            randomAttack.ignoreTeams = frenzied;
            randomAttack.TryAttack();
        }
    }


}
