using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public ItemData resultItem;
    public int resultAmount = 1;

    public List<Ingredient> ingredients;
}

