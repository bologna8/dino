using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Vector2 myVelocity; //Control rigidbody velocity directly for tight, snappy, crispy, crunchy code

    [Header("MOVE VARIABLES")]
    [Tooltip("Horizontal speed while at maximum momentum")] public float baseSpeed = 10f;
    [Tooltip("Multiply speed while in the air")] public float airSpeedMultiplier = 1f;
    [Tooltip("Percent of speed 0 to 1 as you accelerate / deccelerate ")] public AnimationCurve moveSpeedCurve;
    [Tooltip("Time it takes to reach max speed")] public float accelerationTime = 1f;
    [Tooltip("Time it takes to slow to a stop")] public float deccelerationTime = 1f;
    [HideInInspector] public float momentum; //current value between idle(0), walk, sprint(1)
    [HideInInspector] public float moveInput; //Recieve horizontal input from player / ai
    [HideInInspector] public float verticalInput; //Recieve vertical input from player / ai
    [HideInInspector] public bool faceRight = true; //false = left
    [HideInInspector] public float slowPercent = 1f; //between 0 and 1
    [HideInInspector] public bool turning = false; //true while mid turn
    [Tooltip("Time it takes to flip left / right")] public float turnTime = 0.1f;
    
    
    

    [Header("JUMP VARIABLES")]
    [Tooltip("Initial jump velocity")] public float jumpForce = 10f;
    [Tooltip("Maximum amount of time for each jump input")] public float maxJumpTime = 1f;
    [Tooltip("Minimum amount of time for each jump input")] public float minJumpTime = 0.1f;
    private float currentJumpTime; //Amount of time jump button has been held
    [HideInInspector] public bool jumpInput; //Recive jump button input
    [HideInInspector] public bool onGround = false; //Feat on the ground and head in the air
    private bool justJumped; //detect if new jump input
    [Tooltip("Vertical velocity percent 0 to 1 over time as jump input is held")] public AnimationCurve jumpSpeedCurve;
    private float jumpDelayTime = 0.1f; //time Before you can Start a New Jump
    private float jumpDelayCurrent; //current delay until can jump again
    [Tooltip("Downard velocity while in air")] public float myGravity = 1f;
    [Tooltip("Constant downward force, even when grounded, helps with slopes")] public float minGravConstant = 0f;
    [Tooltip("Max gravity fall speed")] public float terminalVelocity = 10f;
    private float airTime; //how long been in air, used for gravity calculation and coyote time
    [Tooltip("Extra time to try and jump just after leaving the ground")] public float coyoteTime;
    [Tooltip("Bouncin off the walls")] public bool wallJump = false;
    [Tooltip("Horizontal and vertical direction from wall jumps")] public Vector2 wallJumpForce;
    [Tooltip("How long wall jumps last")] public float wallJumpTime;
    [Tooltip("Extra mid air jumps")] public int bonusJumps = 0;
    private int remainingJumps; //Keep track of remaining bonus jumps
    [Tooltip("Spawn effect on every jump")] public GameObject jumpEffect;

    

    [Header("LEDGE VARIABLES")]
    [Tooltip("Can ya hang?")] public bool grabLedges = false;
    [Tooltip("Horizontal and Vertical speed of ledge climb")] public Vector2 climbForce;
    [Tooltip("X is delay before actual cimb time of Y")] public Vector2 climbTime;
    private float hangTime; //How long been on ledge, used for slight delay before climbing up
    private float hangClimbDelay = 0.3f; //How long before you can climb input on ledge
    [HideInInspector] public bool onEdge; //Currently grabbing an edge
    [HideInInspector] public Coroutine climbing; //currently started a climb coroutine

    [Tooltip("Climb small hip high obstacles easily")] public bool autoVaultOver = false;
    [Tooltip("Horizontal and Vertical speed of vault")] public Vector2 vaultForce;
    [Tooltip("Time spent vaulting")] public float vaultSpeed;
    [Tooltip("Change fall speed while pressing into a wall")] public bool wallSlide = false;
    [Tooltip("Max fall speed while wall sliding")] public float slideSpeed = 1f;



    [Header("DASH VARIABLES")]
    [Tooltip("Time after doing a dash before ready to dash again")] public float dashCooldown = 0.4f;
    private float dashCurrentCD; //Keep track of current dash cooldown
    [Tooltip("Dash stops instantly before going over edges")] public bool edgeStopDash = true;
    [Tooltip("Collision layer is changed while dashing")] public bool changeCollisionLayer = true;
    private int startLayer; //remember to return to starting layer later
    [Tooltip("Hitbox layer is changed while dashing")] public bool changeHitboxLayer = false;
    private int startLayerHP; //remember to return hitbox to its tarting later;
    
    [Range(0, 30)] [Tooltip("Layer while dashing on ground")] public int dashLayer;
    [Tooltip("Speed while dashing")] public float dashForce = 20f;
    [Tooltip("Duration time of dash")] public float dashTime = 0.4f;

    [Range(0, 30)] [Tooltip("Layer while dashing on ground")] public int airDashLayer;

    [Tooltip("Speed while dashing in air")] public float airDashForce = 20f;
    [Tooltip("Duration time of air dash")] public float airDashTime = 0.1f;
    [Tooltip("Affected by gravity while dashing in air")] public bool gravityWhileAirDashing = false;
    private bool ignoreGravity = false; //Disable gravity during dashes
    [Tooltip("Number of times you can dash while mid air")] public int airDashes = 0;
    private int remainingDashes; //number of air dashes you have left
    
    [HideInInspector] public IEnumerator dashing; //Current dash coroutine, all knockbacks are a new dash technically
    [Tooltip("Spawn effect when doing a dash")] public GameObject dashEffect;
    private GameObject dashingEffect; //keep track of current dash effect to delete at end of dash




    //OTHER VARIABLES
    private Health myHP; //Check stun times and change layer during dashes
    [HideInInspector] public Animator myAnim; //animation handling needs to be cleaned up...
    [HideInInspector] public Rigidbody2D myBod; //nice bod
    private Collider2D myCollider; //Collider used by this bod


    //Colliders and such, should be under the "Self" game object in inspector
    private GameObject mySelf; //must contain all colliders and any sprites to rotate when flipped, Health hitbox should be seperate
    [HideInInspector] public LayerCheck[] colliders; //Check colliders must be in this order in inspector:
    [HideInInspector] public LayerCheck groundCheck; //0
    [HideInInspector] public LayerCheck frontCheck; //1
    [HideInInspector] public LayerCheck ledgeCheck; //2
    [HideInInspector] public LayerCheck airCheck; //3
    [HideInInspector] public LayerCheck forwardCheck; //4
    [HideInInspector] public LayerCheck hipCheck; //5
    [HideInInspector] public LayerCheck headCheck; //6
    [HideInInspector] public LayerCheck passCheck; //7
    [HideInInspector] public LayerCheck dashCheck; //8




    void Start()
    {
        myBod = GetComponent<Rigidbody2D>();
        //myBod.gravityScale = 0; //turn off rigidbody gravity

        myHP = GetComponentInChildren<Health>();
        myAnim = GetComponentInChildren<Animator>();

        startLayer = gameObject.layer; //remember for later
        if (myHP) { startLayerHP = myHP.gameObject.layer; }

        myCollider = GetComponent<Collider2D>();
        mySelf = transform.Find("Self").gameObject;
        colliders = mySelf.transform.GetComponentsInChildren<LayerCheck>();
        if (colliders.Length > 0) { groundCheck = colliders[0]; }
        if (colliders.Length > 1) { frontCheck = colliders[1]; }
        if (colliders.Length > 2) { ledgeCheck = colliders[2]; }
        if (colliders.Length > 3) { airCheck = colliders[3]; }
        if (colliders.Length > 4) { forwardCheck = colliders[4]; }
        if (colliders.Length > 5) { hipCheck = colliders[5]; }
        if (colliders.Length > 6) { headCheck = colliders[6]; }
        if (colliders.Length > 7) { passCheck = colliders[7]; }
        if (colliders.Length > 8) { dashCheck = colliders[8]; }

    }

    void FixedUpdate()
    {
        if (groundCheck) { onGround = groundCheck.touching; }
        
        if (myAnim) { myAnim.SetBool("onGround", onGround); }

        UpdateTimers();
        UpdateInputs();

        if (grabLedges) { ledgeGrab(); } //ledge grab if allowed

        if (myAnim) { myAnim.SetFloat("moveValue", momentum); }

        UpdateGravity();

        myBod.velocity = myVelocity;
        //myBod.velocity = new Vector3(myVelocity.x, 0,0);
        
    }

    public void UpdateTimers()
    {
        if (jumpDelayCurrent > 0) { jumpDelayCurrent -= Time.deltaTime; }
        if (dashCurrentCD > 0 && dashing == null) { dashCurrentCD -= Time.deltaTime; }

        if (onGround || onEdge) //Reset stuff
        { 
            if (myCollider.enabled) //collider disabled while passing through platform
            {
                airTime = 0f;
                remainingJumps = bonusJumps; remainingDashes = airDashes; 
            }
            if (onEdge) { airTime = coyoteTime; } //Can't coyote jump just after being on an edge
            else if (myVelocity.y < 0 && myCollider.enabled) { myVelocity.y = 0; } //reset velocity on ground and not dropping through a platform
        } 
        else { airTime += Time.deltaTime; } //Keep track of air time for gravity and coyote jumps
    }

    public void UpdateInputs()
    {
        var canInput = true;
        if (myHP) { if (myHP.stunTime > 0) { canInput = false; } }
        if (dashing != null || climbing != null) { canInput = false; }

        if (canInput)
        {
            Run(); //Run Run

            //JumpStart is called seperatly on button down
            if (justJumped) //Handle if jump button just pressed is being held or released
            {
                if (jumpInput) { JumpHold(); }
                else { JumpEnd(); }
            }
        }
        else 
        { 
            momentum = 0;
            if (justJumped) { JumpEnd(); }
        }
        
    }

    public void UpdateGravity()
    {
        bool applyGravity = true;
        if (onEdge || ignoreGravity) { applyGravity = false; }

        if (applyGravity)
        {
            myVelocity.y -= myGravity * Mathf.Pow(minGravConstant + airTime, 2); //G = C * (m/s)^2
            myVelocity.y = Mathf.Clamp(myVelocity.y, -terminalVelocity, Mathf.Infinity);
        }
    }


    void Run()
    {
        if (moveInput < 0 && faceRight) { Turn(); }
        if (moveInput > 0 && !faceRight) { Turn(); }     

        if (frontCheck)
        {
            if (frontCheck.touching) //When run into a wall
            {
                momentum = 0;
                if (wallSlide && InputForward()) //slide gives a min slide speed
                { myVelocity.y = Mathf.Clamp(myVelocity.y, -slideSpeed, Mathf.Infinity); } 
            }
            else //Increas or decrease momentum gradually
            { 
                if (moveInput == 0) //Slow down when not inputing
                { 
                    if (deccelerationTime <= 0) { momentum = 0; }
                    else { momentum -= Time.deltaTime / deccelerationTime; } 
                }
                else //If moving, accelerate speed
                {
                    if (accelerationTime <= 0) { momentum = 1; }
                    else { momentum += Time.deltaTime / accelerationTime; } 
                }
            } 
        }
        else { momentum = moveInput; } //Just wack edge case if no collider, still moves snappy

        if (InputForward() && autoVaultOver) { VaultOver(); } //Jump over objects
        
        //Slow percent sets maximum to less than 1 if slowed
        momentum = Mathf.Clamp(momentum, 0, slowPercent);

        myVelocity.x = baseSpeed * moveSpeedCurve.Evaluate(momentum);
        if (!onGround) { myVelocity.x *= airSpeedMultiplier; } //mid air speed multiplier
        if (!faceRight) { myVelocity.x *= -1; } //Step to the left

    }

    public void Turn()
    {
        if (!turning)
        {
            faceRight = !faceRight;
            momentum = 0f;

            StartCoroutine(TurnAround());
        } 
    }

    private IEnumerator TurnAround() //bright eyes
    {
        turning = true;

        var fromAngle = mySelf.transform.rotation;
        var toAngle = Quaternion.Euler(mySelf.transform.eulerAngles + new Vector3(0, 180, 0));

        if (turnTime <= 0f) { yield return null; }
        else //turn over a set period of time, or instantly if 0
        {
            for (var t = 0f; t <= 1; t += Time.deltaTime / turnTime)
            {
                mySelf.transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
                yield return null;
            }
        }

        mySelf.transform.rotation = toAngle;
        turning = false;
    }

    public bool InputForward() //True if input is same direction as you are facing
    {
        if (faceRight && moveInput > 0) { return true; }
        if (!faceRight && moveInput < 0) { return true; }
        return false;
    }



    //JUMP, JUMP, JUMP AROUND
    public void JumpStart()
    {
        if (jumpDelayCurrent <= 0 && dashing == null)
        {
            bool jumpDown = false; //Check if trying to jump down through a pass platform
            if (passCheck) { if (onGround && passCheck.touching && verticalInput < 0) { jumpDown = true; } }

            bool jumpReady = false;

            if (jumpDown) { StartCoroutine(drop()); }
            else if (onGround) { jumpReady = true; } //Normal jump
            else if (onEdge) //Climb up edges
            { 
                if(climbing == null && hangTime > hangClimbDelay) { climbing = StartCoroutine(ledgeClimb()); } 
            }
            else if (frontCheck.touching && !turning && wallJump) //wall jumps are just lil dashes
            { 
                Turn(); 
                var dir = wallJumpForce; 
                if (!faceRight) { dir.x *= -1; }
                DoDash(dir, wallJumpTime);
                if (jumpEffect) { Instantiate(jumpEffect, groundCheck.transform.position, Quaternion.identity); }
            } 
            else if (airTime < coyoteTime) { jumpReady = true; } //jump if just recently left the ground
            else if (remainingJumps > 0)
            {
                remainingJumps --;
                jumpReady = true;
            }

            //Actually do the jumping
            if (jumpReady)
            {
                myVelocity.y = jumpForce;
                
                jumpDelayCurrent = jumpDelayTime;
                justJumped = true;

                if (myAnim) { myAnim.SetTrigger("jumped"); }

                if (jumpEffect) { Instantiate(jumpEffect, groundCheck.transform.position, Quaternion.identity); }
            }
            
        }
        
    }

    public void JumpHold()
    {
        currentJumpTime += Time.deltaTime;
        myVelocity.y = jumpForce * jumpSpeedCurve.Evaluate(currentJumpTime / maxJumpTime);
        airTime = coyoteTime;
        
        if (currentJumpTime > maxJumpTime) { JumpEnd(true); }
        if (headCheck) { if(headCheck.touching) { JumpEnd(true); } }  //end early if hit head
    }

    public void JumpEnd(bool mustEnd = false) //true mean jump is hard canceled by other means
    {
        if (currentJumpTime < minJumpTime && !mustEnd) { JumpHold(); } //minimum jump time should keep jumping
        else
        {
            myVelocity.y = 0;
            justJumped = false;
            currentJumpTime = 0;
        }
    }

    public IEnumerator drop()
    {
        bool stillOnPlatform = true;
        while (passCheck.touching) 
        {
            if (stillOnPlatform) 
            { 
                if(!onGround) { stillOnPlatform = false; }
            }
            else if (onGround) { break; }

            myCollider.enabled = false;
            myVelocity.y = -jumpForce;
            jumpDelayCurrent = jumpDelayTime;
            yield return null;
        }
        
        myCollider.enabled = true;
    }



    //CLIMB TIME!
    private void ledgeGrab()
    {
        bool tryGrab = true;     
        if (turning) { tryGrab = false; }
        if (jumpDelayCurrent > 0) { tryGrab = false; }
        if (myHP) { if (myHP.stunTime > 0) { tryGrab = false; } }
        
        if(climbing != null) { tryGrab = true; } //stay on ledge while ledge climb started
        
        if (!onGround && ledgeCheck.touching && !airCheck.touching && tryGrab)
        {
            if (verticalInput < 0) { jumpDelayCurrent = jumpDelayTime * 2; myVelocity.y = -jumpForce; } //Drop if pressing down
            else //Actually grab that ledge
            {
                onEdge = true;
                airTime = coyoteTime; //Reset airtime while ya hang
                momentum = 0;
                myVelocity.y = 0;
                
                if(dashing != null) { StopCoroutine(dashing); dashing = null; } //cancel dash if ledge grabbed

                var cornerHit = ledgeCheck.findTopCorner(faceRight);
                var offset = ledgeCheck.transform.localPosition;
                var checkParent = ledgeCheck.transform.parent;
                if (checkParent) { offset += checkParent.transform.localPosition; }

                if (!faceRight) { offset = new Vector2(offset.x *-1, offset.y); }
                transform.position = cornerHit - offset;

                hangTime += Time.deltaTime;
            
                //Climb up edge with move inputs if up or in direction of edge
                if (climbing == null && hangTime > hangClimbDelay) 
                { 
                    if (verticalInput > 0) { climbing = StartCoroutine(ledgeClimb()); }
                    if (InputForward()) { climbing = StartCoroutine(ledgeClimb()); }
                }
            }

        }
        else {  onEdge = false; hangTime = 0f; }

        if (myAnim) { myAnim.SetBool("onEdge", onEdge); }

    }

    public IEnumerator ledgeClimb()
    {
        if (myAnim) { myAnim.SetTrigger("climbed"); }

        var climbDir = climbForce;
        bool straightUp = true;
        if (moveInput != 0) { straightUp = false; }
        
        yield return new WaitForSeconds(climbTime.x);

        //Check again for any move input
        if (moveInput != 0) { straightUp = false; }
        
        if (!straightUp) //check if changed move input mid climb
        {
            if (faceRight && moveInput < 0) { straightUp = true; }
            if (!faceRight && moveInput > 0) { straightUp = true; }
        }

        if (onEdge) //check still on edge
        {
            jumpDelayCurrent = jumpDelayTime;
            if (jumpEffect) { Instantiate(jumpEffect, ledgeCheck.transform.position, Quaternion.identity); }

            if (straightUp) { climbDir.x = 0; } 
            else if (!faceRight) { climbDir.x *= -1; } //climb left
            
            DoDash(climbDir, climbTime.y);
        }

        climbing = null;
    }
    

    public void VaultOver() //For smaller, chest high walls
    {
        if (hipCheck && ledgeCheck && airCheck)
        {
            if (hipCheck.touching && !ledgeCheck.touching && !airCheck.touching && dashing == null && !turning)
            {
                var vaultDir = new Vector2(vaultForce.x * moveInput, vaultForce.y);
                DoDash(vaultDir, vaultSpeed);
            }
        }
    }



    //DASHING DARLING
    public void Dash()
    {
        bool canDash = true;
        if (dashCurrentCD > 0) { canDash = false; }
        if (!onGround && remainingDashes < 1) { canDash = false; } //check if have enough air dashes
        if (dashing != null) { canDash = false; }

        if (canDash) 
        {
            dashCurrentCD = dashCooldown + dashTime;

            var d = Vector2.right * dashForce; //direction and speed to dash
            var t = dashTime; //duration of dash
            var noGrav = false;
            var edgeStop = edgeStopDash;

            int changeLayer = -1; //negative means don't change
            if (changeCollisionLayer) { changeLayer = dashLayer; }
            int changeHP = -1; //change the health layer
            if (changeHitboxLayer && myHP) { changeHP = dashLayer; } 
            
            if (!onGround) //different if air dashing
            { 
                d = Vector2.right * airDashForce;
                t = airDashTime;
                noGrav = !gravityWhileAirDashing;
                edgeStop = false;
                remainingDashes --;

                if (changeCollisionLayer) { changeLayer = airDashLayer; }
                if (changeHitboxLayer && myHP) { changeHP = airDashLayer; } 
            }
            
            if (!faceRight) { d *= -1; } //change direction to left

            DoDash(d, t, noGrav, edgeStop, true, changeLayer, changeHP);
            dashingEffect = Instantiate(dashEffect, transform); //spawn dash trail as child obect

            if (myAnim) { myAnim.SetTrigger("dashed"); }
        }
    }

    public void DoDash(Vector2 dir, float duration, bool noGrav = false, bool edgeStop = false, bool wallCheck = false, int newLayer = -1, int layerHP = -1)
    {
        if (dashing != null) { StopCoroutine(dashing); }
        dashing = DirectionalDash(dir, duration, noGrav, edgeStop, wallCheck, newLayer, layerHP);
        StartCoroutine(dashing);
    }

    public IEnumerator DirectionalDash(Vector2 dir, float duration, bool noGrav = false, bool edgeStop = false, bool wallCheck = false, int newLayer = -1, int layerHP = -1)
    {
        myVelocity = dir;
        airTime = coyoteTime;
        ignoreGravity = noGrav;

        if (newLayer >= 0) { gameObject.layer = newLayer; }
        if (layerHP >= 0) { myHP.gameObject.layer = layerHP; }

        for (var t = 0f; t <= 1; t += Time.deltaTime / duration)
        {
            bool endEarly = false; //end early if dashing into wall or dashing off an edge

            if (forwardCheck && edgeStop) //stop if about to dash off an edge
            { if (!forwardCheck.touching && onGround) { endEarly = true; } }

            if (dashCheck && newLayer >= 0) //keep dashing if still stuck in dashable objects
            { 
                if(dashCheck.touching && onGround) 
                { 
                    if (t >= 0.9f) { t = 0.9f; } //Extend time indefinitely
                    if (endEarly) { endEarly = false; } //don't do edge stops under ledges 
                }
            }

            if (frontCheck && wallCheck)  //stop if dashing into wall, even mid dash
            { if (frontCheck.touching) { endEarly = true; } }

            if (!onGround && noGrav) { airTime = coyoteTime; } //continuously reset airtime for gravity
            
            if (endEarly) { t = 1.1f; }
            
            yield return null;
        }

        //reset everything at the end of dash
        if (newLayer >= 0) { gameObject.layer = startLayer; }
        if (layerHP >= 0) { myHP.gameObject.layer = startLayerHP; }

        ignoreGravity = false;
        myVelocity.x = 0;
        momentum = 0;

        if (dashingEffect) { Destroy(dashingEffect); }
        dashing = null;
    }

}