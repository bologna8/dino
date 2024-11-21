using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerCheck : MonoBehaviour
{
     [Tooltip("Choose which layer(s) to check")] public LayerMask checkMask;
     //[Tooltip("VIEW ONLY : currently touching something")] 
     public bool touching;
     /*
     public bool touchingLeft;
     public bool touchingRight;
     public bool touchingTop;
     public bool touchingBot;
     */

     [HideInInspector] public Collider2D lastCollided;
     [HideInInspector] public float slope;

     private Bounds myBounds;

     private SpriteRenderer mySprite;
     private Color startColor;
     [Range(0,1)] public float unselectedSaturation = 0.5f;

     void OnEnable()
     {
          if (myBounds == null) { myBounds = GetComponent<Collider2D>().bounds; }

          if (!mySprite) { mySprite = GetComponent<SpriteRenderer>(); }
          if (mySprite) { startColor = mySprite.color; }
          SetSaturation(unselectedSaturation);
     }

     private void OnTriggerStay2D(Collider2D collision)
     {
          if ((checkMask.value & (1 << collision.transform.gameObject.layer)) > 0) 
          { 
               touching = true; lastCollided = collision;
               slope = collision.transform.rotation.z;
               ExtraEnterOperations(collision);

               var pointOfContact = collision.ClosestPoint(transform.position);

               /*
               if (pointOfContact.x > (transform.position.x )) { touchingRight = true; }
               else { touchingRight = false; }

               if (pointOfContact.x < (transform.position.x )) { touchingLeft = true; }
               else { touchingLeft = false; }

               if (pointOfContact.y > (transform.position.y )) { touchingTop = true; }
               else { touchingTop = false; }
               
               if (pointOfContact.y < (transform.position.y )) { touchingBot = true; }
               else { touchingBot = false; }
               */

               if (mySprite) { mySprite.color = startColor; }
          }    
     }

     public virtual void ExtraEnterOperations(Collider2D collision) {}

     private void OnTriggerExit2D(Collider2D collision)
     {
          if (collision == lastCollided) 
          { 
               ExtraExitOperations(collision); 
               touching = false; 

               /*
               touchingLeft = false;
               touchingRight = false;
               touchingTop = false;
               touchingBot = false;
               */

               SetSaturation(unselectedSaturation);
          }
     }

     private void OnDisable()
     {
          if (lastCollided && touching)
          {
               ExtraExitOperations(lastCollided);
               lastCollided = null;
               touching = false;

               /*
               touchingLeft = false;
               touchingRight = false;
               touchingTop = false;
               touchingBot = false;
               */

               SetSaturation(unselectedSaturation);
          }        
          
     }

     public virtual void ExtraExitOperations(Collider2D collision) {}



     /*
     public Vector3 findTopCorner(bool topR) //Top right or left corner
     {
          if (lastCollided)
          {
               var b = lastCollided.bounds;
               var cornerPoint = lastCollided.transform.position;
               cornerPoint.y += b.extents.y;
               
               if (topR) { cornerPoint.x -= b.extents.x; }
               else { cornerPoint.x += b.extents.x;}

               //lastCollided.ClosestPoint();

               return cornerPoint;
          }

          Debug.Log("No corner found, defaulting to middle");
          return transform.position;
          
     }
     */

     public Vector3 closestCorner(Vector3 position)
     {
          if (lastCollided)
          {
               return lastCollided.ClosestPoint(position);
          }

          Debug.Log("no corner found");
          return transform.position;
     }

     public void SetSaturation(float saturation)
     {
          if (!mySprite) { return; }

          var newColor = Color.grey;
          float grayScale = (newColor.r + newColor.g + newColor.b) / 3;

          newColor.r = Mathf.Lerp(grayScale, startColor.r, saturation);
          newColor.g = Mathf.Lerp(grayScale, startColor.g, saturation);
          newColor.b = Mathf.Lerp(grayScale, startColor.b, saturation);

          mySprite.color = newColor;
     }
}
