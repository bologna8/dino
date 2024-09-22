using UnityEngine;
using UnityEngine.UI;

public class CraftingUIManager : MonoBehaviour
{
    public GameObject ingredientSlotPrefab; 
    public Transform ingredientContainer; 

    public void DisplayRecipeIngredients(CraftingRecipe recipe)
    {
        ClearIngredientSlots(); 

        foreach (var ingredient in recipe.ingredients)
        {
            GameObject slot = Instantiate(ingredientSlotPrefab, ingredientContainer);
            Image icon = slot.GetComponent<Image>(); 

            
            if (Inventory.instance.ContainsItem(ingredient.item, ingredient.amount))
            {
                icon.sprite = ingredient.item.icon; 
                icon.color = Color.white; 
            }
            else
            {
                icon.sprite = ingredient.item.icon; 
                icon.color = Color.gray; 
            }
        }
    }

    private void ClearIngredientSlots()
    {
        foreach (Transform child in ingredientContainer)
        {
            Destroy(child.gameObject); 
        }
    }
}
