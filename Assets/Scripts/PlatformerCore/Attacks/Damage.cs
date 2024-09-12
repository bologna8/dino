using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{

    [Header("Attack Stats")]
    [Tooltip("Damage dealt on hit")] public float damage = 1f;
    [Tooltip("How long actual attack hitbox lasts. If 0 or less, lasts forever")] public float activeDuration = 0f;    
    private float currentDuration; //how long attack has been going
    [Tooltip("Percent Scale of Hitbox increases or decreases during attack duration")] public AnimationCurve sizeOverTime;
    private Vector3 startScale; //Store local starting scale of attack
    [Tooltip("Percent Damage increases or decreases during attack duration")] public AnimationCurve damageOverTime;
    [Tooltip("Attack ends immediately once it hits")] public bool destroyOnHit = false;
    [Tooltip("Spawn a prefab at the end of this attack")] public GameObject endEffect;
    [Tooltip("Duration of stun if hit, x is hard stun lock, y is fall off time of knockback after stun")] public Vector2 stunTime;
    

    [Header("Knockback")]
    [Tooltip("Horizontal knockback based on direction of attack or just side of hitbox")] public bool aimedAttack = true;
    [HideInInspector] public bool faceRight = true; //Attack going right or left
    [Tooltip("Force of knockback in whatever direction attack was aimed")] public float directionalKnockback = 100f;
    [Tooltip("Use directional knockback or locked knockback")] public bool useLockedKB = false;
    [Tooltip("Knockback with a fixed horizontal and vertical amount")] public Vector2 lockedKnockback;
    [Tooltip("Percent Knock back increases or decreases during attack duration")] public AnimationCurve knockbackOverTime;


    //Other hidden stats
    [HideInInspector] public Transform source; //Character that started the attack
    [HideInInspector] public Vector3 offset; //Offsset from origin for melee attacks locked at right distance
    

    private Spawned mySpawn; //For item pooling and teams
    [HideInInspector] public int team; //Teams are like damage layers, without changing layers
    [Tooltip("Friendly fire can damage others on the same team")] public bool ignoreTeams = false; //Can damage others own team 
    
    private List<Health> hitList = new List<Health>(); //hitboxes that have already been hit
    


    void OnEnable()
    {
        if (!mySpawn) { mySpawn = GetComponentInParent<Spawned>(); }
        if (mySpawn) //Reset on every awake
        { 
            team = mySpawn.team; 
            source = mySpawn.source;
        }

        faceRight = true;
        var ang = transform.rotation.eulerAngles.y % 360;
        if (ang > 90 && ang < 270)
        //var ang = transform.rotation.eulerAngles.y % 360;
        { faceRight = false; }
        //if (transform.rotation.eulerAngles.z % 360 > 180) { faceRight = false; Debug.Log("face left"); }

        hitList.Clear();

        if (activeDuration > 0) 
        {
            currentDuration = activeDuration;
            startScale = transform.localScale;

            transform.localScale = startScale * sizeOverTime.Evaluate(0);
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if(activeDuration > 0)
        {
            currentDuration -= Time.deltaTime;
            transform.localScale = startScale * sizeOverTime.Evaluate((activeDuration - currentDuration)/activeDuration);

            if(currentDuration <= 0) { gameObject.SetActive(false); }
        }

    }

    public void Flip()
    {
        faceRight = !faceRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
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

        if (hitHealth && team != 0)
        {
            bool hitIt = true;
            //if (damage <= 0) { hitIt = false; }
            if (hitList.Contains(hitHealth)) { hitIt = false; } 
            if (hitHealth.invincibleTime > 0) { hitIt = false; }

            if (hitHealth.team == team)
            {
                if (!ignoreTeams) { hitIt = false; }
                if (hitHealth.friendlyFireOnly) { hitIt = true; }
            }
            else if (hitHealth.friendlyFireOnly) { hitIt = false; }

            //var checkOrigin = other.transform.GetComponentInParent<Weapon>(); 
            //if (checkOrigin) { if(checkOrigin.transform == origin) { hitIt = false; } } //never damage self
            
            //if (other.transform == source) { hitIt = false; Debug.Log("dong"); }

            if (hitIt)
            {
                hitList.Add(hitHealth); //Don't double hit anything

                var dmg = damage;

                var KB = lockedKnockback;

                var dir = (other.transform.position - transform.position).normalized; //Find angle of attacker to attack

                if (aimedAttack) //change horizontal knockback if aimed or locked
                { 
                    dir.x = Mathf.Abs(dir.x);
                    if (!faceRight) { dir.x *= -1; KB *= -1; }
                }
                else if (dir.x < 0) { KB.x *= -1; } //locked KB should still check direction for horizontal
                
                if (!useLockedKB) //replace directional force with locked vector
                { KB = dir * directionalKnockback; } 


                if (activeDuration > 0) 
                { 
                    dmg *= damageOverTime.Evaluate((activeDuration - currentDuration)/activeDuration);
                    KB *= knockbackOverTime.Evaluate((activeDuration - currentDuration)/activeDuration);
                }

                hitHealth.TakeDamage(dmg, stunTime, KB);

                var ai = other.gameObject.GetComponentInParent<AI>();
                if (ai && source) { ai.Agro(source); }

                if (destroyOnHit) { gameObject.SetActive(false); }
            }
        }

    }

    void OnDisable()
    {
        transform.localScale = startScale;

        if (endEffect)
        {
            PoolManager.Instance.Spawn(endEffect, transform.position, transform.rotation, source, team);
        }
    }



}