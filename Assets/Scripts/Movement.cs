using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float accelerate = 10f;
    public float baseSpeed = 10f;
    public float momentumTime = 1f;
    [HideInInspector] public float momentumCurrent;
    public float jumpForce = 100f;
    private float jumpDelay = 0.1f;
    public GameObject jumpEffect;

    [HideInInspector] public CheckCheck[] colliders;
    private GameObject mySelf;

    [HideInInspector] public int moveInput;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public bool faceRight = true;
    [HideInInspector] public bool onGround = false;
    [HideInInspector] public Rigidbody2D myBod;
    private float startGrav;
    [HideInInspector] public bool onEdge;
    [HideInInspector] public bool turning = false;
    public float turnTime = 0.1f;
    public bool grabLedges = false;

    public float dashForce = 100f;
    public float dashTime = 0.1f;
    public float dashCooldown = 0.3f;
    private float dashCurrent;
    private Health myHP;

    private Coroutine climbingCoroutine;
    [HideInInspector] public Animator myAnim;


    // Start is called before the first frame update
    void Start()
    {
        myBod = GetComponent<Rigidbody2D>();
        startGrav = myBod.gravityScale;

        myHP = GetComponentInChildren<Health>();
        myAnim = GetComponentInChildren<Animator>();

        mySelf = transform.Find("Self").gameObject;
        colliders = mySelf.transform.GetComponentsInChildren<CheckCheck>();

    }

    // Update is called once per frame
    void Update()
    {
        onGround = colliders[0].touching;
        if (myAnim) { myAnim.SetBool("onGround", onGround); }

        if (grabLedges) { ledgeGrab(); }

        if (moveInput == 0) //Momentum is the time it takes to accelerate and decalerate to base Speed
        { if (onGround || onEdge) { momentumCurrent -= Time.deltaTime; } }
        else { momentumCurrent += Time.deltaTime; }
        momentumCurrent = Mathf.Clamp(momentumCurrent, 0, momentumTime);

        if (myAnim) { myAnim.SetFloat("moveValue", momentumCurrent / momentumTime); }

        if (jumpDelay > 0) { jumpDelay -= Time.deltaTime; }
        if (dashCurrent > 0) { dashCurrent -= Time.deltaTime; }
    }

    private void FixedUpdate()
    {
        if (myHP.stunTime <= 0) { Run(); }
    }

    public void Turn()
    {
        if (!turning)
        {
            faceRight = !faceRight;
            momentumCurrent = 0;
            myBod.velocity = new Vector2(0, myBod.velocity.y);

            StartCoroutine(TurnTime());
        } 
 
    }

    private IEnumerator TurnTime()
    {
        turning = true;

        var fromAngle = mySelf.transform.rotation;
        var toAngle = Quaternion.Euler(mySelf.transform.eulerAngles + new Vector3(0, 180, 0));

        for (var t = 0f; t <= 1; t += Time.deltaTime / turnTime)
        {
            mySelf.transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }

        mySelf.transform.rotation = toAngle;
        turning = false;

    }


    void Run()
    {
        if (moveInput < 0 && faceRight) { Turn(); }
        if (moveInput > 0 && !faceRight) { Turn(); }

        if (!colliders[1].touching)
        {
            float Xforce = accelerate * moveInput;
            myBod.AddForce(Vector2.right * Xforce);

            var max = Mathf.Lerp(0, baseSpeed, momentumCurrent / momentumTime);
            var moveX = Mathf.Clamp(myBod.velocity.x, -max, max);
            myBod.velocity = new Vector2(moveX, myBod.velocity.y);
        }
        //else if (onEdge && moveInput != 0 && !turning) { Jump(); }

    }

    public void Dash()
    {
        if (dashCurrent <= 0) 
        {
            var d = Vector2.right * dashForce;
            if (!faceRight) { d *= -1; }
            myHP.TakeDamage(0, dashTime, d);
            myHP.invincibleTime = dashTime;
            dashCurrent = dashCooldown;
        }
    }

    public void Jump()
    {
        if (jumpDelay <= 0)
        {
            if (onGround)
            {
                myBod.velocity = new Vector2(myBod.velocity.x, 0);
                myBod.AddForce(Vector2.up * jumpForce);
                jumpDelay = 0.1f;

                if (myAnim) { myAnim.SetTrigger("jumped"); }

                if (jumpEffect) { Instantiate(jumpEffect, colliders[0].transform.position, Quaternion.identity); }
            }
            else if (onEdge && climbingCoroutine == null) 
            { climbingCoroutine = StartCoroutine(ledgeClimb()); }
            
        }        
            
    }

    public IEnumerator ledgeClimb()
    {
        if (myAnim) { myAnim.SetTrigger("climbed"); }
        var ledgeDelay = 0.4f;
        myHP.TakeDamage(0, ledgeDelay, Vector2.zero);
        myBod.velocity = new Vector2(myBod.velocity.x, 0);
        myBod.gravityScale = 0f;
        
        yield return new WaitForSeconds(ledgeDelay);
        myBod.gravityScale = startGrav;
        jumpDelay = 0.1f;
        if (jumpEffect) { Instantiate(jumpEffect, colliders[2].transform.position, Quaternion.identity); }
        myBod.velocity = new Vector2(myBod.velocity.x, 0);
        myBod.AddForce(Vector2.up * jumpForce);
        climbingCoroutine = null;
    }


    private void ledgeGrab()
    {
        bool tryGrab = true;
        if (verticalInput < 0) { jumpDelay = 0.2f; }     
        if (turning) { tryGrab = false; }
        if (jumpDelay > 0) { tryGrab = false; }
        if (myHP.stunTime > 0) { tryGrab = false; }
        

        if(climbingCoroutine != null) { tryGrab = true; }
        
        if (!onGround && colliders[2].touching && !colliders[3].touching && tryGrab)
        { 
            myBod.gravityScale = 0; onEdge = true;
            myBod.velocity = Vector2.zero;
            momentumCurrent = 0f;

            var cornerHit = colliders[2].findTopCorner(faceRight);
            var offset = colliders[2].transform.localPosition;
            var checkParent = colliders[2].transform.parent;
            if (checkParent) { offset += checkParent.transform.localPosition; }

            if (!faceRight) { offset = new Vector2(offset.x *-1, offset.y); }
            transform.position = cornerHit - offset;

            if (verticalInput > 0) { Jump(); }
            if (moveInput > 0 && faceRight) { Jump(); }
            if (moveInput < 0 && !faceRight) { Jump(); }
        }
        else { myBod.gravityScale = startGrav; onEdge = false; }

        if (myAnim) { myAnim.SetBool("onEdge", onEdge); }

    }


}
