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
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); mousePos.z = 0f;
                direction = (mousePos - lockT.position).normalized;

            }
            if (myType == FollowType.Controller) 
            {
                
            }

            transform.position = lockT.position + (direction * lockLength) + offset;
        }
        else { Destroy(gameObject); }
        

    }
}
