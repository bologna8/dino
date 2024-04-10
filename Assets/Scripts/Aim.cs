using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Automatic Interface Modifier
public class Aim : MonoBehaviour
{
    public enum FollowType { Auto, Mouse, Controller };
    public FollowType myType;

    public Vector3 offset;
    public float lockLength = 1f;
    [HideInInspector] public Transform lockT;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public AI myAI;


    private Vector3 lastMousePos;
    private Vector3 lastControllerAim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lockT)
        {
            if (myType == FollowType.Auto && myAI) 
            {
                if (myAI.chasing) { direction = (myAI.chasing.position - lockT.position).normalized; }   
            }
            if (myType == FollowType.Mouse) 
            {
                lastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); lastMousePos.z = 0f;
                direction = (lastMousePos - lockT.position).normalized;

                if (Input.GetAxis("aimX") != 0 || Input.GetAxis("aimY") != 0) 
                { myType = FollowType.Controller; }

            }
            if (myType == FollowType.Controller) 
            {
                var mousePos = Input.mousePosition;  mousePos.z = 0f;
                if (mousePos != lastMousePos) { myType = FollowType.Mouse; }

                if (Input.GetAxis("aimX") != 0 || Input.GetAxis("aimY") != 0)
                { lastControllerAim = new Vector3(Input.GetAxis("aimX"), Input.GetAxis("aimY"), 0f).normalized; }
                direction = lastControllerAim;
                //Debug.Log(lastControllerAim);
                
            }

            //Debug.Log(Input.GetAxis("aimX") + " , " + Input.GetAxis("aimY"));
            transform.position = lockT.position + (direction * lockLength) + offset;
        }
        else { Destroy(gameObject); }
        

    }
}
