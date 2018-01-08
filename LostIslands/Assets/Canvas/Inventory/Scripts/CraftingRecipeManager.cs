using UnityEngine;

public class CraftingRecipeManager : MonoBehaviour {

    public GameObject RecipesParent;
    public GameObject RecipeSlotPrefab;

    RecipeDatabase rb;

	void Start () 
	{
        rb = this.GetComponent<RecipeDatabase>();
        for (int i = 0; i < rb.RecipeData.Count; i++)//jedes Rezept durchgehen
        {
            GameObject newRecipeSlot = Instantiate(RecipeSlotPrefab, RecipesParent.transform);
            newRecipeSlot.GetComponent<RecipeData>().recipeData = rb.GetRecipeByID(i);
        }
	}
	
	
	void Update () 
	{
		
	}
}


