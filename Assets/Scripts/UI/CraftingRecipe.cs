using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "CraftingRecipe/baseRecipe")]
public class CraftingRecipe : Item
{
    public Item result;
    public Ingredient[] ingredients;

//If inventory contains the recipe items, then you can craft 
    private bool CanCraft()
    {
        foreach (Ingredient ingredient in ingredients)
        {
            bool containsCurrentIngredient = Inventory.instance.ContainsItem(ingredient.item, ingredient.amount);

            if (!containsCurrentIngredient)
            {
                return false;
            }
        }

        return true;
    }


    private void RemoveIngredientsFromInventory()
    {
        foreach (Ingredient ingredient in ingredients)
        {
            Inventory.instance.RemoveItems(ingredient.item, ingredient.amount);
        }
    }

//If the item is used to craft, remove it from the inventory
    public override void Use()
    {
        if (CanCraft())
        {
            RemoveIngredientsFromInventory();

            // Add crafted item to the inventory
            Inventory.instance.AddItem(result);
            Debug.Log("You just crafted a: " + result.name);
        }
        else
        {
            Debug.Log("You don't have enough ingredients to craft: " + result.name);
        }
    }

    public override string GetItemDescription()
    {
        string itemIngredients = "";

        foreach (Ingredient ingredient in ingredients)
        {
            itemIngredients += "- " + ingredient.amount + " " + ingredient.item.name + "\n";
        }

        return itemIngredients;
    }

    [System.Serializable]
    public class Ingredient
    {
        public Item item;
        public int amount;
    }
}
