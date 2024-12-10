using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundType : MonoBehaviour
{
    public enum Type {grass, stone}
    public Type type;

    void Start()
    {
        
    }

    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D col){
        if(col.transform.gameObject.tag == "Player"){
            
            if(col.transform.gameObject.GetComponent<DetectGround>() != null){
                col.transform.gameObject.GetComponent<DetectGround>().standingOn = type;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col.transform.gameObject.tag == "Player"){
            
            if(col.transform.gameObject.GetComponent<DetectGround>() != null){
                col.transform.gameObject.GetComponent<DetectGround>().standingOn = type;
            }
        }
    }
}
