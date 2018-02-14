using UnityEngine;
using LitJson;
using System.Collections.Generic;
using System.IO;
using System;

public class ItemDatabase : MonoBehaviour {

    #region Instance
    public static ItemDatabase instance;
    #endregion

    private List<Item> database = new List<Item>();
    private JsonData itemData;
	
	void Awake () 
	{
        instance = this;
    
        itemData = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath+ "/Items.json").ToString()); //json Datei auslesen
        ConstructItemDatabase();
	}

    void ConstructItemDatabase()
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            database.Add(new Item((int)itemData[i]["id"], itemData[i]["name"].ToString(), itemData[i]["slug"].ToString(), (bool)itemData[i]["stackable"], itemData[i]["description"].ToString(), itemData[i]["type"].ToString(), (int)itemData[i]["durability"], (int)itemData[i]["damage"]));
        }
    }

    public Item FetchItemByID(int id)//gibt Item per ID zurück
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

    public Item GetItemBySlug(string slug)//gibt Item per ID zurück
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].Slug == slug)
            {
                return database[i];
            }
        }
        return null;
    }

}

[Serializable]
public class Item
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public Sprite Sprite { get; set; }
    public bool Stackable { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public int Durability { get; set;}
    public int Damage { get; set; }


    public Item(int id, string name, string slug, bool stackable, string description, string type, int durability, int damage)
    {
        this.ID = id;
        this.Name = name;
        this.Slug = slug;
        this.Sprite = Resources.Load<Sprite>("ItemsSprites/" + slug);
        this.Stackable = stackable;
        this.Description = description;
        this.Type = type;
        this.Durability = durability;
        this.Damage = damage;
    }

    public Item()
    {
        this.ID = -1; //empty Item (erzeugt man mit: new Item();
    }
}
