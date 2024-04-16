using System.Collections;
using UnityEngine;

public class Buried : MonoBehaviour
{
    public GameObject pickupEffectPrefab; // Prefab for the pick-up effect
    public Sprite recipeSprite; // Sprite of the recipe item
    public CraftingRecipe[] BuriedRecipes;
    public float timeToDig = 1f;
    [HideInInspector] public bool playerTouching = false;

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
        if (playerTouching)
        {
            var r = Random.Range(0, BuriedRecipes.Length);
            var recipe = BuriedRecipes[r];
            if (recipe)
            {
                // Unlock the recipe for the player
                player.UnlockRecipe(recipe);

                // Display the recipe sprite
                if (recipeSprite)
                {
                    // Instantiate a GameObject to display the sprite
                    GameObject spriteObject = new GameObject("RecipeSprite");
                    spriteObject.transform.position = transform.position;
                    SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = recipeSprite;
                }

                // Show the pick-up effect
                if (pickupEffectPrefab)
                {
                    Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
                }
            }
        }
    }
}
