using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public enum State { idle, patrol };
    public State currentState;
    private Movement myMovement; 


    // Start is called before the first frame update
    void Start()
    {
        myMovement = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == State.idle && myMovement) { myMovement.moveInput = 0; }
        if (currentState == State.patrol) { Patrol(); }
    }

    void Patrol()
    {
        if (myMovement)
        {
            if (myMovement.colliders[1].touching) { myMovement.Turn(); }
            if (myMovement.onGround && !myMovement.colliders[4].touching) { myMovement.Turn(); }
            
            if (myMovement.faceRight) { myMovement.moveInput = 1; }
            else { myMovement.moveInput = -1; }
            
        }
    }


}
