using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [HideInInspector] public Aim myAim; //Aims the direction of attacks
    [Tooltip("Prefab for actual attack goes here")] public GameObject damagePrefab;
    private Damage attackStats; //current damage script
    private bool attackReady = true; //ready to use attack again after attack fiinished and cooldown done

    //other refrences
    private Health myHealth;
    private Movement myMovement;
    private Animator myAnim; //needs to be reworked tbh

    [HideInInspector] public bool ignoreTeams;


    // Start is called before the first frame update
    void Start()
    {
        myHealth = GetComponentInChildren<Health>();
        myMovement = GetComponent<Movement>();
        myAnim = GetComponentInChildren<Animator>();

        if (damagePrefab) { changeAttack(damagePrefab); }
    }

    public void changeAttack(GameObject newAttack)
    {
        damagePrefab = newAttack;
        attackStats = newAttack.GetComponent<Damage>();
    }

    public void tryAttack()
    {
        if (attackReady && attackStats && damagePrefab)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    public IEnumerator AttackRoutine()
    {
        attackReady = false;

        var startSpot = transform.position;
        var dir = Vector3.right;
        var startAngle = Quaternion.FromToRotation(Vector3.right, dir);

        if (myAnim) { myAnim.SetTrigger("attack"); }

        if (myAim)
        {
            dir = myAim.direction;
            startAngle = Quaternion.FromToRotation(Vector3.right, dir);
            startSpot = myAim.transform.position;

            if (myAim.transform.position.x > transform.position.x) //Betta look to the left
            { if(!myMovement.faceRight) { myMovement.Turn(); } }
            else { if(myMovement.faceRight) { myMovement.Turn(); } }

            
            if (myAnim)
            {
                var offset = 0.21f; //check rough angle of attack
                if ( myAim.transform.position.y > transform.position.y + offset) { myAnim.SetFloat("attackDir", 0f); }
                else if (myAim.transform.position.y < transform.position.y - offset) {myAnim.SetFloat("attackDir", 1f); }
                else { myAnim.SetFloat("attackDir", 0.5f); }
                
            }

        }

        var startOffset = startSpot - transform.position;
        

        //Attack move directionally
        var preMove = dir * attackStats.movementPreAttack;
        var midMove = dir * attackStats.movementMidAttack;
        if (attackStats.onlyHorizontalMovePre) 
        {
            preMove = new Vector3(attackStats.movementPreAttack, 0, 0);
            if (!myMovement.faceRight) { preMove.x *= -1; }
        }
        if (attackStats.onlyHorizontalMoveMid)
        {
            midMove = new Vector3(attackStats.movementMidAttack, 0, 0);
            if (!myMovement.faceRight) { midMove *= -1; }
        }

        //Start Actual Attack
        if (!attackStats.moveWhileAttacking) { myMovement.DoDash(preMove, attackStats.windup); }
        
        yield return new WaitForSeconds(attackStats.windup);
        
        if (!attackStats.moveWhileAttacking) { myMovement.DoDash(midMove, attackStats.attackDuration); }

        var newAttack = Instantiate(damagePrefab, startSpot, startAngle).GetComponent<Damage>();
        newAttack.origin = transform;
        newAttack.offset = startOffset;
        newAttack.team = myHealth.team;
        newAttack.ignoreTeams = ignoreTeams;
        if (!myMovement.faceRight) { newAttack.Flip(); }

        yield return new WaitForSeconds(attackStats.attackDuration);

        yield return new WaitForSeconds(attackStats.cooldown);

        attackReady = true;
    }

}
