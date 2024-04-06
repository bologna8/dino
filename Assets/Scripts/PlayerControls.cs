using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public GameObject aimingPrefab;
    private Movement myMove;
    private Weapon[] myWeapons;
    private Health myHealth;

    // Start is called before the first frame update
    void Start()
    {
        myMove = GetComponent<Movement>();
        myWeapons = GetComponents<Weapon>();
        myHealth = GetComponentInChildren<Health>();
        myHealth.team = gameObject.layer;

        if (aimingPrefab)
        {
            var myAim = Instantiate(aimingPrefab).GetComponent<Aim>();
            myAim.lockT = transform;

            foreach (Weapon w in myWeapons) { w.myAim = myAim; }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (myHealth.stunTime > 0) //Stunned
        {
            myMove.moveInput = 0;
            myMove.verticalInput = 0;
        }
        else //not stunned, can input
        {
            myMove.moveInput = (int)Input.GetAxis("Horizontal");
            myMove.verticalInput = (int)Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump")) { myMove.Jump(); }
            if (Input.GetButtonDown("Dash")) { myMove.Dash(); }

            if (Input.GetButton("Primary")) { myWeapons[0].tryAttack(); }
            else if (Input.GetButton("Secondary")) { myWeapons[1].tryAttack(); }
        }
        
        
        /*
        var attacking = false;
        foreach (Weapon w in myWeapons) { if (w.attacking) { attacking = true; } }

        if(!attacking)
        {
            if (Input.GetButton("Primary")) { myWeapons[0].tryAttack(); }
            else if (Input.GetButton("Secondary")) { myWeapons[1].tryAttack(); }
        }
        */
        
    }
}
