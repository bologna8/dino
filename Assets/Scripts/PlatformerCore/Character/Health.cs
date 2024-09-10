using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Tooltip("Maximum Health")] public float maxHP = 10f;
    [Tooltip("Current Health Remaining")] public float currentHP;


    [HideInInspector] public Core myCore;
    //[Tooltip("If this health is on a child object, should it destroy entire object when destroyed")] public bool destroyParent = false;
    private Spawned mySpawn;
    [HideInInspector] public Transform source; //The transform that created the attack
    [HideInInspector] public int team = -1;
    //private float respawnTime = 1f;
    [Tooltip("Health Bar That is shown When Damaged")] public GameObject HealthBarPrefab;

    [Tooltip("How much health gained per second while recovering")] public float passiveRegen;
    [Tooltip("Time before passive regen recovery kicks in, taking damage resets this timer")] public float regenBufferTime;
    [HideInInspector] public float currentRegenBuffer; //Count down while not taking damage
    [Tooltip("Should attacking reset the current regen buffer time")] public bool attackingResetsBuffer;

    [HideInInspector] public float stunTime;
    [HideInInspector] public float invincibleTime;
    [HideInInspector] public Movement myMovement;

    [Tooltip("Effect spawned Every Time Hit")] public GameObject hitEffect;
    private TrailRenderer hitTrail;
    [Tooltip("Effect spawned when destroyed")] public GameObject deathEffect;
    [Tooltip("Offset for death deffect")] public Vector3 deathOffset;

    [Tooltip("Will only take damage from its own team, normally other way around")] public bool friendlyFireOnly = false;

    void Awake()
    {
        if (myMovement == null) { myMovement = GetComponentInParent<Movement>(); }

        if (mySpawn == null) { mySpawn = GetComponentInParent<Spawned>(); }
    }

    void OnEnable()
    {
        //if(team < 0) { team = gameObject.layer;}
        
        if (mySpawn) //Reset on every awake
        { 
            team = mySpawn.team; 
            source = mySpawn.source;
        }

        if (HealthBarPrefab)
        {
            //var findCanvas = GameObject.Find("Canvas");
            //if (findCanvas) { Instantiate(HealthBarPrefab, findCanvas.transform).GetComponent<HealthUI>().tracking = this; }
            var hpBar = PoolManager.Instance.Spawn(HealthBarPrefab, transform.position, Quaternion.identity, source, team, true);
            hpBar.GetComponent<HealthUI>().tracking = this;
        }
        

        currentHP = maxHP;
        //stunTime = respawnTime;

        //hitTrail = GetComponent<TrailRenderer>();
        //if (hitTrail) { hitTrail.enabled = false; } //What if you want trail going from the start? Like a projectile with a health
    }

    void Update()
    {
        if (invincibleTime > 0) { invincibleTime -= Time.deltaTime; }
        if (stunTime > 0) { stunTime -= Time.deltaTime; }
        
        if (passiveRegen > 0) { Regenerate(); }
    }

    public void TakeDamage(float dmg, Vector2 stun, Vector2 KB)
    {
        Debug.Log("ding");
        
        if (hitEffect) { Instantiate(hitEffect, transform.position, Quaternion.identity); }

        currentHP -= dmg;

        if (dmg > 0) { currentRegenBuffer = regenBufferTime; }

        if (currentHP <= 0) { Die(); }
        else
        {
            if (stunTime <= stun.x) { stunTime = stun.x; }

            if (KB != Vector2.zero && myMovement) { myMovement.DoDash(KB, stun); }
        }
    }

    public void Die()
    {
        if (deathEffect) 
        { 
            PoolManager.Instance.Spawn(deathEffect, transform.position + deathOffset, transform.rotation, transform, team);
            /*
            var effect = Instantiate(deathEffect, transform.position + deathOffset, transform.rotation);

            var spawnDamage = effect.GetComponentsInChildren<Damage>();
            foreach (var d in spawnDamage) { d.team = team; }
            var spawnHealth = effect.GetComponentsInChildren<Health>();
            foreach (var h in spawnHealth) { h.team = team; }
            var spawnProjectile = effect.GetComponentsInChildren<Projectile>();
            foreach (var p in spawnProjectile) { p.team = team; }
            */
        }

        //var checkParent = transform.parent;
        if (mySpawn) { mySpawn.gameObject.SetActive(false); }
        else { Destroy(gameObject); }

        //if (checkParent && destroyParent) { Destroy(checkParent.gameObject); }
        //else { Destroy(gameObject); }
    }

    public void GainHealth(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
    }

    public void Regenerate()
    {
        if (currentRegenBuffer > 0f)
        {
            currentRegenBuffer -= Time.deltaTime;
        }
        else if (currentHP < maxHP)
        {
            GainHealth(passiveRegen * Time.deltaTime);
        }
    }

}
