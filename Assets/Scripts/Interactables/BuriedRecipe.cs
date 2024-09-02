using System.Collections;
using UnityEngine;

public class BuriedRecipe : Interactable
{
    public GameObject pickupEffectPrefab;
    public CraftingRecipe[] BuriedRecipes;

    public float timeToDig = 1f;
    [HideInInspector] public bool playerTouching = false;

    void Update()
    {
        if (player)
        {
            if (player.interacting)
            {
                player.Dig(timeToDig);
                StartCoroutine(delayedDig(timeToDig, player));
            }
        }
    }


    void OnTriggerStay2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerControls>();
        if (player)
        {
            playerTouching = true;
            if (player.interacting)
            {
                player.Dig(timeToDig);
                StartCoroutine(delayedDig(timeToDig, player));
            }
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerControls>();
        if (player && playerTouching) { playerTouching = false; }
    }


    IEnumerator delayedDig(float digTime, PlayerControls player)
    {
        yield return new WaitForSeconds(digTime);

        if (player)
        {
            if(BuriedRecipes.Length > 0){
                var recipeIndex = Random.Range(0, BuriedRecipes.Length);
                var recipe = BuriedRecipes[recipeIndex];

                if (recipe)
                {
                    player.UnlockRecipe(recipe);

                    if (pickupEffectPrefab)
                    {
                        Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}