using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float respawnTime = 0f;
    public float maxHP = 10f;
    public float currentHP;

    [HideInInspector] public float stunTime;
    [HideInInspector] public float invincibleTime;

    //[HideInInspector] public bool faceRight;

    [HideInInspector] public Movement myMovement;

    private TrailRenderer hitTrail;
    //private SpriteRenderer mySprite;

    //private int shakeThreshold = 1111;

    // Start is called before the first frame update
    void Start()
    {
        myMovement = GetComponentInParent<Movement>();
        //myAct = GetComponentInParent<Actions>();

        currentHP = maxHP;
        stunTime = respawnTime;

        hitTrail = GetComponent<TrailRenderer>();
        if (hitTrail) { hitTrail.enabled = false; }

    }

    // Update is called once per frame
    void Update()
    {
        //if (!mySprite) { mySprite = moveMe.mySprite; startMat = mySprite.material; }

        if (invincibleTime > 0) { invincibleTime -= Time.deltaTime; }
        if (stunTime > 0) { stunTime -= Time.deltaTime; }
         
    }

    public void TakeDamage(float dmg, float stun, Vector2 KB)
    {
        currentHP -= dmg;
        if (stunTime <= stun) { stunTime = stun; }

        if (myMovement) { myMovement.myBod.AddForce(KB); }
        
    }

    


}
