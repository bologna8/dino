using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineHandler : MonoBehaviour
{

    private LineRenderer myLine;
    private Spawned mySpawn;
    private Aim myAim;

    [HideInInspector] public Vector2 startPoint;
    [HideInInspector] public Vector2 endPoint;

    public bool findNewEndpoint;
    
    [Tooltip("What angle to send a ray in search of nearest end point")] public float searchAngle;
    [Tooltip("X is how far to send a ray, Y is how wide a beam the ray searches")] public Vector2 searchRange;
    [Tooltip("What layers to stop at when searching for an end point")] public LayerMask searchLayers;

    void OnEnable()
    {
        startPoint = transform.position;
        var aimedAngle = searchAngle;

        if (!myLine) { myLine = transform.GetComponent<LineRenderer>(); }
        
        if (!mySpawn) { mySpawn = transform.GetComponent<Spawned>(); }
        if (mySpawn) 
        { 
            if (mySpawn.myCore) 
            {
                if (!mySpawn.myCore.movingRight) { aimedAngle = 180 - searchAngle; }

                if (mySpawn.myCore.myAim) { myAim = mySpawn.myCore.myAim; } 
            } 
        }

        if (findNewEndpoint)
        {
            bool didHit = false;
            var dir = new Vector2(Mathf.Cos(aimedAngle * Mathf.Deg2Rad), Mathf.Sin(aimedAngle * Mathf.Deg2Rad));
            RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, dir, searchRange.x);
            Vector2 tempPoint = new Vector2(99999, 99999);
            foreach (RaycastHit2D hit in hits) {
                if (hit.collider.CompareTag("Collision Tiles"))
                {
                    if (didHit)
                    {
                        tempPoint = endPoint;
                    } 
                    endPoint = startPoint + (dir * hit.distance);
                    Debug.Log("hit");
                    didHit = true;
                }
                if(Vector2.Distance(startPoint,tempPoint)< Vector2.Distance(startPoint, endPoint))
                {
                    endPoint = tempPoint;
                }
            }
            if (!didHit)
            {
                endPoint = startPoint + (dir * searchRange.x);
            }
            

            //RaycastHit2D checkHits = Physics2D.CircleCast(endPoint, searchRange.y, Vector2.zero, 0, searchLayers);
            //if (checkHits.point != null) { endPoint = checkHits.point; }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myLine)
        {
            myLine.positionCount = 2;

            if (mySpawn) 
            {
                if (myAim) 
                { 
                    startPoint = myAim.transform.position;
                    myAim.attackAngleOffset = mySpawn.angle;
                }
                else { startPoint = mySpawn.source.position; }
            }
            else { startPoint = transform.position;}

            myLine.SetPosition(0, startPoint);
            myLine.SetPosition(1, endPoint);
        }
        
        if (myAim && findNewEndpoint) 
        { myAim.attackAngleOffset = searchAngle; }

    }

    void OnDisable()
    {
        if (myAim && findNewEndpoint)
        { myAim.attackAngleOffset = 0;}
    }
}
