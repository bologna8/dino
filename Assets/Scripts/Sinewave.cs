using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sinewave : MonoBehaviour
{
     public Player myGuy;

     public int points = 42;
     public float spd;
     public float amp;
     public float jag;

     public bool flipped;

     private LineRenderer myLine;

    // Start is called before the first frame update
    void Start()
    {
          myLine = GetComponent<LineRenderer>();
    }

     private void Draw()
     {
          var dir = (myGuy.boxThrown.transform.position - transform.position);
          var angle = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward);
          transform.eulerAngles = (new Vector3(0, 0, angle));

          float xStart = 0;
          //float Tau = 2 * Mathf.PI;
          float xFinish = myGuy.currentLength;
          float stretch = 2 - (myGuy.currentLength / myGuy.leashLength);

          myLine.positionCount = points;
          for (int currentPoint = 0; currentPoint < points; currentPoint ++)
          {
               float progress = (float)currentPoint / (points - 1);
               float x = Mathf.Lerp(xStart, xFinish, progress);

               float r = Random.Range(-jag, jag);
               float y = r + Mathf.Sin((x)+(Time.timeSinceLevelLoad *spd)) * amp * stretch;
               if (flipped) { y *= -1; }
               myLine.SetPosition(currentPoint, new Vector3(x,y,0));
          }
     }

     // Update is called once per frame
     void FixedUpdate()
    {
          if (!myGuy.boxThrown) { myLine.positionCount = 0; }
          else { Draw(); }
    }
}
