using System;
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
    private SpriteRenderer mySprite;
    [HideInInspector] public bool interacting = false;
    [HideInInspector] public List<Bush> bushesTouched = new List<Bush>();
    [HideInInspector] public bool hidden = false;
    public GameObject hideEffect;

    // Event to notify when a recipe is unlocked
    public static event Action<CraftingRecipe> onRecipeUnlocked;

    private List<CraftingRecipe> unlockedRecipes = new List<CraftingRecipe>();
    
    public void UnlockRecipe(CraftingRecipe recipe)
    {
        if(!unlockedRecipes.Contains(recipe))
        {
            unlockedRecipes.Add(recipe);
            Debug.Log("Recipe unlocked " + recipe.name);

            // Trigger the event when a recipe is unlocked
            onRecipeUnlocked?.Invoke(recipe);
        }
        else{
            Debug.Log("Recipe already unlocked "+recipe.name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myMove = GetComponent<Movement>();
        myWeapons = GetComponents<Weapon>();
        myHealth = GetComponentInChildren<Health>();
        myHealth.team = gameObject.layer;

        myAnim = GetComponentInChildren<Animator>();
        mySprite = GetComponentInChildren<SpriteRenderer>();

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
            hidden = false;
        }
        else //not stunned, can input
        {
            myMove.moveInput = (int)Input.GetAxis("Horizontal");
            myMove.verticalInput = (int)Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump")) { myMove.Jump(); }

            if (!myMove.onEdge)
            {
                if (Input.GetButtonDown("Dash")) 
                { 
                    myMove.Dash();
                }

                if (Input.GetButton("Primary")) { myWeapons[0].tryAttack(); }
                else if (Input.GetButton("Secondary")) { myWeapons[1].tryAttack(); }

                
                if (Input.GetButton("Interact")) { interacting = true; }
                else { interacting = false; }

                //Bush stuff
                if (bushesTouched.Count > 0) 
                { 
                    if (Input.GetButtonDown("Interact")) 
                    { 
                        hidden = !hidden; 
                        if(hidden && hideEffect) { Instantiate(hideEffect, transform.position, Quaternion.identity); } 
                    } 
                }
                else if (hidden) { hidden = false;}

            }
            
        }

        //stealth stuff
        myMove.slowPercent = 1f;
        if (hidden) { myMove.slowPercent = 0.25f; }

        if(mySprite) 
        {
            if(hidden) { mySprite.sortingOrder = -10; }
            else { mySprite.sortingOrder = 0; }
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
