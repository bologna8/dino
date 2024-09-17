using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [HideInInspector] public Core myCore;
    [HideInInspector] public Aim myAim; //Aims the direction of attacks
    [Tooltip("Prefab for actual attack goes here")] public GameObject attackPrefab;
    
    private Attack attackStats; //current Attack script
    [HideInInspector] public bool attackReady = true; //ready to use attack again after attack fiinished and cooldown done
    [HideInInspector] public bool attackHeld; //Is input for attack still being done
    private int currentClip; //Number of attacks remaining in the current clip
    private float currentSpreadTime; //How long has attack been used in a row
    private bool spreadNegative; //Used to make random spread bounce back and forth positive and negative


    [HideInInspector] public float verticalInput;
    [Tooltip("Prefab for up variation")] public GameObject upAttackPrefab;
    [Tooltip("Prefab for down variation")] public GameObject downAttackPrefab;
    private GameObject currentAttackPrefab;

    //other refrences
    //private Health myHealth;
    private Movement myMovement;
    //private Animator myAnim; //needs to be reworked tbh

    public bool ignoreTeams;


    void Awake()
    {
        //if (myHealth == null) { myHealth = GetComponentInChildren<Health>(); }
        if (myMovement == null) { myMovement = GetComponent<Movement>(); }
        //if (myAnim == null) { myAnim = GetComponentInChildren<Animator>(); }

        if (attackPrefab) { changeAttack(attackPrefab); }
    }

    void Update()
    {
        if (attackStats)
        {
            if (attackReady) 
            {
                if (currentSpreadTime > 0) { currentSpreadTime -= Time.deltaTime; }
                else if (myAim) { myAim.turnSpeedMultiplier = 1; }

                if (attackStats.typeOfInput == Attack.AttackType.Automatic && attackHeld)
                { TryAttack(); }

                if (attackStats.typeOfInput == Attack.AttackType.Unload)
                { 
                    if (currentClip < attackStats.clipSize) { TryAttack(); }
                    else if (myAim) { myAim.turnSpeedMultiplier = 1; }
                }

                //if (myAim) { myAim.attackAngleOffset = 0f; }
            } 
            else
            {
                if (currentSpreadTime < attackStats.spreadTime) { currentSpreadTime += Time.deltaTime;}
            }

            currentSpreadTime = Mathf.Clamp(currentSpreadTime, 0, attackStats.spreadTime);
        }
        
    }

    public void setNewAttacks(GameObject newBaseAttack, GameObject newUpAttack = null, GameObject newDownAttack = null)
    {
        attackPrefab = newBaseAttack;
        upAttackPrefab = newUpAttack;
        downAttackPrefab = newDownAttack;
        changeAttack(newBaseAttack);
    }

    public void changeAttack(GameObject newAttack)
    {
        currentAttackPrefab = newAttack;
        attackStats = newAttack.GetComponent<Attack>();
        currentClip = attackStats.clipSize;

    }

    public IEnumerator Reload()
    {
        attackReady = false;

        yield return new WaitForSeconds(attackStats.cooldown);

        currentClip = attackStats.clipSize;
        currentSpreadTime = 0;

        attackReady = true;
    }

    public void TryAttack()
    {
        if (attackReady && attackStats)
        {
            var useAttack = attackPrefab;

            if (verticalInput > 0 && upAttackPrefab) { useAttack = upAttackPrefab; }
            if (verticalInput < 0 && downAttackPrefab) { useAttack = downAttackPrefab; }


            if (currentAttackPrefab != useAttack) { changeAttack(useAttack); }


            if (attackStats.typeOfAmo == Attack.AmoType.none) 
            { StartCoroutine(AttackRoutine()); }
            else if (currentClip > 0)
            {
                currentClip --;
                StartCoroutine(AttackRoutine());
            }
            
        }
    }

    public IEnumerator AttackRoutine()
    {
        attackReady = false;

        var startSpot = transform.position;
        var dir = Vector3.right;
        var startAngle = Quaternion.FromToRotation(Vector3.right, dir);

        if (myCore && attackStats.attackAnim) { myCore.ChangeAttackAnimation(attackStats.attackAnim); }

        var safety = false; //don't spawn any porjectiles if aim cursor is touching something like a wall

        //if (myAnim) { myAnim.SetTrigger("attack"); }

        if (myAim)
        {
            myAim.turnSpeedMultiplier = attackStats.attackingAimSpeed;
            myAim.attackAngleOffset = attackStats.startAimAngleOffset;

            dir = myAim.currentDirection;

            startAngle = Quaternion.FromToRotation(Vector3.right, dir);
            startSpot = myAim.transform.position;

            /*
            if (myCore) //Turn to attack if not already facing that way
            {
                if (myAim.transform.position.x > transform.position.x) //Betta look to the left
                { if(!myCore.faceRight) { myCore.Turn(); } }
                else if(myCore.faceRight) { myCore.Turn(); }
            }
            */
            
            if(myAim.touching) { safety = true; }
        }


        var startOffset = startSpot - transform.position;
        

        //Attack move directionally
        var preMove = dir * attackStats.movementPreAttack;
        var midMove = dir * attackStats.movementMidAttack;
        if (attackStats.onlyHorizontalMovePre) 
        {
            preMove = new Vector3(attackStats.movementPreAttack, 0, 0);
            if(myCore) { if (!myCore.lookingRight) { preMove.x *= -1; } }
        }

        if (attackStats.onlyHorizontalMoveMid)
        {
            midMove = new Vector3(attackStats.movementMidAttack, 0, 0);
            if(myCore) { if (!myCore.lookingRight) { midMove *= -1; } }
        }

        //Debug.Log(midMove);

        //Start Actual Attack
        if (!attackStats.moveWhileAttacking && myMovement && attackStats.windup > 0) 
        { myMovement.DoDash(preMove, new Vector2(attackStats.windup, 0f)); }
        
        yield return new WaitForSeconds(attackStats.windup);
        
        if (!attackStats.moveWhileAttacking && myMovement && attackStats.attackDuration > 0) 
        { myMovement.DoDash(midMove, new Vector2(attackStats.attackDuration, 0f)); }


        if (!safety) //Don't spawn anything in walls
        { 
            for (int i = 0; i < attackStats.multiplier; i++)
            { SpawnAttack(i, startSpot, startOffset, startAngle, dir); }
        }

        yield return new WaitForSeconds(attackStats.attackDuration);

        if (myAim) { myAim.attackAngleOffset = 0f; }

        if (currentClip <= 0) { StartCoroutine(Reload()); }
        else 
        {
            yield return new WaitForSeconds(attackStats.fireRate);
            attackReady = true;
        }

    }

    public void SpawnAttack(int i, Vector3 startSpot, Vector3 startOffset, Quaternion startAngle, Vector3 dir)
    {
        //var attackRight = true;
        //if (myMovement) { if (!myMovement.faceRight) {attackRight = false; } }

        //var newAttack = Instantiate(attackPrefab, startSpot, startAngle).GetComponent<Attack>();
        //newAttack.InitializeAttack(transform, startOffset, myHealth.team, attackRight, ignoreTeams);

        bool attackRight = true;
        if (dir.x < 0) { attackRight = false; }

        //Add rotation based on spread angle

        var spread = attackStats.spreadIntensity;
        var percent = 0f;

        if (attackStats.spreadTime > 0) 
        {
            percent = currentSpreadTime/attackStats.spreadTime;    
            spread *= attackStats.spreadCurve.Evaluate(percent);         
        }
    
        if (attackStats.repeatPattern > 0) 
        { 
            percent = ((((float)attackStats.clipSize - (float)currentClip) * (float)attackStats.repeatPattern) / (float)attackStats.clipSize); //gotta float
            while (percent > 1) { percent--; } //Percent can't be more than 1, repeat pattern throughout the clip
            spread *= attackStats.spreadCurve.Evaluate(percent); 
        }
        

        var fireAngle = startAngle;
        if (spread != 0)
        {
            spread = Random.Range(spread * attackStats.spreadRange, spread);
            if (attackStats.alternateNegative) //Alternate sides every other shot
            {
                if (spreadNegative) { spread *= -1; }
                spreadNegative = !spreadNegative;
            }

            if (attackStats.spreadEvenly && attackStats.multiplier > 1) 
            {
                spread = -(Mathf.Abs(spread));
                if (i > 0) { spread += ((Mathf.Abs(spread) * 2) / (attackStats.multiplier - 1)) * i; }
            }
            else if (!attackRight) { spread *= -1;} //Make sure top and bottom stay same on left and right

            var r = fireAngle.eulerAngles;

            fireAngle = Quaternion.Euler(r.x, r.y, r.z + spread);
        }

        if (attackStats.startAimAngleOffset != 0)
        {
            var r = fireAngle.eulerAngles;
            if (r.z > 90 && r.z < 270) { fireAngle = Quaternion.Euler(r.x, r.y, r.z - attackStats.startAimAngleOffset); }
            else { fireAngle = Quaternion.Euler(r.x, r.y, r.z + attackStats.startAimAngleOffset); }

        }


        GameObject newAttack = null;
        if (currentAttackPrefab && myAim && myCore)
        {
            newAttack = PoolManager.Instance.Spawn(currentAttackPrefab, startSpot, fireAngle, myAim.transform, myCore.team);
        }
        else { Debug.Log("ding, this shoudln't be happening, attack error"); }
        


        if (attackStats.momentumLaunch && newAttack) 
        {
            var launchV = Vector2.zero;
            var myV = GetComponent<Rigidbody2D>().velocity;

            if (myV.x > 0 && dir.x > 0) { launchV.x = myV.x; }
            if (myV.x < 0 && dir.x < 0) { launchV.x = myV.x; }

            if (myV.y > 0 && dir.y > 0) { launchV.y = myV.y; }
            if (myV.y < 0 && dir.y < 0) { launchV.y = myV.y; }

            newAttack.GetComponent<Rigidbody2D>().velocity = launchV;
        }
    }



}
