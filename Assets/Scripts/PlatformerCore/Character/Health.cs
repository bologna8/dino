using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Tooltip("Maximum Health")] public float maxHP = 10f;
    [Tooltip("Current Health Remaining")] public float currentHP;

    public Damage.Type immunities;


    [HideInInspector] public Core myCore;
    [HideInInspector] public Spawned mySpawn;

    [Tooltip("Recolor sprite when hit")] public Color hitColor = Color.red;
    private SpriteRenderer mySprite;
    private Color startColor;

    [Tooltip("Health Bar That is shown When Damaged")] public GameObject HealthBarPrefab;

    [Tooltip("How much health gained per second while recovering")] public float passiveRegen;
    [Tooltip("Time before passive regen recovery kicks in, taking damage resets this timer")] public float regenBufferTime;
    [HideInInspector] public float currentRegenBuffer; //Count down while not taking damage
    [Tooltip("Should attacking reset the current regen buffer time")] public bool attackingResetsBuffer;

    [HideInInspector] public float stunTime;
    [HideInInspector] public float invincibleTime;
    [HideInInspector] public Movement myMovement;

    [Tooltip("Effect spawned Every Time Hit")] public GameObject hitEffect;

    [Tooltip("Will only take damage from its own team, normally other way around")] public bool friendlyFireOnly = false;

    private TrailRenderer hitTrail;
    [Tooltip("Effect spawned when destroyed")] public GameObject deathEffect;
    [Tooltip("Offset for death deffect")] public Vector3 deathOffset;

    [Tooltip("Clip played when HP reaches 0")] public AnimationClip deathAnimation;
    [Tooltip("How long death animation lasts")] public float timeToDie;

    void Awake()
    {
        if (myMovement == null) { myMovement = GetComponentInParent<Movement>(); }

        if (mySpawn == null) { mySpawn = GetComponentInParent<Spawned>(); }

        if (mySprite == null) 
        { 
            if (mySpawn) { mySprite = mySpawn.GetComponentInChildren<SpriteRenderer>(); }
            else { mySprite = GetComponentInParent<SpriteRenderer>(); }

            if (mySprite) { startColor = mySprite.color; }
        }
    }

    void OnEnable()
    {

        if (HealthBarPrefab)
        {
            var tempTeam = 0;
            Transform tempSource = null;
            if (mySpawn) { tempTeam = mySpawn.team; }
            var hpBar = PoolManager.Instance.Spawn(HealthBarPrefab, transform.position, Quaternion.identity, tempSource, tempTeam, false, true);
            var bar = hpBar.GetComponent<HealthUI>();
            if(bar) { bar.tracking = this; }
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
        if (currentHP <= 0) { return;}

        if (hitEffect) 
        {
            if (PoolManager.Instance && mySpawn) { PoolManager.Instance.Spawn(hitEffect, transform.position, transform.rotation, transform, mySpawn.team); }
            else { Instantiate(hitEffect, transform.position, Quaternion.identity); }
        }

        currentHP -= dmg;

        if (dmg > 0) { currentRegenBuffer = regenBufferTime; }

        if (currentHP <= 0) { StartCoroutine(delayedDeath()); }
        else
        {
            if (stunTime <= stun.x) 
            { 
                stunTime = stun.x; 
                StartCoroutine(flashColor(stun.x));
            }

            if (KB != Vector2.zero && myMovement) { myMovement.DoDash(KB, stun); }
        }
    }

    public IEnumerator flashColor(float time)
    {
        if (mySprite) 
        { 
            mySprite.color = hitColor;
            yield return new WaitForSeconds(time);
            mySprite.color = startColor;
        }
    }

    IEnumerator delayedDeath()
    {
        if (myCore) { myCore.Stun(timeToDie, deathAnimation); }
        yield return new WaitForSeconds(timeToDie);

        Die();
    }

    public void Die()
    {

        //Temporary simple scene reload if player dies
        //if (myCore) { if (!myCore.myAI) { SceneManager.LoadScene(SceneManager.GetActiveScene().name); } }

        //var checkParent = transform.parent;

        if (deathEffect) 
        { 
            var tempTeam = 0;
            if (mySpawn) { tempTeam = mySpawn.team; }
            PoolManager.Instance.Spawn(deathEffect, transform.position + deathOffset, transform.rotation, transform, tempTeam);

        }

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
