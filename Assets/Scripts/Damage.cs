using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [HideInInspector] public int team;
    [HideInInspector] public bool ignoreTeams = false;

    public float damage = 1f;
    public float stunTime = 0.1f;
    public float knockback = 100f;

    public bool onlyMoveHorizontal = true;
    public float movementPreAttack;
    public float movementMidAttack;
    public float windup = 0.1f;
    public float attackDuration = 0.1f;
    public float cooldown = 0.1f;

    [HideInInspector] public Vector3 offset;
    public float projectileSpeed;

    [Tooltip("Set == to Attack Duration if melee")] public float attackLifetime = 1f;

    private Rigidbody2D myBod;

    [HideInInspector] public Transform origin;
    [HideInInspector] public bool faceRight = true;
    private List<Health> hitList = new List<Health>();

    //Boomerang stuff
    public LayerMask hitMask;

    public bool breakable = false;
    public float returnTime;
    private float returnCurrent;
    private Vector3 rotateSpeed = new Vector3(0,0, 420);
    private bool bounced;
    private Vector3 endPosition;

    public GameObject destroyEffect;

    // Start is called before the first frame update
    void Start()
    {
        //if (!faceRight) { offset.x *= -1; }
        //transform.position += offset;

        myBod = GetComponent<Rigidbody2D>();
        if (projectileSpeed != 0f) 
        { myBod.AddForce(transform.right * projectileSpeed); }
    }

    // Update is called once per frame
    void Update()
    {
        attackLifetime -= Time.deltaTime;
        if (attackLifetime <= 0) 
        { 
            if (returnTime > 0f && !bounced) { BounceBack(); }
            else { Destroy(gameObject); }
        }

        if (projectileSpeed == 0f && origin) 
        { transform.position = origin.transform.position + offset; }

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
                hitList.Add(hitHealth);

                var dir = (other.transform.position - transform.position).normalized; //Find angle of attacker to attack
                dir.x = Mathf.Abs(dir.x); if (!faceRight) { dir.x *= -1; } //KB only in the direction the attack is facing

                hitHealth.TakeDamage(damage, stunTime, dir * knockback);

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
        if (destroyEffect) { Instantiate(destroyEffect, transform.position, Quaternion.identity); }
    }
}
