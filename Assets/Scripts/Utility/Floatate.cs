using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Float and rotate
public class Floatate : MonoBehaviour
{

      public float rotateSpeed = 25f;
      public bool spinWise = true;

      public float bobAmount = 0.5f;
      public float bobSpeed = 0.5f;
      private float bobCurrent = 0f;
      private bool up = true;
      private Vector3 startPos;

      public LayerCheck groundCheck;

      // Start is called before the first frame update
      void Start()
      {
            groundCheck = GetComponentInParent<LayerCheck>();
            startPos = transform.localPosition;
      }

      // Update is called once per frame
      void FixedUpdate()
      {
            if (groundCheck.touching)
            {
                  if (spinWise) { transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime); }
                  else { transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime); }


                  if (Mathf.Abs(bobCurrent) > bobAmount) { up = !up; } //that's whats up 

                  if (up) { bobCurrent += bobSpeed * Time.deltaTime; }
                  else { bobCurrent -= bobSpeed * Time.deltaTime; }

                  transform.localPosition = new Vector3(transform.localPosition.x, startPos.y + bobCurrent, transform.localPosition.z);
            }    

      }
}
