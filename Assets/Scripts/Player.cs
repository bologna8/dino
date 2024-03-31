using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
     public float moveSpeed;
     public float maxRunSpeed;
     public float jumpForce;

     public CheckCheck[] colliders;
     public GameObject mySelf;

     public GameObject boxPrefab;

     public Vector2 throwForce;
     public Vector2 throwUp;
     public float pullSpeed;

     private bool pulling = false;
     private float dropDelay;
     private float dropDelayMax = 0.69f;
     private bool boxGrounded;
     private bool boxStuck;

     private int moving;
     private bool faceRight = true;
     private bool onGround = false;
     private Rigidbody2D myBod;
     private float startGrav;
     private bool onEdge;
     private bool turning = false;
     private float turnTime = 0.123f;
     //private bool turnAgain = false;
     private float turnBackTime = 0.1f;
     private float turnBack;

     private GameObject boxBack;
     [HideInInspector]
     public GameObject boxThrown;
     private Rigidbody2D boxBod;
     private bool onBack = false;
     private float boxAirTime;
     private bool canPull = true;

     public float leashLength;
     [HideInInspector]
     public float currentLength;
     public Vector2 camMinMax;
     private float camSize;
     private GameObject leashLethal;
     private Vector3 camTarget;



     // Start is called before the first frame update
     void Start()
     {
          myBod = GetComponent<Rigidbody2D>();
          startGrav = myBod.gravityScale;

          leashLethal = transform.Find("LeashLethal").gameObject;

          mySelf = transform.Find("myGuy").gameObject;

          boxBack = mySelf.transform.Find("BackBox").gameObject;
          colliders = mySelf.transform.GetComponentsInChildren<CheckCheck>();

          if (!onBack)
          {
               boxBack.SetActive(false);
               boxThrown = GameObject.Find("Box");
               boxBod = boxThrown.GetComponent<Rigidbody2D>();
          }
     }

     // Update is called once per frame
     void Update()
     {
          //if (turnAgain) { turnAgain = false; Turn(); }

          if (Input.GetButtonDown("Left") && faceRight) { Turn(); }
          if (Input.GetButtonDown("Right") && !faceRight) { Turn(); }

          moving = 0;
          if (Input.GetButton("Left")) 
          { 
               if (!faceRight) { moving = -1; }
               else if (!pulling) { reTurn(); }
          }
          if (Input.GetButton("Right")) 
          {
               if (faceRight) { moving = 1; }
               else if (!pulling) { reTurn(); }
          }

          onGround = colliders[0].touching;

          if (boxThrown) 
          { 
               //boxGrounded = boxThrown.GetComponentsInChildren<CheckCheck>()[0].touching;
               if (Mathf.Abs(boxBod.velocity.y) <= 0.1f) { boxGrounded = true; } else { boxGrounded = false; }

               boxStuck = boxThrown.GetComponentsInChildren<CheckCheck>()[1].touching;
               if (boxGrounded) { boxAirTime = 0; }
               else { boxAirTime += Time.deltaTime; }
               LeashEnd();
          }
          else { boxGrounded = false; LeashClear(); }
          

          if (Input.GetButtonDown("Jump"))
          {
               if (!onBack) { if (!pulling) { Jump(); } }

               else { ThrowBox(); }
          }

          if (Input.GetButton("Down"))
          {
               if (dropDelay <= 0)
               {
                    if (onBack) { DropBox(); }
                    //else if (onEdge) { tryGrab = false; }
                    else { PullBox(); }
               }                              
          }
          else if (pulling) { ReleaseBox(); }

          if (dropDelay > 0 && !pulling) { dropDelay -= Time.deltaTime; }

          ledgeGrab();

          CameraCrew();
     }

     private void FixedUpdate()
     {
          if (moving == 0) { if (onGround) { myBod.velocity = new Vector2(myBod.velocity.x * 0.9f, myBod.velocity.y); } }
          else if (!pulling) { Run(); }
     }

     void CameraCrew()
     {
          Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, camTarget, 0.42f);
          
          if (Camera.main.orthographicSize != camSize) { Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, camSize, 1); }
     }

     void Turn()
     {
          if (!turning)
          {
               faceRight = !faceRight; onEdge = false;
               myBod.velocity = new Vector2(myBod.velocity.x * 0.42f, myBod.velocity.y);

               StartCoroutine(TurnTime());
          } 
          //else { turnAgain = true; }          
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

          turnBack = turnBackTime;
     }

     void reTurn()
     {
          if (Input.GetButton("Left") && Input.GetButton("Right")) { }
          else
          {
               if (turnBack <= 0) { Turn(); }
               else { turnBack -= Time.deltaTime; }
          }          
     }

     void Run()
     {
          var max = maxRunSpeed; var spd = moveSpeed;
          if (onBack) { max *= 0.69f; spd *= 0.69f; }

          if (Mathf.Abs(myBod.velocity.x) < max)
          { 
               
               if (colliders[1].touching)
               {
                    if (onGround) { if (!colliders[2].touching) { GetOverIt(0.69f); } }
                    else if (colliders[2].touching && !colliders[3].touching) { GetOverIt(0.69f); }
               }

               float moveX = spd * moving;
               myBod.AddForce(Vector2.right * moveX);

          }
     }

     void GetOverIt(float f)
     {
          if (!turning)
          {
               myBod.velocity = new Vector2(myBod.velocity.x, 0);
               myBod.AddForce(Vector2.up * jumpForce * f);
          }          
     }

     void Jump()
     {
          if (onGround || onEdge)
          {
               myBod.velocity = new Vector2(myBod.velocity.x, 0);
               myBod.AddForce(Vector2.up * jumpForce);
          } 
          else if (colliders[2].touching || colliders[3].touching)
          {
               Turn();
               myBod.velocity = new Vector2(0, 0);

               var f = 1; if (!faceRight) { f = -1; }
               myBod.AddForce(new Vector2(f*jumpForce * 0.69f, jumpForce));
          }
               
     }

     void ThrowBox()
     {
          if (onBack)
          {
               onBack = false; boxBack.SetActive(false); boxAirTime = 0;
               boxThrown = Instantiate(boxPrefab, new Vector3(transform.position.x, transform.position.y + 5, 0), transform.rotation);
               boxBod = boxThrown.GetComponent<Rigidbody2D>();

               var thrown = throwForce;

               if (Input.GetButton("Up")) { thrown = throwUp; }

               if (moving != 0) { thrown = new Vector2(thrown.x * 1.69f, thrown.y); }

               if (!faceRight) { thrown = new Vector2(-thrown.x, thrown.y); }

               boxBod.AddForce(thrown);
          }
     }

     void PullBox()
     {
          if (boxStuck) { ReleaseBox(); }
          else if(canPull)
          {
               if (pulling)
               {
                    moving = 0;

                    if (Vector2.Distance(transform.position, boxThrown.transform.position) < 2.1f) { CatchBox(); }

                    else
                    {
                         boxBod.isKinematic = true; boxBod.velocity = Vector2.zero;
                         pulling = true; boxThrown.GetComponent<BoxCollider2D>().isTrigger = true; 
                         boxThrown.transform.position = Vector2.MoveTowards(boxThrown.transform.position, boxBack.transform.position, pullSpeed * Time.deltaTime);
                    }

                    if (faceRight && transform.position.x > boxThrown.transform.position.x) { Turn(); }
                    if (!faceRight && transform.position.x < boxThrown.transform.position.x) { Turn(); }
               }
               else if (boxGrounded && onGround) { pulling = true; }
               else { pulling = false; }
          }          
     }

     void CatchBox()
     {
          onBack = true; boxBack.SetActive(true); Destroy(boxThrown); pulling = false; dropDelay = dropDelayMax;
     }

     void DropBox()
     {
          if (onBack)
          {
               onBack = false; boxBack.SetActive(false); dropDelay = dropDelayMax;
               boxThrown = Instantiate(boxPrefab, boxBack.transform.position, transform.rotation);
               boxBod = boxThrown.GetComponent<Rigidbody2D>();
          }
     }

     void ReleaseBox()
     {
          pulling = false; 
          if (boxThrown) 
          { 
               boxBod.isKinematic = false; 
               boxThrown.GetComponent<BoxCollider2D>().isTrigger = false;
          }

     }

     void LeashEnd()
     {
          leashLethal.SetActive(true);

          var dir = (transform.position - boxThrown.transform.position);
          var pos = boxThrown.transform.position + (dir.normalized * leashLength);
          leashLethal.transform.position = pos;

          var angle = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward);
          leashLethal.transform.eulerAngles = (new Vector3(0, 0, angle));

          currentLength = Vector2.Distance(transform.position, boxThrown.transform.position);
          if (currentLength > leashLength) { Respawn(); }

          var mid = transform.position + boxThrown.transform.position;
          camTarget = new Vector3 (mid.x / 2, (mid.y / 2) +2, -10);

          camSize = camMinMax.x + ((camMinMax.y - camMinMax.x) * (currentLength / leashLength));
     }

     void LeashClear()
     {
          leashLethal.SetActive(false);
          camTarget = new Vector3(transform.position.x, transform.position.y + 4, -10);

          camSize = camMinMax.x;
     }

     void Die()
     {

     }

     void Respawn()
     {
          transform.position = boxThrown.transform.position + new Vector3(0, 4, 0);
          canPull = true;
     }

     private void OnTriggerStay2D(Collider2D collision)
     {
          if(collision.name == "Box" || collision.name == "Box(Clone)") 
          {
               if (pulling) { CatchBox(); }
               else if (boxAirTime > 0.1f && boxAirTime != 0) { CatchBox(); }
          }
     }

     private void ledgeGrab()
     {
          
          bool tryGrab = true;
          if (Input.GetButton("Down") && dropDelay <= 0) { tryGrab = false; }
          if (turning) { tryGrab = false; }
          //if (Input.GetButton("Left") && !faceRight) { tryGrab = false; }
          //if (Input.GetButton("Right") && faceRight) { tryGrab = false; }
          
          if (!onGround && colliders[4].touching && !colliders[5].touching && tryGrab)
          { 
               myBod.gravityScale = 0; onEdge = true;
               myBod.velocity = new Vector2(myBod.velocity.x, 0);

               var cornerHit = colliders[4].findTopCorner(faceRight);
               var offset = colliders[4].transform.localPosition;
               if (!faceRight) { offset = new Vector3(offset.x *-1, offset.y,0); }
               transform.position = cornerHit - offset;
          }

          else { myBod.gravityScale = startGrav; onEdge = false; }          
     }

}
