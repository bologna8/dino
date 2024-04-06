using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [HideInInspector] public Aim myAim;
    public GameObject damagePrefab;
    private Damage attackStats;
    private bool attackReady = true;
    private Health myHealth;
    private Movement myMovement;

    //[HideInInspector] public bool attacking = false;


    // Start is called before the first frame update
    void Start()
    {
        myHealth = GetComponentInChildren<Health>();
        myMovement = GetComponent<Movement>();

        if (damagePrefab) { changeAttack(damagePrefab); }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeAttack(GameObject newAttack)
    {
        attackStats = newAttack.GetComponent<Damage>();
    }

    public void tryAttack()
    {
        if (attackReady && attackStats)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    public IEnumerator AttackRoutine()
    {
        attackReady = false;

        var startSpot = transform.position;
        var dir = Vector2.right;
        var startAngle = Quaternion.FromToRotation(Vector3.right, dir);

        if (myAim)
        {
            //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); mousePos.z = 0f;
            //var dir = (mousePos - transform.position).normalized;
            dir = myAim.direction;
            startAngle = Quaternion.FromToRotation(Vector3.right, dir);
            startSpot = myAim.transform.position;

            if (myAim.transform.position.x > transform.position.x) //Betta look to the left
            { if(!myMovement.faceRight) { myMovement.Turn(); } }
            else { if(myMovement.faceRight) { myMovement.Turn(); } }
        }
        

        //Attack move directionally
        var preMove = dir * attackStats.movementPreAttack;
        var midMove = dir * attackStats.movementMidAttack;
        if (attackStats.onlyMoveHorizontal) 
        {
            preMove = new Vector3(attackStats.movementPreAttack, 0, 0);
            midMove = new Vector3(attackStats.movementMidAttack, 0, 0);
            if (!myMovement.faceRight) { preMove.x *= -1; midMove *= -1; }
        }

        //if (myMovement.faceRight) // Betta look to the left!
        //{ if (mousePos.x < transform.position.x) { myMovement.Turn(); } }
        //else 
        //{ if (mousePos.x > transform.position.x) {myMovement.Turn(); } }

        //Start Actual Attack
        myHealth.TakeDamage(0, attackStats.windup + attackStats.attackDuration, preMove);
        yield return new WaitForSeconds(attackStats.windup);
        myHealth.TakeDamage(0, attackStats.attackDuration, midMove);

        var newAttack = Instantiate(damagePrefab, startSpot, startAngle).GetComponent<Damage>();
        newAttack.origin = transform;
        newAttack.offset = startSpot - transform.position;
        newAttack.team = myHealth.team;
        if (!myMovement.faceRight) { newAttack.Flip(); }

        yield return new WaitForSeconds(attackStats.attackDuration);

        yield return new WaitForSeconds(attackStats.cooldown);

        attackReady = true;
    }

}
