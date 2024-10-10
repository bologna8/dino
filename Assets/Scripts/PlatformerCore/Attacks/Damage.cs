using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{

    [Header("Attack Stats")]
    [Tooltip("Damage dealt on hit")] public float damage = 1f;

    [System.Flags]
    public enum Type { none = 0, basic = 1 << 1, heavy = 1 << 2, fire = 1 << 3 }
    public Type myType;

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
    //[HideInInspector] public Transform source; //Character that started the attack
    [HideInInspector] public Vector3 offset; //Offsset from origin for melee attacks locked at right distance
    

    private Spawned mySpawn; //For item pooling and teams
    //[HideInInspector] public int team; //Teams are like damage layers, without changing layers
    [Tooltip("Can deal damage to the character that created the attack")] public bool damageSelf; 
    
    private List<Health> hitList = new List<Health>(); //hitboxes that have already been hit
    


    void OnEnable()
    {
        if (!mySpawn) { mySpawn = GetComponentInParent<Spawned>(); }

        faceRight = true;
        var ang = transform.rotation.eulerAngles.y % 360;
        if (ang > 90 && ang < 270) { faceRight = false; }

        hitList.Clear();

        startScale = transform.localScale;
        if (activeDuration > 0) 
        {
            currentDuration = activeDuration;

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

        if (hitHealth)
        {
            bool hitIt = true;

            if (hitHealth.mySpawn && mySpawn)
            {
                if (hitHealth.mySpawn.team == mySpawn.team)
                {
                    if (hitHealth.friendlyFireOnly) { hitIt = true; }
                    else if (!mySpawn.ignoreTeams) { hitIt = false; }
                    else if (!damageSelf && hitHealth.mySpawn && mySpawn) 
                    { 
                        if (hitHealth.mySpawn.source == mySpawn.source) { hitIt = false; }
                    }
                    
                }
                else if (hitHealth.friendlyFireOnly) { hitIt = false; }
            }

            if (hitList.Contains(hitHealth)) { hitIt = false; } 
            if (hitHealth.invincibleTime > 0) { hitIt = false; }
            if (hitHealth.immunities.HasFlag(myType) && hitHealth.immunities != 0) { hitIt = false; }
            

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
                if (ai && mySpawn) { ai.Agro(mySpawn.source); }

                if (destroyOnHit) { gameObject.SetActive(false); }
            }
        }

    }

    void OnDisable()
    {
        transform.localScale = startScale;

        if (endEffect)
        {
            var tempTeam = 0;
            Transform tempSource = null;
            if (mySpawn) { tempTeam = mySpawn.team; tempSource = mySpawn.source; }
            PoolManager.Instance.Spawn(endEffect, transform.position, transform.rotation, tempSource, tempTeam);
        }
    }



}
