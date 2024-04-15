using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public GameObject aimingPrefab;
    public enum State { idle, patrol, chase };
    public State currentState;
    private Movement myMovement; 
    private Weapon[] myWeapons;
    private Health myHealth;

    public Vector2 idleTime = new Vector2(1,2);
    private float idleCurrent;
    public Vector2 patrolTime = new Vector2(2,4);
    private float patrolCurrent;

    public float attackRange = 1f;
    public float sightRange = 10f;
    public LayerMask sightLayers;

    [HideInInspector] public Transform chasing;
    public Vector2 agroMemoryTime = new Vector2(3,6);
    private float memoryCurrent;

    public bool carnivore = true;
    [HideInInspector] public bool frenzied;


    // Start is called before the first frame update
    void Start()
    {
        myMovement = GetComponent<Movement>();
        myWeapons = GetComponents<Weapon>();
        myHealth = GetComponentInChildren<Health>();
        myHealth.team = gameObject.layer;

        if (aimingPrefab)
        {
            var myAim = Instantiate(aimingPrefab).GetComponent<Aim>();
            myAim.lockT = transform;
            myAim.myAI = this;

            foreach (Weapon w in myWeapons) { w.myAim = myAim; }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myMovement && myHealth)
        {
            //Look Ahead
            var facing = Vector2.right;
            if (!myMovement.faceRight) { facing *= -1;}

            RaycastHit2D hit = Physics2D.Raycast(transform.position, facing, sightRange, sightLayers);

            if (hit.collider != null) 
            {
                var debugColor = Color.yellow;

                var player = hit.transform.gameObject.GetComponent<PlayerControls>();
                bool seePlayer = false; 
                if (player) { if (!player.hidden) { seePlayer = true; } }
                
                var decoy = hit.transform.gameObject.GetComponent<Decoy>();
                bool takeTheBait = false; 
                if (decoy) { if  (decoy.bait && carnivore) { takeTheBait = true; } }

                if (seePlayer || takeTheBait) { debugColor = Color.red; Agro(hit.transform); }

                Debug.DrawRay(transform.position, facing * hit.distance, debugColor);
            }
            else { Debug.DrawRay(transform.position, facing * sightRange, Color.green); } 


            if (currentState == State.chase) 
            {
                memoryCurrent -= Time.deltaTime; //They forgor
                if (memoryCurrent <= 0) { Chill(); }
            }
            

            if (myHealth.stunTime > 0) { myMovement.moveInput = 0; }
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
        myMovement.moveInput = 0;
        idleCurrent -= Time.deltaTime;
        if (idleCurrent <= 0) 
        { 
            currentState = State.patrol; 
            patrolCurrent = Random.Range(patrolTime.x, patrolTime.y); 
        }
    }

    void Patrol()
    {
        patrolCurrent -= Time.deltaTime;
        if (patrolCurrent <= 0) { Chill(); }
        else
        {
            if (myMovement.colliders[1].touching) { myMovement.Turn(); }
            if (myMovement.onGround && !myMovement.colliders[4].touching) { myMovement.Turn(); }
            
            if (myMovement.faceRight) { myMovement.moveInput = 1; }
            else { myMovement.moveInput = -1; }  
        }

        
    }

    void Chill()
    {
        currentState = State.idle; 
        idleCurrent = Random.Range(idleTime.x, idleTime.y);
        chasing = null;
    }

    public void Agro(Transform target)
    {
        currentState = State.chase; 
        memoryCurrent = Random.Range(agroMemoryTime.x, agroMemoryTime.y);
        chasing = target;
    }

    void Chase()
    {
        if (chasing)
        {
            if (Vector3.Distance(chasing.position, transform.position) < attackRange)
            {
                myMovement.moveInput = 0;
                Attack();
            }
            else
            {
                if (chasing.position.x > transform.position.x) 
                { myMovement.moveInput = 1; }
                else { myMovement.moveInput = -1; }
            }
        }
        else { Chill(); }
    }

    void Attack()
    {
        if (myWeapons.Length > 0 && myHealth.stunTime <= 0)
        {
            var randomAttack = myWeapons[Random.Range(0, myWeapons.Length)];
            randomAttack.ignoreTeams = frenzied;
            randomAttack.tryAttack();
        }
    }


}
