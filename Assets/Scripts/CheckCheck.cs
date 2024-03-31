using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCheck : MonoBehaviour
{
     public bool touching = false;

     private Collider2D lastCollided;
     private Collider2D myBox;

     //public LayerMask checkMask;
     public int collisionLayer;

    // Start is called before the first frame update
    void Start()
    {
          myBox = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     private void OnTriggerStay2D(Collider2D collision)
     {
          //myBox.IsTouchingLayers(checkMask)
          if (collision.gameObject.layer == collisionLayer)
          {
               touching = true;
               lastCollided = collision;
          }          
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
