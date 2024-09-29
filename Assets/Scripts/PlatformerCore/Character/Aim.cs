using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Automatic Interface Modifier
public class Aim : MonoBehaviour
{
    public enum InputType { Auto, Mouse, Controller };
    public InputType myInputType;

    public enum AimType { Free, Hold, Simple };
    public AimType myAimType;  

    public float turnSpeed = -1f;
    [HideInInspector] public float turnSpeedMultiplier = 1;
    public Vector3 offset;
    public float lockLength = 1f;
    [HideInInspector] public Vector3 currentDirection;
    [HideInInspector] public Vector3 desiredDirection;
    [HideInInspector] public float currentAng; //current euler angle of aim



    [HideInInspector] public float attackAngleOffset;
    [HideInInspector] public float forwardAngle;
    [Range(0, 180)] public float maxRotation;



    private LineRenderer myLine;
    public float sightRange = 10f;
    public LayerMask sightLayers;
    [HideInInspector] public Transform lastSeen;
    [HideInInspector] public float lookLockTime;


    [HideInInspector] public Transform AutoAimAt;
    [HideInInspector] public Vector2 waitToRotate;
    [HideInInspector] public float currentWaitTime;
    [HideInInspector] public bool turnToMax;
    

    private Controls myControls;
    [HideInInspector] public float verticalInput; 

    //private LayerCheck safetyLayers;
    [HideInInspector] public bool touching = false;

    private SpriteRenderer mySprite;
    private Color startColor;
    //public Color safetyColor;



    public Transform WeaponPivot;
    private Vector3 weaponPivotStart;

    //[Tooltip("Distance from pivot in diraction of aim, X is right hand, Y is left.")] public Vector2 HandPositions;
    public Transform RightHand;
    public Transform RightHandle;
    public Transform LeftHand;
    public Transform LeftHandle;



    //Start is called before the first frame update
    void Awake()
    {
        //safetyLayers = GetComponent<LayerCheck>();

        mySprite = GetComponent<SpriteRenderer>();
        if (mySprite) { startColor = mySprite.color; }

        myLine = GetComponent<LineRenderer>();

        currentDirection = Vector3.right;

        if (WeaponPivot) { weaponPivotStart = WeaponPivot.localPosition; }

        if (myInputType != InputType.Auto)
        {
            var pc = GetComponentInParent<PlayerControls>(); 
            if (pc) { myControls = pc.myControls; }
        }
    }


    void OnMouseMove()
    {
        if (myInputType == InputType.Controller)
        { myInputType = InputType.Mouse; }
    }

    void OnControllerStick(InputValue val)
    {
        if (myInputType == InputType.Mouse)
        { myInputType = InputType.Controller; }
        
        if (myInputType == InputType.Controller)
        { desiredDirection = val.Get<Vector2>(); } 
    }


   
    // Update is called once per frame
    void Update()
    {
        bool lockAim = false;
        
        if (myAimType == AimType.Hold && !myControls.Aiming.AimButton.IsPressed())
        { lockAim = true; }

        if (myAimType == AimType.Simple) 
        { lockAim = true; }

        if (attackAngleOffset != 0)
        {
            if (forwardAngle > 90 && forwardAngle < 270) { forwardAngle -= attackAngleOffset; }
            else { forwardAngle += attackAngleOffset; }
        }

        if (lockAim) //Override current desired angle if aiming button required and not pressed
        {

            var rad = forwardAngle * Mathf.Deg2Rad;

            desiredDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            currentDirection = desiredDirection;
        }
        else //All this for smooth aim and angle turning 
        {
            var minAng = (forwardAngle - maxRotation);
            var maxAng = (forwardAngle + maxRotation);
            var oppAng = forwardAngle - 180; 

            if (myInputType == InputType.Auto)
            {
                if (AutoAimAt)
                {
                    desiredDirection = (AutoAimAt.transform.position - transform.parent.position).normalized;

                    //Check angle currently turning to maintain that truning direction when sight is lost
                    var currentAng = Vector3.SignedAngle(currentDirection, Vector3.right, Vector3.back);
                    var desiredAng = Vector3.SignedAngle(desiredDirection, Vector3.right, Vector3.back);
                    if (currentAng < oppAng) { currentAng += 360; }
                    if (desiredAng < oppAng) { desiredAng += 360; }

                    if (currentAng < desiredAng) { turnToMax = true; }
                    else {turnToMax = false; }

                }
                else //Scan back and forth looking for targets
                {
                    if (turnToMax) { desiredDirection = new Vector3(Mathf.Cos(maxAng * Mathf.Deg2Rad), Mathf.Sin(maxAng * Mathf.Deg2Rad), 0); }
                    else { desiredDirection = new Vector3(Mathf.Cos(minAng * Mathf.Deg2Rad), Mathf.Sin(minAng * Mathf.Deg2Rad), 0); }

                    if (Vector3.Distance(currentDirection,desiredDirection) < 0.1f) 
                    { 
                        currentWaitTime -= Time.deltaTime;
                        if (currentWaitTime < 0)
                        {
                            currentWaitTime = Random.Range(waitToRotate.x, waitToRotate.y);
                            turnToMax = !turnToMax;
                        }
                        
                    }

                }
            }


            //Update aim angle based on relative mouse position
            if (myInputType == InputType.Mouse) 
            {

                Vector3 mousePos = Mouse.current.position.ReadValue();
                mousePos.z = Camera.main.nearClipPlane;
                var mouseScreenPos = Camera.main.ScreenToWorldPoint(mousePos);  mouseScreenPos.z = 0f;

                desiredDirection = (mouseScreenPos - transform.parent.position).normalized;
            }

            //Clamp desired angle within allowable range
            var ang = Vector3.SignedAngle(desiredDirection, Vector3.right, Vector3.back); //Gives 180 above x axis, or -180 for lower half
            if (ang < oppAng) { ang += 360; } //Opposite end of cirle from forward, this is the secret sauce

            //Clamp desired direction within acceptable angle range
            ang = Mathf.Clamp(ang, minAng, maxAng);
            desiredDirection = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad), 0);


