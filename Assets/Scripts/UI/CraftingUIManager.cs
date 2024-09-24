using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CraftingUIManager : MonoBehaviour
{
    public Transform recipeListParent;       
    public Transform ingredientContainer;   
    public GameObject ingredientSlotPrefab;  

    private List<CraftingRecipe> collectedRecipes = new List<CraftingRecipe>();

    public void AddRecipeToUI(CraftingRecipe recipe)
    {
        if (!collectedRecipes.Contains(recipe))
        {
            collectedRecipes.Add(recipe); 

            GameObject recipeGO = new GameObject(recipe.result.name);  

            RectTransform rt = recipeGO.AddComponent<RectTransform>();
            recipeGO.transform.SetParent(recipeListParent, false);  

            Image recipeIcon = recipeGO.AddComponent<Image>();

            recipeIcon.sprite = recipe.result.icon;
            recipeIcon.color = Color.white;  

            rt.sizeDelta = new Vector2(100, 100);  
            rt.localScale = Vector3.one;  
        }
    }

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
