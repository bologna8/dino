using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float accelerate = 10f;
    public float baseSpeed = 10f;
    public float momentumTime = 1f;
    private float momentumCurrent;
    public float jumpForce = 100f;
    private float jumpDelay = 0.1f;

    private CheckCheck[] colliders;
    private GameObject mySelf;

    [HideInInspector] public int moveInput;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public bool faceRight = true;
    [HideInInspector] public bool onGround = false;

    private Rigidbody2D myBod;
    private float startGrav;
    private bool onEdge;
    private bool turning = false;
    public float turnTime = 0.1f;
    public bool grabLedges = false;


    // Start is called before the first frame update
    void Start()
    {
        myBod = GetComponent<Rigidbody2D>();
        startGrav = myBod.gravityScale;

        mySelf = transform.Find("Self").gameObject;
        colliders = mySelf.transform.GetComponentsInChildren<CheckCheck>();

    }

    // Update is called once per frame
    void Update()
    {
        onGround = colliders[0].touching;

        if (grabLedges) { ledgeGrab(); }

        if (moveInput == 0) //Momentum is the time it takes to accelerate and decalerate to base Speed
        { if (onGround || onEdge) { momentumCurrent -= Time.deltaTime; } }
        else { momentumCurrent += Time.deltaTime; }
        momentumCurrent = Mathf.Clamp(momentumCurrent, 0, momentumTime);

        if (jumpDelay > 0) { jumpDelay -= Time.deltaTime; }
    }

    private void FixedUpdate()
    {
        Run();

    }

    void Turn()
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

    }


    public void Jump()
    {
        if (jumpDelay <= 0)
        {
            if (onGround || onEdge)
            {
                myBod.velocity = new Vector2(myBod.velocity.x, 0);
                myBod.AddForce(Vector2.up * jumpForce);
                jumpDelay = 0.1f;
            } 
        }
            
    }


    private void ledgeGrab()
    {
        bool tryGrab = true;
        if (verticalInput > 0) { Jump(); }
        if (verticalInput < 0) { tryGrab = false; }
        if (turning) { tryGrab = false; }
        if (jumpDelay > 0) { tryGrab = false; }
        
        if (!onGround && colliders[2].touching && !colliders[3].touching && tryGrab)
        { 
            myBod.gravityScale = 0; onEdge = true;
            myBod.velocity = Vector2.zero;

            var cornerHit = colliders[2].findTopCorner(faceRight);
            var offset = colliders[2].transform.localPosition;
            if (!faceRight) { offset = new Vector2(offset.x *-1, offset.y); }
            transform.position = cornerHit - offset;
        }
        else { myBod.gravityScale = startGrav; onEdge = false; }          
    }


}