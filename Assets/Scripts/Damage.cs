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

    public float attackMovementSpeed = 1f;
    private Rigidbody2D myBod;

    [HideInInspector] public bool faceRight = true;
    private List<Health> hitList = new List<Health>();

    // Start is called before the first frame update
    void Start()
    {
        myBod = GetComponent<Rigidbody2D>();
        myBod.AddForce(transform.right * attackMovementSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        attackDuration -= Time.deltaTime;
        if (attackDuration <= 0) { Destroy(gameObject); }
    }

    public void Flip()
    {
        faceRight = !faceRight;
        //transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        //Debug.Log("left");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var hitHealth = other.gameObject.GetComponent<Health>();
        if (hitHealth && other.gameObject.layer != gameObject.layer)
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
}
