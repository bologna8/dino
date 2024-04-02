using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCheck : MonoBehaviour
{
     public bool touching = false;
     [HideInInspector] public Collider2D lastCollided;

     public LayerMask checkMask;

     public void Start()
     {

     }

     public void Update()
     {
          
     }

     private void OnTriggerStay2D(Collider2D collision)
     {
          if ((checkMask.value & (1 << collision.transform.gameObject.layer)) > 0) 
          { touching = true; lastCollided = collision; }        
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
