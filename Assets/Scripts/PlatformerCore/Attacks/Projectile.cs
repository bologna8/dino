using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile stats")]
    [Tooltip("Considered melee attack if 0")] public float projectileSpeed = 100;
    [Tooltip("How long projectile goes before self destructing")] public float lifetime = 1f;
    private float currentTime;
    [Tooltip("Break when hitting any of these layers")] public LayerMask breakLayer;
    [Tooltip("Spawn effect when attack hits an object in the breakLayer")] public GameObject breakEffect;


    [Header("Boomerang")]
    [Tooltip("Does not bounce back if 0")] public float returnTime;
    private float returnCurrent; //keep track of current time until boomerang returns
    [Tooltip("Speed sprite rotates")] public Vector3 rotateSpeed = new Vector3(0,0, 420);
    private bool bounced; //boomerang bounced back yet
    private Vector3 endPosition; //Point boomerang bounced back from

    //Other hidden stats
    private Spawned mySpawn;
    private Rigidbody2D myBod; //nice bod

    private TrailRenderer myTrail;



    void OnEnable()
    {
        if (!myBod) { myBod = GetComponent<Rigidbody2D>(); }

        if (!mySpawn) { mySpawn = GetComponentInParent<Spawned>(); }
        if (mySpawn) //Reset on every awake
        { 
            //team = mySpawn.team; 
            //source = mySpawn.source;

            if(mySpawn.tracking && mySpawn.myAim) 
            {
                transform.position = mySpawn.myAim.transform.position;
                transform.rotation = Quaternion.FromToRotation(Vector3.right, mySpawn.myAim.currentDirection);
            }
        }



        currentTime = lifetime;
        //faceRight = true;
        //if (transform.rotation.eulerAngles.z % 360 > 180) { faceRight = false; Debug.Log(transform.rotation.eulerAngles.z % 360); }

        if (!myTrail) { myTrail = GetComponent<TrailRenderer>(); }
        if (myTrail) { myTrail.Clear(); }

        if (projectileSpeed != 0f) 
        { 
            myBod.velocity = Vector3.zero;
            myBod.AddForce(transform.right * projectileSpeed); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lifetime > 0)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0) 
            { 
                if (returnTime > 0f && !bounced) { BounceBack(); }
                else { gameObject.SetActive(false); }
                //else { Destroy(gameObject); }
            }
        }
        

        //Boomerang stuff
        if (returnTime > 0f) { transform.Rotate(rotateSpeed * Time.deltaTime); } //Rotato Potato

        if (bounced && mySpawn) //Return to sender
        {
            returnCurrent += Time.deltaTime;
            //transform.position = Vector3.Lerp(endPosition, origin.position, returnCurrent / returnTime);
            transform.position = Vector3.Lerp(endPosition, mySpawn.source.position, returnCurrent / returnTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((breakLayer.value & (1 << other.transform.gameObject.layer)) > 0) 
        {
            if (returnTime > 0f && !bounced) { BounceBack(); }
            else //Break if not a boomerang
            { 
                if (breakEffect) 
                { 
                    var tempTeam = 0;
                    Transform tempSource = null;
                    if (mySpawn) { tempTeam = mySpawn.team; }
                    PoolManager.Instance.Spawn(breakEffect, other.ClosestPoint(transform.position), transform.rotation, tempSource, tempTeam); 
                }
                gameObject.SetActive(false);
            }            
        }

        if (mySpawn && bounced)
        { if(other.transform == mySpawn.source ) { gameObject.SetActive(false); } }

    }

    public void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void BounceBack()
    { 
        currentTime = returnTime;
        bounced = true;
        endPosition = transform.position;
        //hitList.Clear();
        Flip();
    }


}
