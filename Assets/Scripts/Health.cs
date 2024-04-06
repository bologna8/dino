using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [HideInInspector] public int team;
    private float respawnTime = 0.1f;
    public GameObject HealthBarPrefab;
    public float maxHP = 10f;
    public float currentHP;

    [HideInInspector] public float stunTime;
    [HideInInspector] public float invincibleTime;
    [HideInInspector] public Movement myMovement;

    public GameObject hitEffect;
    private TrailRenderer hitTrail;
    public GameObject deathEffect;

    // Start is called before the first frame update
    void Start()
    {
        if (HealthBarPrefab) 
        { Instantiate(HealthBarPrefab, GameObject.Find("Canvas").transform).GetComponent<HealthUI>().tracking = this; }

        myMovement = GetComponentInParent<Movement>();

        currentHP = maxHP;
        stunTime = respawnTime;

        hitTrail = GetComponent<TrailRenderer>();
        if (hitTrail) { hitTrail.enabled = false; }

    }

    // Update is called once per frame
    void Update()
    {
        if (invincibleTime > 0) { invincibleTime -= Time.deltaTime; }
        if (stunTime > 0) { stunTime -= Time.deltaTime; }
    }

    public void TakeDamage(float dmg, float stun, Vector2 KB)
    {
        bool getHit = true;
        if (invincibleTime > 0) { getHit = false; } //can't touch this

        if (dmg == 0f) { getHit = true; } //no damage dash movements always work
        else if (getHit && hitEffect) 
        { Instantiate(hitEffect, transform.position, Quaternion.identity); }

        if (getHit)
        {
            currentHP -= dmg;
            if (currentHP <= 0) { Die(); }
            else 
            {
                if (stunTime <= stun) { stunTime = stun; }

                if (myMovement) 
                { 
                    myMovement.myBod.velocity = Vector2.zero;
                    myMovement.myBod.AddForce(KB);
                    myMovement.momentumCurrent = 0f;
                }
            }
        }
        

    }

    public void Die()
    {
        if (deathEffect) { Instantiate(deathEffect, transform.position, Quaternion.identity); }
        
        var checkParent = transform.parent;
        if (checkParent) { Destroy(checkParent.gameObject); }
        else { Destroy(gameObject); }        
    }

    


}
