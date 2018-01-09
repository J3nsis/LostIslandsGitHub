using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecipeDatabase : MonoBehaviour
{

    private List<Recipe> database = new List<Recipe>();
    public JsonData RecipeData;

    void Awake()
    {
        RecipeData = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath + "/Recipes.json")); //json Datei auslesen
        ConstructItemDatabase();
    }

    void ConstructItemDatabase()
    {
        for (int i = 0; i < RecipeData.Count; i++)
        {
            database.Add(new Recipe((int)RecipeData[i]["ID"], (int)RecipeData[i]["NeedLevel"], RecipeData[i]["GetItem_Slug"].ToString(), (int)RecipeData[i]["GetItem_Amount"], RecipeData[i]["NeedItem1_Slug"].ToString(), (int)RecipeData[i]["NeedItem1_Amount"], RecipeData[i]["NeedItem2_Slug"].ToString(), (int)RecipeData[i]["NeedItem2_Amount"]));
        }
    }

    public Recipe GetRecipeByID(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].ID == id)
            {
                return database[i];
            }
        }
        return null;
    }
}

[Serializable]
public class Recipe
{
    public int ID;
    public int NeedLevel;

    public string GetItem_Slug;
    public int GetItem_Amount;
    public string NeedItem1_Slug;
    public int NeedItem1_Amount;
    public string NeedItem2_Slug;
    public int NeedItem2_Amount;


    public Recipe(int ID, int NeedLevel, string GetItem_Slug, int GetItem_Amount, string NeedItem1_Slug, int NeedItem1_Amount, string NeedItem2_Slug, int NeedItem2_Amount)
    { 
        this.ID = ID;
        this.NeedLevel = NeedLevel;

        this.GetItem_Slug = GetItem_Slug;
        this.GetItem_Amount = GetItem_Amount;
        this.NeedItem1_Slug = NeedItem1_Slug;
        this.NeedItem1_Amount = NeedItem1_Amount;
        this.NeedItem2_Slug = NeedItem2_Slug;
        this.NeedItem2_Amount = NeedItem2_Amount;

    }

    public Recipe()
    {
        this.ID = -1; //leeres Item
    }
}
