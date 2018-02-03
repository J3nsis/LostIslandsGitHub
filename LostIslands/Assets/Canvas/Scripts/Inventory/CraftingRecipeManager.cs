using UnityEngine;
using UnityEngine.UI;

public class CraftingRecipeManager : MonoBehaviour {


    [SerializeField]
    GameObject SlotPrefab, LevelColumPrefab, RecipeinSlotPrefab, AllParent;

    RecipeDatabase rdb;

	void Start () 
	{
        rdb = this.GetComponent<RecipeDatabase>();

        foreach(Recipe recipe in rdb.database)
        {
            if (AllParent.transform.Find("Level" + recipe.NeedLevel) == null)
            {
                GameObject levelColumPrefab = Instantiate(LevelColumPrefab);
                levelColumPrefab.transform.SetParent(AllParent.transform);
                levelColumPrefab.name = "Level" + recipe.NeedLevel;
                levelColumPrefab.transform.Find("LevelText").GetComponent<Text>().text = "Level " + recipe.NeedLevel + ":"; //Level Text über Recipes setzen
            }
            GameObject levelColum = AllParent.transform.Find("Level" + recipe.NeedLevel).gameObject;

            GameObject newSlot = Instantiate(SlotPrefab);
            newSlot.transform.SetParent(levelColum.transform.Find("RecipesParent").transform);


            GameObject newRecipe = Instantiate(RecipeinSlotPrefab);
            newRecipe.transform.SetParent(newSlot.transform);
            newRecipe.GetComponent<RecipeData>().recipeData = recipe;
        }
	}
	
	
	void Update () 
	{
		
	}
}


