using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    
    //public bool hooked = true;

    private DistanceJoint2D joint; //dope
    private LineRenderer line;

    [HideInInspector] public Transform origin;

    // Start is called before the first frame update
    void Start()
    {
        joint = GetComponent<DistanceJoint2D>();
        line = GetComponent<LineRenderer>();

        if (origin) { joint.connectedBody = origin.GetComponent<Rigidbody2D>(); }

    }

    // Update is called once per frame
    void Update()
    {
        if (line && origin)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, origin.position);
        }

        if (Input.GetButtonDown("Jump")) { Destroy(gameObject); } //Temp for testing

    }

}
