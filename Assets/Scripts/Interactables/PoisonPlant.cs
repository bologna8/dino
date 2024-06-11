using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPlant : MonoBehaviour
{
    public float range = 10f;

    public GameObject deathEffect;
    private Weapon myWeapon;

    // Start is called before the first frame update
    void Start()
    {
        myWeapon = GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myWeapon && Save.player)
        {
            if (Vector3.Distance(transform.position, Save.player.position) < range)
            {
                if(!Save.player.GetComponent<PlayerControls>().hidden) { myWeapon.tryAttack(); }
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerControls>();
        if (player)
        {
            if (player.interacting) { Destroy(gameObject); }
        }
    }

    void OnDestroy()
    {
        if (deathEffect) { Instantiate(deathEffect, transform.position, Quaternion.identity); }
    }

}
