using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerCheck : MonoBehaviour
{
     [Tooltip("Choose which layer(s) to check")] public LayerMask checkMask;
     //[Tooltip("VIEW ONLY : currently touching something")] 
     public bool touching = false;

     [HideInInspector] public Collider2D lastCollided;
     [HideInInspector] public float slope;

     private void OnTriggerStay2D(Collider2D collision)
     {
          if ((checkMask.value & (1 << collision.transform.gameObject.layer)) > 0) 
          { 
               touching = true; lastCollided = collision;
               slope = collision.transform.rotation.z;
               ExtraEnterOperations(collision);
          }    
     }

     public virtual void ExtraEnterOperations(Collider2D collision) {}

     private void OnTriggerExit2D(Collider2D collision)
     {
          if (collision == lastCollided) 
          { ExtraExitOperations(collision); touching = false; }
     }

     private void OnDisable()
     {
          if (lastCollided && touching)
          {
               ExtraExitOperations(lastCollided);
               lastCollided = null;
               touching = false;
          }        
          
     }

     public virtual void ExtraExitOperations(Collider2D collision) {}



     public Vector3 findTopCorner(bool topR) //Top right or left corner
     {
          if (lastCollided)
          {
               var b = lastCollided.bounds;
               var cornerPoint = lastCollided.transform.position;
               cornerPoint.y += b.extents.y;
               
               if (topR) { cornerPoint.x -= b.extents.x; }
               else { cornerPoint.x += b.extents.x;}

               return cornerPoint;
          }

          Debug.Log("No corner found, defaulting to middle");
          return transform.position;
          
     }
}
