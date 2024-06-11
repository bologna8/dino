using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerCheck : MonoBehaviour
{
     [Tooltip("Choose which layer(s) to check")] public LayerMask checkMask;
     [Tooltip("VIEW ONLY : currently touching something")] public bool touching = false;
     [Tooltip("VIEW ONLY : what layer was last touched")] public int touchingLayer = -1; //not used currently
     [HideInInspector] public Collider2D lastCollided;

     private void OnTriggerStay2D(Collider2D collision)
     {
          if ((checkMask.value & (1 << collision.transform.gameObject.layer)) > 0) 
          { touching = true; lastCollided = collision; touchingLayer = collision.transform.gameObject.layer; }    
     }

     private void OnTriggerExit2D(Collider2D collision)
     {
          if (collision == lastCollided) { touching = false; }
     }

     public Vector3 findTopCorner(bool topR)
     {
          var b = lastCollided.bounds;
          if (topR) { return new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, 0); }
          else { return new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, 0); }
     }
}
