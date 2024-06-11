using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{

    [Header("Attack Stats")]
    [Tooltip("Damage dealt on hit")] public float damage = 1f;
    [Tooltip("Duration of stun from being hit")] public float stunTime = 0.1f;

    
    [Tooltip("Time till attack can be used again after attacking")] public float cooldown = 0.1f;
    [Tooltip("How long actual attack hitbox lasts")] public float attackLifetime = 1f;
    [Tooltip("Ignore pre and mid attack movements")] public bool moveWhileAttacking;


    [Header("Pre-Attack")]
    [Tooltip("Time it takes before attacking")] public float windup = 0.1f;
    [Tooltip("Make move before the attack, x is flipped if facing left")] public float movementPreAttack;
    [Tooltip("")] public bool onlyHorizontalMovePre = true; //otherwise move in direction aiming

    [Header("Mid-Attack")]
    [Tooltip("Duration of attack movement")] public float attackDuration = 0.1f;
    [Tooltip("Make move during the attack, x is flipped if facing left")]public float movementMidAttack;
    [Tooltip("")] public bool onlyHorizontalMoveMid = true; //otherwise move in direction aiming
    

    [Header("Knockback")]
    [Tooltip("Horizontal knockback based on direction of attack or just side of hitbox")] public bool aimedAttack = true;
    [HideInInspector] public bool faceRight = true; //Attack going right or left
    [Tooltip("Force of knockback in whatever direction attack was aimed")] public float directionalKnockback = 100f;
    [Tooltip("Use directional knockback or locked knockback")] public bool useLockedKB = false;
    [Tooltip("Knockback with a fixed horizontal and vertical amount")] public Vector2 lockedKnockback;


    [Header("Projectile stats")]
    [Tooltip("Considered melee attack if 0")] public float projectileSpeed;
    [Tooltip("Break when hitting something")] public bool breakable = false;
    [Tooltip("Layers that will break projectile")] public LayerMask hitMask; //Check for walls to stop attack
    [Tooltip("Instantiate effect where and when attack is destroyed")] public GameObject destroyEffect;


    [Header("Boomerang")]
    [Tooltip("Does not bounce back if 0")] public float returnTime;
    private float returnCurrent; //keep track of current time until boomerang returns
    [Tooltip("Speed sprite rotates")] public Vector3 rotateSpeed = new Vector3(0,0, 420);
    private bool bounced; //boomerang bounced back yet
    private Vector3 endPosition; //Point boomerang bounced back from

    //Other hidden stats
    private Rigidbody2D myBod; //nice bod
    [HideInInspector] public Transform origin; //Character that started the attack
    [HideInInspector] public Vector3 offset; //Offsset from origin for melee attacks locked at right distance
    

    [HideInInspector] public int team; //Teams are like damage layers, without changing layers
    [HideInInspector] public bool ignoreTeams = false; //Can damage others own team 
    private List<Health> hitList = new List<Health>(); //hitboxes that have already been hit
    


    // Start is called before the first frame update
    void Start()
    {
        myBod = GetComponent<Rigidbody2D>();
        if (projectileSpeed != 0f) 
        { myBod.AddForce(transform.right * projectileSpeed); }

        if (!faceRight) //Flip sprite
        { transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z); }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        attackLifetime -= Time.deltaTime;
        if (attackLifetime <= 0) 
        { 
            if (returnTime > 0f && !bounced) { BounceBack(); }
            else { Destroy(gameObject); }
        }

        if (projectileSpeed == 0f && origin) 
        { transform.position = origin.position + offset; }

        //Boomerang stuff
        if (returnTime > 0f) { transform.Rotate(rotateSpeed * Time.deltaTime); }

        if (bounced)
        {
            returnCurrent += Time.deltaTime;
            transform.position = Vector3.Lerp(endPosition, origin.position, returnCurrent / returnTime);
        }
    }

    public void Flip()
    {
        faceRight = !faceRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void BounceBack()
    { 
        attackLifetime = returnTime;
        bounced = true;
        endPosition = transform.position;
        //hitList.Clear();
        Flip();
    }

    void OnTriggerEnter2D(Collider2D other)
    {      
        CheckHit(other);        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        CheckHit(other);   
    }
    void CheckHit(Collider2D other)
    {
        var hitHealth = other.gameObject.GetComponent<Health>();
        if (hitHealth)
        {
            bool hitIt = true;
            if (damage <= 0) { hitIt = false; }
            if (hitList.Contains(hitHealth)) { hitIt = false; } 
            if (hitHealth.team == team && !ignoreTeams) { hitIt = false; }
            var checkOrigin = other.transform.GetComponentInParent<Weapon>(); 
            if (checkOrigin) { if(checkOrigin.transform == origin) { hitIt = false; } }

            if (hitIt)
            {
                hitList.Add(hitHealth); //Just hit it, don't hit again

                var KB = lockedKnockback;

                var dir = (other.transform.position - transform.position).normalized; //Find angle of attacker to attack

                if (aimedAttack) //change horizontal if aimed or always go based on direction
                { 
                    dir.x = Mathf.Abs(dir.x);
                    if (!faceRight) { dir.x *= -1; KB *= -1; }
                }
                else if (dir.x < 0) { KB.x *= -1; } //locked KB should still check direction for horizontal
                
                if (!useLockedKB) { KB = dir * directionalKnockback; } //replace directional force with locked vector

                hitHealth.TakeDamage(damage, stunTime, KB);

                var ai = other.gameObject.GetComponentInParent<AI>();
                if (ai && origin) { ai.Agro(origin); }

                if (breakable) { Destroy(gameObject); }
                else if (hitHealth.currentHP > 0 && returnTime > 0 && !bounced) 
                { BounceBack(); } //Boomerang bounce if enemy don't die
            }
        }

        //ok boomerang
        if (other.transform == origin && bounced)
        { Destroy(gameObject); }

        if ((hitMask.value & (1 << other.gameObject.layer)) > 0)
        {
            if (breakable) { Destroy(gameObject); }
            else if (!bounced) { BounceBack(); }
        }

    }

    void OnDestroy()
    {
        if (destroyEffect) 
        { 
            var effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            var grappleCheck = effect.GetComponent<Grapple>();
            if (grappleCheck) { grappleCheck.origin = origin;}
        }
    }
}
