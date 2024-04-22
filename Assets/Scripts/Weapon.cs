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

    private Animator myAnim;

    [HideInInspector] public bool ignoreTeams;

    public GameObject attackArm;


    // Start is called before the first frame update
    void Start()
    {
        myHealth = GetComponentInChildren<Health>();
        myMovement = GetComponent<Movement>();
        myAnim = GetComponentInChildren<Animator>();

        if (damagePrefab) { changeAttack(damagePrefab); }

        //attackArm = transform.Find("Arm").gameObject;
        if (attackArm) { attackArm.SetActive(false); }

    }

    // Update is called once per frame
    void Update()
    {
        
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
            //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); mousePos.z = 0f;
            //var dir = (mousePos - transform.position).normalized;
            dir = myAim.direction;
            startAngle = Quaternion.FromToRotation(Vector3.right, dir);
            startSpot = myAim.transform.position;

            if (myAim.transform.position.x > transform.position.x) //Betta look to the left
            { if(!myMovement.faceRight) { myMovement.Turn(); } }
            else { if(myMovement.faceRight) { myMovement.Turn(); } }

            
            if (myAnim)
            {
                var offset = 0.21f;
                if ( myAim.transform.position.y > transform.position.y +offset) { myAnim.SetFloat("attackDir", 0f); }
                else if (myAim.transform.position.y < transform.position.y - offset) {myAnim.SetFloat("attackDir", 1f); }
                else { myAnim.SetFloat("attackDir", 0.5f); }
                
            }

        }

        //point Arm at Aim
        if (attackArm) 
        {
            attackArm.SetActive(true);
            //Debug.Log(startAngle);
            attackArm.transform.rotation = startAngle;
            
            //if (myMovement.faceRight) { attackArm.transform.localScale = new Vector3(1,1,1); }
            //else { attackArm.transform.localScale = new Vector3(-1,1,1); }
            
            //if (myMovement.faceRight) { attackArm.transform.rotation = startAngle; }
            //else { attackArm.transform.rotation = Quaternion.Inverse(startAngle); }
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

        //Start Actual Attack
        myHealth.TakeDamage(0, attackStats.windup + attackStats.attackDuration, preMove);
        yield return new WaitForSeconds(attackStats.windup);
        myHealth.TakeDamage(0, attackStats.attackDuration, midMove);

        var newAttack = Instantiate(damagePrefab, startSpot, startAngle).GetComponent<Damage>();
        newAttack.origin = transform;
        newAttack.offset = startSpot - transform.position;
        newAttack.team = myHealth.team;
        newAttack.ignoreTeams = ignoreTeams;
        if (!myMovement.faceRight) { newAttack.Flip(); }

        yield return new WaitForSeconds(attackStats.attackDuration);

        if (attackArm) { attackArm.SetActive(false); }

        yield return new WaitForSeconds(attackStats.cooldown);

        attackReady = true;
    }

}
