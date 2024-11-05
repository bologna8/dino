using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Manually assign this please, and keep colliders in order")]
    //public Transform CharacterArt;
    public Transform ColliderContainer;
    [HideInInspector] public LayerCheck[] colliders; //Check colliders must be in this order in inspector:

    //middle section
    [HideInInspector] public LayerCheck headCheck; //0
    [HideInInspector] public LayerCheck passCheck; //1 - while dropping through passable objects
    [HideInInspector] public LayerCheck dashCheck; //2 - while dashing through dashable objects
    [HideInInspector] public LayerCheck groundCheck; //3

    //Right side
    [HideInInspector] public LayerCheck rightAirCheck; //4 - check above ledges
    [HideInInspector] public LayerCheck rightLedgeCheck; //5
    [HideInInspector] public LayerCheck rightWallCheck; //6
    [HideInInspector] public LayerCheck rightHipCheck; //7
    [HideInInspector] public LayerCheck rightForwardCheck; //8 - check ahead for edges
    
    
    //Left side
    [HideInInspector] public LayerCheck leftAirCheck; //9 - check above ledges
    [HideInInspector] public LayerCheck leftLedgeCheck; //10
    [HideInInspector] public LayerCheck leftWallCheck; //11
    [HideInInspector] public LayerCheck leftHipCheck; //12
    [HideInInspector] public LayerCheck leftForwardCheck; //13 - check ahead for edges

    [HideInInspector] public LayerCheck overheadCheck; //14 - for stopping unwanted wallslides and ledge pull ups

    [HideInInspector] public Core myCore;
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
    //[HideInInspector] public bool faceRight = true; //false = left
    //[HideInInspector] public bool goingLeft; //Last move input was to the left not right
    [HideInInspector] public bool movingRight = true; //Current movement direction is to the right
    //[HideInInspector] public bool turning = false; //true while mid turn
    //[Tooltip("Time it takes to flip left / right")] public float turnTime = 0.1f;
    [HideInInspector] public float slowPercent = 1f; //between 0 and 1
    [Range(0f,1f)] [Tooltip("Percentage of momentum preserved after landing")] public float landingMomentum = 0.5f;
    
    

    [Header("GRAVITY VARIABLES")]
    [Tooltip("Scale current gravity is multiplied by")] public float myGravityScale = 1f;
    [Tooltip("Scale that the speed of gravity increases")] public float gravityAcceleration = 1f;
    //[Tooltip("Max downward fall speed")] public float terminalVelocity = 10f;
    public float minGrav = 1f;
    [Tooltip("Min and Max fall speeds")] public float terminalVelocity = 20;
    //[Tooltip("Initial downward speed given after a jump ends")] public float jumpEndDownForce;
    private float airTime; //how long been in air, used for gravity calculation and coyote time
    


    [Header("JUMP VARIABLES")]
    [Tooltip("Initial jump velocity")] public float jumpForce = 10f;
    [Tooltip("Maximum amount of time for each jump input")] public float maxJumpTime = 1f;
    [Tooltip("Minimum amount of time for each jump input")] public float minJumpTime = 0.1f;
    private float currentJumpTime; //Amount of time jump button has been held
    [HideInInspector] public bool jumpInput; //Recive jump button input
    [HideInInspector] public bool onGround = false; //Feat on the ground and head in the air
    [HideInInspector] public bool justJumped; //detect if new jump input
    [Tooltip("Vertical velocity percent 0 to 1 over time as jump input is held")] public AnimationCurve jumpSpeedCurve;
    private float jumpDelayTime = 0.1f; //time Before you can Start a New Jump
    private float jumpDelayCurrent; //current delay until can jump again, prevents quick double inputs
    [Tooltip("Extra time to try and jump just after leaving the ground")] public float coyoteTime;
    private float coyoteCurrent; //Keep track of how long since left ground specifically
    [Tooltip("Extra mid air jumps")] public int bonusJumps = 0;
    private int remainingJumps; //Keep track of remaining bonus jumps
    [Tooltip("Spawn effect on every jump")] public GameObject jumpEffect;




    [Header("WALL JUMP VARIABLES")]
    [Tooltip("Bouncin off the walls")] public bool canWallJump = false;
    [Tooltip("Horizontal and vertical direction from wall jumps")] public Vector2 wallJumpForce;
    [Tooltip("Delay before wall jump for animation windup")] public float wallJumpDelay;
    [Tooltip("Duration of wall jump, x is locked in time, y is time for force to fade")] public Vector2 wallJumpTime;
    [Tooltip("Spawn effect on wall jumps")] public GameObject wallJumpEffect;
    [Tooltip("Dely until you actually lock onto a wall, needed so wall taps don't spam animation")] public float wallLockDelay;
    private float currentWallTime; //How long been on a wall
    [Tooltip("Change fall speed while pressing into a wall")] public bool canWallSlide = false;
    [Tooltip("Max fall speed while wall sliding")] public float wallSlideSpeed = 1f;
    [HideInInspector] public bool wallSliding; //Currently on a wall while falling
    private float recentWallJump;

    

    [Header("LEDGE VARIABLES")]
    [Tooltip("Can ya hang?")] public bool grabLedges = false;
    [Tooltip("X is delay before actual cimb time of Y")] public Vector2 climbTime;
    private float hangTime = 0; //How long been on ledge, used for slight delay before climbing up
    private float hangClimbDelay = 0.3f; //How long before you can climb input on ledge
    [HideInInspector] public bool onEdge; //Currently grabbing an edge
    [HideInInspector] public Coroutine climbing; //currently started a climb coroutine
    [Tooltip("Spawn effect when ledge grabbed")] public GameObject GrabLedgeEffect;
    [Tooltip("Climb small hip high obstacles easily")] public bool autoVaultOver = false;
    [Tooltip("Minimum momentum percent needed to vault of obstacles")] [Range(0f, 1f)] public float minVaultMomentum = 0f;
    [Tooltip("Time spent vaulting")] public float vaultSpeed;



    [Header("DASH VARIABLES")]
    [Tooltip("Time after doing a dash before ready to dash again")] public float dashCooldown = 0.4f;
    private float dashCurrentCD; //Keep track of current dash cooldown
    private Vector2 dashVelocity; //Current speed and direction of dash
    private Vector2 dashDecay; //Time it takes for dash velocity to fade, x is current, y is total time
    private Vector2 lastDash; //Initial velocity of the last dash done, used for fall off velocity

    [Tooltip("Multiply velocity when canceling dash")] public float dashCancelMultiplier;
    [Range(0f,1f)] [Tooltip("Percentage of current momentum saved after a dash")] public float momentumMaintained = 1f;
    private float savedMomentum; //previous momentum saved before dashing
    [Tooltip("Multiply dash distances based on current momentum")] public bool momentumBasedDash = false; 
    [Tooltip("Dash stops instantly before going over edges")] public bool edgeStopDash = true;
    [Tooltip("Collision layer is changed while dashing")] public bool changeCollisionLayer = true;
    private int startLayer; //remember to return to starting layer later
    [Tooltip("Hitbox layer is changed while dashing")] public bool changeHitboxLayer = false;
    private int startLayerHP; //remember to return hitbox to its tarting later;
    
    [Range(0, 30)] [Tooltip("Layer while dashing on ground")] public int dashLayer;
    [Tooltip("Speed while dashing")] public float dashForce = 20f;
    [Tooltip("Duration time of dash, x is time locked in animation, y is time of residual speed fall off")] public Vector2 dashTime;
    public GameObject dashEffect;


    [Range(0, 30)] [Tooltip("Layer while dashing on ground")] public int airDashLayer;
    [Tooltip("Speed while dashing in air")] public float airDashForce = 20f;
    [Tooltip("Duration time of air dash, x is time locked in animation, y is time of residual speed fall off")] public Vector2 airDashTime;
    [Tooltip("Multiply dash distances based on current momentum")] public bool momentumBasedAirDash = false;
    [Tooltip("Affected by gravity while dashing in air")] public bool gravityWhileAirDashing = false;
    private bool ignoreGravity = false; //Disable gravity during dashes
    [Tooltip("Number of times you can dash while mid air")] public int airDashes = 0;
    private int remainingDashes; //number of air dashes you have left
    [Range(-180, 180)] [Tooltip("Angle that air dash travels")] public float airDashAngle;
    public GameObject airDashEffect;
    
    [HideInInspector] public IEnumerator dashing; //Current dash coroutine, all knockbacks are a new dash technically
    //[Tooltip("Spawn effect when doing a dash")] public GameObject dashEffect;
    private GameObject dashingEffect; //keep track of current dash effect to delete at end of dash



    
    //OTHER VARIABLES
    private Health myHP; //Check stun times and change layer during dashes
    //[HideInInspector] public Animator myAnim; //animation handling needs to be cleaned up...
    [HideInInspector] public Rigidbody2D myBod; //nice bod
    [HideInInspector] public Collider2D myCollider; //Collider used by this bod


    //Colliders and such, should be under the "Self" game object in inspector
    //private GameObject mySelf; //must contain all colliders and any sprites to rotate when flipped, Health hitbox should be seperate

    //private float downSlopeSpeed = 20f; //Add downard force when going down slopes



    void Awake()
    {
        myBod = GetComponent<Rigidbody2D>();
        //myBod.gravityScale = 0; //turn off rigidbody gravity

        myHP = GetComponentInChildren<Health>();
        //myAnim = GetComponentInChildren<Animator>();

        startLayer = gameObject.layer; //remember for later
        if (myHP) { startLayerHP = myHP.gameObject.layer; }

        myCollider = GetComponent<Collider2D>();
        //mySelf = transform.Find("Self").gameObject;
        colliders = ColliderContainer.GetComponentsInChildren<LayerCheck>();

        if (colliders.Length > 0) { headCheck = colliders[0]; }
        if (colliders.Length > 1) { passCheck = colliders[1]; }
        if (colliders.Length > 2) { dashCheck = colliders[2]; }
        if (colliders.Length > 3) { groundCheck = colliders[3]; }

        if (colliders.Length > 4) { rightAirCheck = colliders[4]; }
        if (colliders.Length > 5) { rightLedgeCheck = colliders[5]; }
        if (colliders.Length > 6) { rightWallCheck = colliders[6]; }
        if (colliders.Length > 7) { rightHipCheck = colliders[7]; }
        if (colliders.Length > 8) { rightForwardCheck = colliders[8]; }

        if (colliders.Length > 9) { leftAirCheck = colliders[9]; }
        if (colliders.Length > 10) { leftLedgeCheck = colliders[10]; }
        if (colliders.Length > 11) { leftWallCheck = colliders[11]; }
        if (colliders.Length > 12) { leftHipCheck = colliders[12]; }
        if (colliders.Length > 13) { leftForwardCheck = colliders[13]; }

        if (colliders.Length > 14) { overheadCheck = colliders[14]; }
        

    }

    void FixedUpdate()
    {
        if (groundCheck) { onGround = groundCheck.touching; }


        //if (myAnim) { myAnim.SetBool("onGround", onGround); }

        UpdateTimers();
        UpdateInputs();

        if (grabLedges) { ledgeGrab(); } //ledge grab if allowed

        //if (myAnim) { myAnim.SetFloat("momentum", momentum); }

        UpdateGravity();

    }

    public void UpdateTimers()
    {

        if (jumpDelayCurrent > 0) { jumpDelayCurrent -= Time.deltaTime; } //prevent multi quick jump inputs

        if (dashing == null) 
        {
            if (dashCurrentCD > 0) { dashCurrentCD -= Time.deltaTime; } 
            if (dashDecay.x > 0) //fade dash velocity over time
            { 
                dashDecay.x -= Time.deltaTime;
                dashVelocity = Vector2.Lerp(lastDash, Vector2.zero, (dashDecay.y - dashDecay.x) / dashDecay.y);
            }
            else { dashVelocity = Vector2.zero; lastDash = Vector2.zero; }
        }
        

        if (onGround || onEdge) //Reset ground stuff
        {
            currentWallTime = 0;
            recentWallJump = 0;

            if (airTime > coyoteTime) //Reset stuff when first landing
            { 
                momentum *= landingMomentum; //reduce speed when first landing on the ground
                if (dashVelocity.x != 0) { dashVelocity.x *= landingMomentum; lastDash.x *= landingMomentum; }
                if (dashVelocity.y != 0) { dashVelocity.y = 0; lastDash.y = 0; } //Reset verticle dash speed when you hit the ground
            }

            if (myCollider.enabled) //collider disabled while passing through platform
            {
                airTime = 0; coyoteCurrent = 0;
                remainingJumps = bonusJumps; remainingDashes = airDashes; dashCurrentCD = 0; 
            }

            if (onEdge) { coyoteCurrent = coyoteTime; } //{ airTime = coyoteTime; } //Can't coyote jump just after being on an edge
            else if (myCollider.enabled) { myVelocity.y = 0; } 
            /*
            else if (myBod.velocity.y <= 0 && myCollider.enabled)
            { 
                if (groundCheck.slope != 0) //fall faster going down slopes
                { 
                    //check if trying walking down a slope
                    var goingDown = false;
                    if (groundCheck.slope < 0) 
                    { 
                        if (moveInput > 0) { goingDown = true; }
                        if (dashVelocity.x > 0 && dashVelocity.y <= 0) { goingDown = true; }
                    }
                    if (groundCheck.slope > 0)
                    {
                        if (moveInput < 0) { goingDown = true; }
                        if (dashVelocity.x < 0 && dashVelocity.y <= 0) { goingDown = true; }
                    }

                    if (goingDown) { myVelocity.y = -downSlopeSpeed; } //slide down slopes
                    else { myVelocity.y = 0; } //normal ground reset
                } 
                else { myVelocity.y = 0; } //reset velocity on ground and not dropping through a platform 
            } 
            */
        } 
        else { airTime += Time.deltaTime; coyoteCurrent += Time.deltaTime; } //Keep track of air time for gravity and coyote jumps

        if (rightWallCheck && leftWallCheck) //Update time since last touched a wall to jump off of
        {

            if (rightWallCheck.touching || leftWallCheck.touching) { currentWallTime += Time.deltaTime; }
            else if (recentWallJump > 0) { recentWallJump -= Time.deltaTime; currentWallTime = wallLockDelay; }
            else { currentWallTime = 0; }
            
        }
    }

    public void UpdateInputs()
    {
        var canInput = true;
        if (myHP) { if (myHP.stunTime > 0) { canInput = false; } }
        if (dashing != null || climbing != null) { canInput = false; }

        if (canInput)
        {
            Run(); //Forest Run

            //JumpStart is called seperatly on button down
            if (justJumped) //Handle if jump button just pressed is being held or released
            {
                if (jumpInput) { JumpHold(); }
                else { JumpEnd(false); }
            }
        }
        else 
        { 
            momentum = 0f;
            myVelocity.x = 0f;
            if (justJumped) { JumpEnd(false); }
        }
        
    }

    public void UpdateGravity()
    {
        var velocity = myVelocity + dashVelocity;

        bool applyGravity = true;
        if (onEdge || onGround || ignoreGravity) { applyGravity = false; }

        var gravTime = airTime * gravityAcceleration;
        
        if (applyGravity) { velocity.y -= myGravityScale * Mathf.Pow(minGrav + gravTime, 2); } //G = C * (m/s)^2
        
        wallSliding = false;
        if (myBod.velocity.y < 0)
        {
            var noHead = true;
            if (rightWallCheck && leftWallCheck) //Both wall checks touching same as head check toughing
            { if (rightWallCheck.touching && leftWallCheck.touching) { noHead = false;} } 
            
            if (overheadCheck && noHead) { if (overheadCheck.touching) { noHead = false; } }

            if(canWallSlide && currentWallTime >= wallLockDelay && noHead) 
            {
                wallSliding = true;
                if (moveInput != 0) 
                { airTime = 0; velocity.y = Mathf.Clamp(velocity.y, -wallSlideSpeed, Mathf.Infinity); }
            } 
            else //clamp fall speed with slide velocity
            { velocity.y = Mathf.Clamp(velocity.y, -terminalVelocity, Mathf.Infinity); }
        }

        myBod.velocity = velocity;
    }

    void Run()
    {
        CheckDirectionChange();
        
        if (autoVaultOver && momentum >= minVaultMomentum) { VaultOver(); } //Jump over objects //&& InputForward()

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



        if (leftHipCheck && rightHipCheck)
        {
            if (rightHipCheck.touching)
            {
                if (moveInput > 0) { momentum = 0; }
                //if (dashVelocity.x > 0 && movingRight) { dashVelocity.x = 0; }
            } 

            if (leftHipCheck.touching)
            {
                if (moveInput < 0) { momentum = 0; }
                //if (dashVelocity.x < 0 && !movingRight) { dashVelocity.x = 0; }
            }

        }

        /*
        if (dashCheck) 
        { 
            if(dashCheck.touching) { momentum = 0; } 
        }
        */
        

        if (wallSliding) 
        { 
            if (rightWallCheck.touching) 
            { 
                if (myCore) { if(!myCore.lookingRight) { myCore.Turn(); } }
                if (moveInput > 0) { momentum = 0; }
                else if (moveInput < 0 && canWallJump) { dashing = WallJump(); StartCoroutine(dashing); }
                
            }
            if (leftWallCheck.touching) 
            { 
                if (myCore) { if(myCore.lookingRight) { myCore.Turn(); } }
                if (moveInput < 0) { momentum = 0; }
                else if (moveInput > 0 && canWallJump) { dashing = WallJump(); StartCoroutine(dashing); }
            }

        }

        //Slow percent sets maximum to less than 1 if slowed
        momentum = Mathf.Clamp(momentum, 0, slowPercent);

        myVelocity.x = baseSpeed * moveSpeedCurve.Evaluate(momentum);
        if (!onGround) { myVelocity.x *= airSpeedMultiplier; } //mid air speed multiplier

        //Step to the left
        if (!movingRight) { myVelocity.x *= -1; }
        

    }

    public void CheckDirectionChange()
    {
        bool turned = false;
        if (moveInput > 0 && !movingRight) { turned = true; }
        if (moveInput < 0 && movingRight) { turned = true; }

        if (turned) { momentum = 0f; movingRight = !movingRight; } // Turn();

    }




    //JUMP, JUMP, JUMP AROUND
    public void JumpStart()
    {
        var canJump = true;
        if (headCheck) { if(headCheck.touching) { canJump = false; } }
        if (climbing != null) { canJump = false; }

        if (jumpDelayCurrent <= 0 && canJump)
        {
            bool jumpDown = false; //Check if trying to jump down through a pass platform
            if (passCheck) { if (onGround && passCheck.touching && verticalInput < 0) { jumpDown = true; } }

            bool jumpReady = false;
            
            if (jumpDown) { StartCoroutine(drop()); }
            else if (onGround) { jumpReady = true; } //Normal jump
            else if (onEdge) //Climb up edges
            { 
                if (hangTime > hangClimbDelay) 
                { 
                    //if (rightLedgeCheck.touching) { climbing = StartCoroutine(ledgeClimb(rightLedgeCheck.findTopCorner(movingRight), climbTime.y, climbTime.x)); }
                    //if (leftLedgeCheck.touching) { climbing = StartCoroutine(ledgeClimb(leftLedgeCheck.findTopCorner(movingRight), climbTime.y, climbTime.x)); }
                    if (rightLedgeCheck.touching) { climbing = StartCoroutine(ledgeClimb(rightLedgeCheck.closestCorner(rightAirCheck.transform.position), climbTime.y, climbTime.x)); }
                    if (leftLedgeCheck.touching) { climbing = StartCoroutine(ledgeClimb(leftLedgeCheck.closestCorner(leftAirCheck.transform.position), climbTime.y, climbTime.x)); }
                } 
            }
            else if (currentWallTime >= wallLockDelay && canWallJump && !ignoreGravity) //(frontCheck.touching && !turning && wallJump) //wall jumps are just lil dashes
            {
                if (rightWallCheck.touching || leftWallCheck.touching)
                {
                    //if (myAnim) { myAnim.SetBool("onWall", true); }
                    JumpEnd();
                    if (myCore) { myCore.Turn(); }
                    dashing = WallJump(); StartCoroutine(dashing); 
                }
                
            } 
            else if (coyoteCurrent < coyoteTime) //jump if just recently left the ground, not via upward
            { 
                if (myBod.velocity.y <= 0 && currentWallTime <= 0) { jumpReady = true; } 
            } 
            else if (remainingJumps > 0)
            {
                remainingJumps --;
                jumpReady = true;
            }

            if (dashing != null && onGround) //Cancel dash with jump
            {
                if (dashCheck) { if (dashCheck.touching) { return; } } 
                CancelDash(true, dashTime.y, dashCancelMultiplier); 
            }

            //Actually do the jumping
            if (jumpReady)
            {
                myVelocity.y = jumpForce;
                airTime = 0;
                coyoteCurrent = coyoteTime;
                
                jumpDelayCurrent = jumpDelayTime;
                justJumped = true;

                if (jumpEffect) { PoolManager.Instance.Spawn(jumpEffect, groundCheck.transform.position, Quaternion.identity); }    
            }
            
        }
        
    }

    public void JumpHold()
    {
        currentJumpTime += Time.deltaTime;
        airTime = 0;
        myVelocity.y = jumpForce * jumpSpeedCurve.Evaluate(currentJumpTime / maxJumpTime);

        if (currentJumpTime > maxJumpTime) { JumpEnd(); }
        if (headCheck) { if(headCheck.touching) { JumpEnd(); } }  //end early if hit head
    }

    public void JumpEnd(bool mustEnd = true) //true mean jump is hard canceled by other means
    {
        if (currentJumpTime < minJumpTime && !mustEnd) { JumpHold(); } //minimum jump time should keep jumping
        else
        {
            myVelocity.y = 0; //-jumpEndDownForce;
            justJumped = false;
            currentJumpTime = 0;

            //if (myAnim) { myAnim.SetBool("jumping", false); }
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

    public IEnumerator WallJump()
    {
        ignoreGravity = true;
        dashVelocity = Vector2.zero;

        yield return new WaitForSeconds(wallJumpDelay);

        recentWallJump = wallLockDelay;

        //if (frontCheck.touching && !turning && myCore) { myCore.Turn(); } 
        if (rightWallCheck.touching) { movingRight = false; }
        if (leftWallCheck.touching) { movingRight = true; }

        var dir = wallJumpForce; 
        if (!movingRight) { dir.x *= -1; }
        DoDash(dir, wallJumpTime);
        //if (jumpEffect) { Instantiate(jumpEffect, groundCheck.transform.position, Quaternion.identity); }
        if (wallJumpEffect) { PoolManager.Instance.Spawn(wallJumpEffect, groundCheck.transform.position, Quaternion.identity); }  
    }



    //CLIMB TIME!
    private void ledgeGrab()
    {
        bool tryGrab = false;

        if (leftLedgeCheck && leftAirCheck)
        {
            if (!leftAirCheck.touching && leftLedgeCheck.touching && moveInput <= 0)
            { tryGrab = true; }   
        }

        if (rightLedgeCheck && rightAirCheck)
        {
            if(!rightAirCheck.touching && rightLedgeCheck.touching && moveInput >= 0)
            { tryGrab = true; }
        }


        if (onGround) { tryGrab = false; }
        //if (turning) { tryGrab = false; }
        if (jumpDelayCurrent > 0) { tryGrab = false; }
        if (myHP) { if (myHP.stunTime > 0) { tryGrab = false; } }
        
        //if(climbing != null) { tryGrab = true; } //stay on ledge while ledge climb started

        
        if (tryGrab)
        {
            if (verticalInput < 0) //Drop if pressing down
            { 
                jumpDelayCurrent = jumpDelayTime; myVelocity.y = -wallSlideSpeed; 
                currentWallTime = wallLockDelay;
            } 
            else //Actually grab that ledge
            {
                bool turnToCatch = false;

                if (rightLedgeCheck.touching && !movingRight) { turnToCatch = true; }
                if (leftLedgeCheck.touching && movingRight) { turnToCatch = true; }

                if (turnToCatch)
                {
                    movingRight = !movingRight;
                    if (myCore) { myCore.Turn(); }
                }

                if (!onEdge && GrabLedgeEffect)
                { PoolManager.Instance.Spawn(GrabLedgeEffect, transform.position, Quaternion.identity, transform); }

                onEdge = true;
                airTime = 0; //Reset airtime while ya hang
                coyoteCurrent = coyoteTime;
                momentum = 0;
                myVelocity.y = 0;
                
                if (dashing != null) { CancelDash(); } //cancel dash if ledge grabbed


                var cornerHit = Vector3.zero;
                var offset = Vector3.zero;

                if (movingRight) 
                { 
                    cornerHit = rightLedgeCheck.closestCorner(rightAirCheck.transform.position);
                    offset = rightLedgeCheck.transform.localPosition;
                }
                else
                {
                    cornerHit = leftLedgeCheck.closestCorner(leftAirCheck.transform.position);
                    offset = leftLedgeCheck.transform.localPosition;
                }

                transform.position = cornerHit - offset;

                hangTime += Time.deltaTime;
            
                //Climb up edge with move inputs if up or in direction of edge
                if (climbing == null && hangTime >= hangClimbDelay) 
                { 
                    bool climbInput = false;
                    if (verticalInput > 0) { climbInput = true; }
                    if (rightLedgeCheck.touching && moveInput > 0) { climbInput = true; }
                    if (leftLedgeCheck.touching && moveInput < 0) { climbInput = true; }

                    if (climbInput) { climbing = StartCoroutine(ledgeClimb(cornerHit, climbTime.y, climbTime.x)); }
                }
            }

        }
        else { onEdge = false; hangTime = 0f; }

        //if (myAnim) { myAnim.SetBool("onEdge", onEdge); }

    }

    public IEnumerator ledgeClimb(Vector3 corner, float duration, float delay = 0)
    {
        //if (myAnim) { myAnim.SetBool("climbing", true); }

        if (overheadCheck) { if (overheadCheck.touching) { yield break; } }

        yield return new WaitForSeconds(delay);

        var startFrom = transform.position;
        var moveTo = corner - groundCheck.transform.localPosition;

        if (movingRight) { moveTo.x += myCollider.bounds.extents.x; }
        else { moveTo.x -= myCollider.bounds.extents.x; }

        //ledgeCheck.touching = false; //Snaps back to ledge for some reason without this

        for (var t = 0f; t <= 1; t += Time.deltaTime / (duration /2)) //move up first
        {
            transform.position = Vector2.Lerp(startFrom, new Vector2(startFrom.x, moveTo.y), t);
            yield return null;
        }

        for (var t = 0f; t <= 1; t += Time.deltaTime / (duration /2)) //move over next
        {
            transform.position = Vector2.Lerp(new Vector2(startFrom.x, moveTo.y), moveTo, t);
            if (groundCheck) { groundCheck.touching = true; } //hardSet to grounded for anim update override issue
            yield return null;
        }

        //Snaps back to ledge for some reason without this
        leftLedgeCheck.touching = false; 
        rightLedgeCheck.touching = false;

        /* 
        //Go directly diagonal
        for (var t = 0f; t <= 1; t += Time.deltaTime / duration)
        {
            transform.position = Vector2.Lerp(startFrom, moveTo, t);
            ledgeCheck.touching = false; //Snaps back to ledge for some reason without this
            yield return null;
        }
        */

        transform.position = moveTo;
        climbing = null;

        //if (myAnim) { myAnim.SetBool("climbing", false); }
    }
    

    public void VaultOver() //For smaller, chest high walls
    {
        if (leftHipCheck && rightHipCheck && leftLedgeCheck && rightLedgeCheck && dashing == null && myBod.velocity.y >= 0) // don't vault when goin down
        {
            if (movingRight && rightHipCheck.touching && rightHipCheck.slope == 0 && !rightLedgeCheck.touching) 
            { climbing = StartCoroutine(ledgeClimb(rightHipCheck.closestCorner(transform.position), vaultSpeed)); }

            if (!movingRight && leftHipCheck.touching && leftHipCheck.slope == 0 && !leftLedgeCheck.touching)
            { climbing = StartCoroutine(ledgeClimb(leftHipCheck.closestCorner(transform.position), vaultSpeed)); }

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
            dashCurrentCD = dashCooldown + dashTime.x;

            
            var d = Vector2.right * dashForce; //direction and speed to dash
            //change direction to left
            if (!movingRight) { d *= -1; }

            if (momentumBasedDash) { d *= (momentum + 1f); } //more momentum multiplies dash distances
            var t = dashTime; //duration of dash
            var noGrav = false;
            var edgeStop = edgeStopDash;

            int changeLayer = -1; //negative means don't change
            if (changeCollisionLayer) { changeLayer = dashLayer; }
            int changeHP = -1; //change the health layer
            if (changeHitboxLayer && myHP) { changeHP = dashLayer; } 
            
            if (!onGround) //different if air dashing
            { 
                var ang = new Vector2(Mathf.Cos(airDashAngle * Mathf.Deg2Rad), Mathf.Sin(airDashAngle * Mathf.Deg2Rad));
                if (!movingRight) { ang = new Vector2(Mathf.Cos((180 - airDashAngle) * Mathf.Deg2Rad), Mathf.Sin((180 - airDashAngle) * Mathf.Deg2Rad)); }
                d = ang * airDashForce;
                if (momentumBasedAirDash) { d *= (momentum + 1f); }
                t = airDashTime;
                noGrav = !gravityWhileAirDashing;
                edgeStop = false;
                remainingDashes --;

                if (changeCollisionLayer) { changeLayer = airDashLayer; }
                if (changeHitboxLayer && myHP) { changeHP = airDashLayer; } 

                if (airDashEffect)
                { PoolManager.Instance.Spawn(airDashEffect, transform.position, Quaternion.identity, transform); }
            }
            else if (dashEffect)
            { PoolManager.Instance.Spawn(dashEffect, transform.position, Quaternion.identity, transform); }
            

            DoDash(d, t, noGrav, true, edgeStop, changeLayer, changeHP);
            //if (dashEffect) { dashingEffect = Instantiate(dashEffect, transform); } //spawn dash trail as child obect

        }
    }

    public void DoDash(Vector2 dir, Vector2 duration, bool noGrav = false, bool keepMomentum = false, bool edgeStop = false, int newLayer = -1, int layerHP = -1)
    {
        if (dashing != null) { CancelDash(); }
        if (climbing != null) { StopCoroutine(climbing); climbing = null; }


        dashing = DirectionalDash(dir, duration, noGrav, keepMomentum, edgeStop, newLayer, layerHP);
        StartCoroutine(dashing);
    }

    public IEnumerator DirectionalDash(Vector2 dir, Vector2 duration, bool noGrav, bool keepMomentum, bool edgeStop, int newLayer, int layerHP)
    {
        //if (myAnim) { myAnim.SetBool("dashing", true); }

        savedMomentum = momentum;
        lastDash = dir; dashVelocity = dir;


        myBod.velocity = Vector2.zero; //reset current velocity on new dash
        myVelocity = Vector2.zero;

        airTime = 0;
        coyoteCurrent = coyoteTime;
        ignoreGravity = noGrav;

        if (newLayer >= 0) { gameObject.layer = newLayer; }
        if (layerHP >= 0) { myHP.gameObject.layer = layerHP; }

        for (var t = 0f; t <= 1; t += Time.deltaTime / duration.x)
        {
            dashDecay = new Vector2(duration.y, duration.y); //Keep resetting decay time while dash is going

            bool endEarly = false; //end early if dashing into wall or dashing off an edge

            if (edgeStop) //stop if about to dash off an edge
            { 
                if (leftForwardCheck) { if (!leftForwardCheck.touching && dir.x < 0) { endEarly = true; } }
                if (rightForwardCheck) { if (!rightForwardCheck.touching && dir.x > 0) { endEarly = true; } }
            }

            if (dashCheck && newLayer >= 0) //keep dashing if still stuck in dashable objects
            { 
                if(dashCheck.touching && onGround) 
                { 
                    if (t > 0) { t = 0.5f; } //Extend time indefinitely
                    if (endEarly) { endEarly = false; } //don't do edge stops under ledges 
                }
            }


            //Stop if dashing into a wall (not doin this for now so you can have dashable big enemies)
            //if (leftHipCheck) { if (leftHipCheck.touching && dir.x < 0) { endEarly = true; } }
            //if (rightHipCheck) { if (rightHipCheck.touching && dir.x > 0) { endEarly = true; } }



            if (!onGround && noGrav) { airTime = 0; coyoteCurrent = coyoteTime; } //continuously reset airtime for gravity
            
            if (endEarly) { CancelDash(); }
            
            yield return null;
        }

        CancelDash(keepMomentum, duration.y);
    }

    public void CancelDash(bool keepMomentum = false, float speedDecay = 0f, float cancelMultiplier = 1f)
    {
        if (dashing != null)
        {
            //reset everything at the end of dash
            StopCoroutine(dashing);

            dashDecay = new Vector2(speedDecay, speedDecay); //set fall off time for dash velocity
            lastDash *= cancelMultiplier; dashVelocity = lastDash; //multiply velocity after cancelled

            gameObject.layer = startLayer;
            if (myHP) { myHP.gameObject.layer = startLayerHP; }

            ignoreGravity = false;

            if (keepMomentum) { momentum = savedMomentum * momentumMaintained; }
            else { momentum = 0; }

            //if (myAnim) { myAnim.SetBool("dashing", false); }

            if (dashingEffect) { Destroy(dashingEffect); }

            dashing = null;
        }
        
    }

}