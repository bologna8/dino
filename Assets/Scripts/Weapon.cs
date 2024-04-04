using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject damagePrefab;
    private Damage attackStats;
    private bool attackReady = true;
    private Health myHealth;
    private Movement myMovement;

    //[HideInInspector] public bool attacking = false;


    // Start is called before the first frame update
    void Start()
    {
        myHealth = GetComponent<Health>();
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

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); mousePos.z = 0f;
        var dir = mousePos - transform.position;
        var startAngle = Quaternion.FromToRotation(Vector3.right, dir);

        if (mousePos.x > transform.position.x) //
        { if(!myMovement.faceRight) { myMovement.Turn(); } }
        else { if(myMovement.faceRight) { myMovement.Turn(); } }

        //Turn if mouse is on other side and move accordingly
        var preMove = attackStats.movementPreAttack;
        var midMove = attackStats.movementMidAttack;
        if (myMovement.faceRight)
        { if (mousePos.x < transform.position.x) {myMovement.Turn(); } }
        else //Facing left
        {
            if (mousePos.x > transform.position.x) {myMovement.Turn(); }
            preMove.x *= -1; midMove.x *= -1;
        }

        //Start Actual Attack
        myHealth.TakeDamage(0, attackStats.windup + attackStats.attackDuration, preMove);
        yield return new WaitForSeconds(attackStats.windup);
        myHealth.TakeDamage(0, attackStats.attackDuration, midMove);

        var newAttack = Instantiate(damagePrefab, transform.position, startAngle).GetComponent<Damage>();
        newAttack.origin = transform;
        newAttack.gameObject.layer = gameObject.layer;
        if (!myMovement.faceRight) { newAttack.Flip(); }

        yield return new WaitForSeconds(attackStats.attackDuration);

        yield return new WaitForSeconds(attackStats.cooldown);

        attackReady = true;
    }

}
