using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerCheck : MonoBehaviour
{
     [Tooltip("Choose which layer(s) to check")] public LayerMask checkMask;
     [Tooltip("VIEW ONLY : currently touching something")] public bool touching = false;
     [Tooltip("VIEW ONLY : what layer was last touched")] public int touchingLayer = -1; //not used currently
     [HideInInspector] public Collider2D lastCollided;
     [HideInInspector] public float slope;

     private void OnTriggerStay2D(Collider2D collision)
     {
          if ((checkMask.value & (1 << collision.transform.gameObject.layer)) > 0) 
          { 
               touching = true; lastCollided = collision; 
               touchingLayer = collision.transform.gameObject.layer;
               slope = collision.transform.rotation.z;
          }    
     }

     private void OnTriggerExit2D(Collider2D collision)
     {
          if (collision == lastCollided) { touching = false; }
     }

     public Vector3 findTopCorner(bool topR) //Top right or left corner
     {
          var b = lastCollided.bounds;
          var cornerPoint = lastCollided.transform.position;
          cornerPoint.y += b.extents.y;
          
          if (topR) { cornerPoint.x -= b.extents.x; }
          else { cornerPoint.x += b.extents.x;}

          return cornerPoint;
     }
}
