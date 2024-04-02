using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    private Movement myMove;
    // Start is called before the first frame update
    void Start()
    {
        myMove = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        myMove.moveInput = (int)Input.GetAxis("Horizontal");
        myMove.verticalInput = (int)Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump")) { myMove.Jump(); }
    }
}
