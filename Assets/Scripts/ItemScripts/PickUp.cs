using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public GameObject pickupEffect;
    public Vector2 randStartY;
    public Vector2 randStartX;

    public float despawnTime = 120f;
    public float delayPickupTime = 0.1f;
    private float lifeTime;
    private Rigidbody2D myBod;

    public Item item;
    public CraftingRecipe recipe;

    // Start is called before the first frame update
    void Start()
    {
        var randX = Random.Range(randStartX.x, randStartX.y);
        var randY = Random.Range(randStartY.x, randStartY.y);
        if (Random.Range(0f,1f) > 0.5f) { randX *= -1; }

        myBod = GetComponent<Rigidbody2D>();
        if (myBod) { myBod.AddForce(new Vector2(randX, randY)); }
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime > despawnTime) { gameObject.SetActive(false); }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && lifeTime > delayPickupTime)
        { 
            if (pickupEffect) 
            { PoolManager.Instance.Spawn(pickupEffect, transform.position); } 

            if (item) { Inventory.instance.AddItem(item); }

            //Do the same for recipies somehow pls Angelina my angel 0w0
            //if (recipe) { intentory.UnlockRecipe(recipe); } 

            gameObject.SetActive(false);
        }
        
    }

}