            if (turnSpeed < 0) { currentDirection = desiredDirection; } //Instant aim if negaitve value
            else //Move aim over time rather than instantly
            { 
                var forwardDirection = new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad), Mathf.Sin(forwardAngle * Mathf.Deg2Rad), 0);
                bool rotateForwardFirst = false;

                var angDif = Vector3.Angle(currentDirection, desiredDirection);
                
                if (maxRotation > 90 && maxRotation < 180) //handle larger arcs
                {
                    var oppDirection = new Vector3(Mathf.Cos(oppAng * Mathf.Deg2Rad), Mathf.Sin(oppAng * Mathf.Deg2Rad), 0);

                    var angToOpp = Vector3.Angle(currentDirection, oppDirection);

                    if (angDif > angToOpp) //If opposite of forward is closer than destination, rotate to correct forward first
                    { rotateForwardFirst = true; } 
                }

                if (angDif == 180) { rotateForwardFirst = true; } //rotate forward if Half way

                if (turnSpeed * turnSpeedMultiplier > 0)
                {
                    if (rotateForwardFirst) //Turn towards forward direction first if long way to turn
                    { currentDirection = Vector3.RotateTowards(currentDirection, forwardDirection, turnSpeed * turnSpeedMultiplier * Time.deltaTime, 0f); }
                    else { currentDirection = Vector3.RotateTowards(currentDirection, desiredDirection, turnSpeed * turnSpeedMultiplier * Time.deltaTime, 0f); }
                }
                
            }
        }



        currentAng = Vector3.SignedAngle(currentDirection, Vector3.right, Vector3.back);
        transform.localPosition = (currentDirection * lockLength) + offset;
        //transform.parent.position


        //Safety layers disable attacking while aim is touching
        /*
        if (safetyLayers) 
        { 
            if (touching != safetyLayers.touching)
            {
                touching = safetyLayers.touching;

                if (mySprite)
                {
                    if (touching) { mySprite.color = safetyColor; }
                    else { mySprite.color = startColor; }
                }
            }
            
        }
        

        if (touching) 
        {
            lastSeen = null;
            lookLockTime = 0;
            if (myLine) { myLine.enabled = false; }
        }
        else 
        {
            var endPoint = transform.position + (currentDirection * sightRange);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, sightRange, sightLayers);

            if (hit.collider != null) 
            {
                endPoint = hit.point;

                if (hit.transform == lastSeen) { lookLockTime += Time.deltaTime;}
                else { lastSeen = hit.transform; lookLockTime = 0; }
            }
            else { lastSeen = null; lookLockTime = 0; }

            if (myLine) 
            {
                myLine.enabled = true;
                myLine.SetPosition(0, transform.position); 
                myLine.SetPosition(1, endPoint); 
            }

        }
        */

        var endPoint = transform.position + (currentDirection * sightRange);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, sightRange, sightLayers);

        if (hit.collider != null) 
        {
            endPoint = hit.point;

            if (hit.transform == lastSeen) { lookLockTime += Time.deltaTime;}
            else { lastSeen = hit.transform; lookLockTime = 0; }
        }
        else { lastSeen = null; lookLockTime = 0; }

        if (myLine) 
        {
            //myLine.enabled = true;
            myLine.SetPosition(0, transform.position); 
            myLine.SetPosition(1, endPoint); 
        }


        UpdateWeaponAngle();
        
    }

    public void UpdateWeaponAngle()
    {
        if (WeaponPivot) 
        { 
            if (Mathf.Abs(currentAng) < 90) 
            { 
                WeaponPivot.eulerAngles = new Vector3(0, 0, currentAng);
                WeaponPivot.localPosition = weaponPivotStart;
            }
            else //Flip Weapon if on left side
            { 
                WeaponPivot.eulerAngles = new Vector3(180, 0, 360 - currentAng);
                WeaponPivot.localPosition = new Vector3(-weaponPivotStart.x, weaponPivotStart.y, weaponPivotStart.z);
            }
            offset = WeaponPivot.localPosition;



            if (RightHand && RightHandle) 
            { 
                RightHand.position = RightHandle.position;
                RightHand.rotation = RightHandle.rotation;
            }
            if (LeftHand && LeftHandle) 
            { 
                LeftHand.position = LeftHandle.position;
                LeftHand.rotation = LeftHandle.rotation;
            }
                
        }
        
    }


}
