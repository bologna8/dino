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

    void OnEnable()
    {
        lifeTime = 0f;

        var randX = Random.Range(randStartX.x, randStartX.y);
        var randY = Random.Range(randStartY.x, randStartY.y);
        if (Random.Range(0f, 1f) > 0.5f) { randX *= -1; }

        if (!myBod) { myBod = GetComponent<Rigidbody2D>(); }
        if (myBod) { myBod.AddForce(new Vector2(randX, randY)); }
    }

    void Update()
    {
        if (despawnTime > 0)
        {
            lifeTime += Time.deltaTime;
            if (lifeTime > despawnTime) 
            { gameObject.SetActive(false); }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var playerCheck = other.GetComponent<PlayerControls>();
        if (playerCheck && lifeTime >= delayPickupTime)
        {
            if (item)
            {
                if (Inventory.instance.ContainsItem(item, 9)) { return; } //Can't pickup if already have too many

                Inventory.instance.AddItem(item);
                InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
                if (inventoryUI != null)
                {
                    inventoryUI.UpdateInventoryUI();
                }
            }

            
            if (recipe)
            {
                InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
                if (inventoryUI != null)
                {
                    inventoryUI.UpdateCraftingUI(recipe);
                }
            }

            if (pickupEffect)
            {
                PoolManager.Instance.Spawn(pickupEffect, transform.position);
            }
            
            gameObject.SetActive(false);
        }
    }
}
