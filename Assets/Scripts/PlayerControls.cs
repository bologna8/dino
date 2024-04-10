using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public GameObject aimingPrefab;
    private Movement myMove;
    private Weapon[] myWeapons;
    private Health myHealth;

    private Animator myAnim;
    [HideInInspector] public bool interacting = false;

    // Start is called before the first frame update
    void Start()
    {
        myMove = GetComponent<Movement>();
        myWeapons = GetComponents<Weapon>();
        myHealth = GetComponentInChildren<Health>();
        myHealth.team = gameObject.layer;

        myAnim = GetComponentInChildren<Animator>();

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
            interacting = false;
        }
        else //not stunned, can input
        {
            myMove.moveInput = (int)Input.GetAxis("Horizontal");
            myMove.verticalInput = (int)Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump")) { myMove.Jump(); }

            if (!myMove.onEdge)
            {
                if (Input.GetButtonDown("Dash")) { myMove.Dash(); }

                if (Input.GetButton("Primary")) { myWeapons[0].tryAttack(); }
                else if (Input.GetButton("Secondary")) { myWeapons[1].tryAttack(); }

                if (Input.GetButton("Interact")) { interacting = true; }
                else { interacting = false; }
            }
            
        }
        
    }

    public void Dig(float digTime)
    {
        StartCoroutine(StartDigging(digTime));
    }

    IEnumerator StartDigging(float digTime)
    {
        myHealth.TakeDamage(0, digTime, Vector3.zero);
        interacting = false;
        if (myAnim) { myAnim.SetTrigger("dug"); }
        yield return new WaitForSeconds(digTime);
    }
}
