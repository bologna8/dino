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

    [HideInInspector] public Transform chasing;
    public Vector2 agroMemoryTime = new Vector2(3,6);
    private float memoryCurrent;


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
            if (myMovement.colliders.Length > 5) 
            { 
                if(myMovement.colliders[5].touching) { Agro(myMovement.colliders[5].lastCollided.transform); }
                else if (currentState == State.chase) 
                {
                    memoryCurrent -= Time.deltaTime; //They forgor
                    if (memoryCurrent <= 0) { Chill(); }
                }
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

    void Agro(Transform target)
    {
        currentState = State.chase; 
        memoryCurrent = Random.Range(agroMemoryTime.x, agroMemoryTime.y);
        chasing = target;
    }

    void Chase()
    {
        if (chasing)
        {
            if (myMovement.colliders[6].touching) { Attack(); }
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
            randomAttack.tryAttack();
        }
    }


}
