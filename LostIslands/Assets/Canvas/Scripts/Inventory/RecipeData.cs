using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

    public Recipe recipeData;//alles drin aus Database 
    
    void Start ()
    {
        if (recipeData == null)
        {
            Debug.LogWarning("RecipeData == null");
            return;
        }

        if (Resources.Load<Sprite>("ItemsSprites/" + recipeData.GetItem_Slug) != null)
        {
            this.GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemsSprites/" + recipeData.GetItem_Slug);
        }
        else
        {
            Debug.LogWarning("No sprite found!");
        }
        //alle sonstigen Infos in ToolTip
    }
	
    void Update()
    {
        this.GetComponent<RecipeData>().enabled = true;
    }

    public void OnClick()
    {
        if (PlayerStats.instance.ps.Level < recipeData.NeedLevel)
        {
            Chat.instance.NewWarning("You have to be Level " + recipeData.NeedLevel + " to craft this item!");
            return;
        }


        if (InventoryItems.instance.GetAmountofItem(ItemDatabase.instance.GetItemBySlug(recipeData.NeedItem1_Slug).ID) >= recipeData.NeedItem1_Amount)//wenn genug im Inventar
        {
            if (recipeData.NeedItem2_Amount != 0 || recipeData.NeedItem2_Slug != "")//wenn man zweites Item braucht
            {
                if (InventoryItems.instance.GetAmountofItem(ItemDatabase.instance.GetItemBySlug(recipeData.NeedItem2_Slug).ID) < recipeData.NeedItem2_Amount)//wenn man zweites Item hat
                {
                    Chat.instance.NewWarning("Not enough resources to craft '" + ItemDatabase.instance.GetItemBySlug(recipeData.GetItem_Slug).Name + "'!");
                    return;
                }
                InventoryItems.instance.RemoveItembySlug(recipeData.NeedItem2_Slug, recipeData.NeedItem2_Amount);//entferne 2.
            }

            InventoryItems.instance.RemoveItembySlug(recipeData.NeedItem1_Slug, recipeData.NeedItem1_Amount);//entferne 1.

            InventoryItems.instance.AddItembySlugorID(recipeData.GetItem_Slug, recipeData.GetItem_Amount);//füge neues hinzu
        }
        else
        {
            Chat.instance.NewWarning("Not enough resources to craft '" + ItemDatabase.instance.GetItemBySlug(recipeData.GetItem_Slug).Name + "'!");
            return;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) //wenn Maus drüber geht
    {
        CraftingsTooltip.instance.Activate(recipeData);
    }

    public void OnPointerExit(PointerEventData eventData) //Wenn Maus wieder weg
    {
        CraftingsTooltip.instance.Deactivate();
    }
}
