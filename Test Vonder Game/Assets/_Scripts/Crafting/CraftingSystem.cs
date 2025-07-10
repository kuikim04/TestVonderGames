using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public List<Recipe> recipes;

    private bool Craft(int recipeIndex)
    {
        if (recipeIndex < 0 || recipeIndex >= recipes.Count)
        {
            Debug.Log("Invalid recipe index");
            return false;
        }

        Recipe recipe = recipes[recipeIndex];

        if (!HasIngredients(recipe))
        {
            Debug.Log("Not enough ingredients");
            return false;
        }

        foreach (var ingredient in recipe.ingredients)
        {
            Inventory.Instance.RemoveItem(ingredient.item, ingredient.amount);
        }

        bool added = Inventory.Instance.AddItem(recipe.resultItem, recipe.resultAmount);

        if (!added)
        {
            Debug.Log("Inventory full");
            return false;
        }

        Debug.Log($"Crafted: {recipe.resultItem.itemName}");
        return true;
    }

    private bool HasIngredients(Recipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            int totalAmount = Inventory.Instance.GetTotalQuantity(ingredient.item);
            if (totalAmount < ingredient.amount)
                return false;
        }
        return true;
    }

    public void CraftByIndex(int recipeIndex)
    {
        Craft(recipeIndex);
    }

}
