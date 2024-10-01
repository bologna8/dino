using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollBoulder : MonoBehaviour
{
    public Rigidbody2D RB;
    public float vel = -2;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (!RB.IsSleeping())
        {
            RB.velocity = new Vector2(vel, RB.velocity.y );
        }
    }
}
