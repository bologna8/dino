using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public float damage = 1f;
    public float stunTime = 0.1f;
    public float knockback = 100f;

    public Vector2 movementPreAttack;
    public float windup = 0.1f;
    public Vector2 movementMidAttack;
    public float attackDuration = 0.1f;
    public float cooldown = 0.1f;

    public Vector3 offset;
    public float projectileSpeed;

    [Tooltip("Set == to Attack Duration if melee")] public float attackLifetime = 1f;

    private Rigidbody2D myBod;

    [HideInInspector] public Transform origin;
    [HideInInspector] public bool faceRight = true;
    private List<Health> hitList = new List<Health>();

    //Boomerang stuff
    public LayerMask bounceMask;
    public float returnTime;
    private float returnCurrent;
    private Vector3 rotateSpeed = new Vector3(0,0, 420);
    private bool bounced;
    private Vector3 endPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (!faceRight) { offset.x *= -1; }
        transform.position += offset;

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
        if (other.gameObject.layer != gameObject.layer)
        {
            var hitHealth = other.gameObject.GetComponent<Health>();
            if (hitHealth)
            {
                if (!hitList.Contains(hitHealth))
                {
                    hitList.Add(hitHealth);

                    var dir = (other.transform.position - transform.position).normalized; //Find angle of attacker to attack
                    dir.x = Mathf.Abs(dir.x); if (!faceRight) { dir.x *= -1; } //KB only in the direction the attack is facing

                    hitHealth.TakeDamage(damage, stunTime, dir * knockback);
                }
            }
        }
        else if (bounced) { Destroy(gameObject); }

        if ((bounceMask.value & (1 << other.gameObject.layer)) > 0 && !bounced)
        { BounceBack(); }
    }
}
