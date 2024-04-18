using System.Collections;
using UnityEngine;

public class Buried : MonoBehaviour
{
    public GameObject pickupEffectPrefab; 
    public Sprite recipeSprite;
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
                player.UnlockRecipe(recipe);

                if (recipeSprite)
                {
                    GameObject spriteObject = new GameObject("RecipeSprite");
                    spriteObject.transform.position = transform.position;
                    SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = recipeSprite;
                }

                if (pickupEffectPrefab)
                {
                    Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}
