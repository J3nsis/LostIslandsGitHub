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

        this.GetComponent<Image>().sprite = Resources.Load<Sprite>("ItemsSprites/" + recipeData.GetItem_Slug);

        //alle sonstigen Infos in ToolTip
    }
	
    void Update()
    {
        this.GetComponent<RecipeData>().enabled = true;
    }

    public void OnClick()
    {
        if (InventoryItems.instance.GetAmountofItem(ItemDatabase.instance.GetItemBySlug(recipeData.NeedItem1_Slug).ID) >= recipeData.NeedItem1_Amount)//wenn genug im Inventar
        {
            if (recipeData.NeedItem2_Amount != 0)//wenn man zweites Item braucht
            {
                if (InventoryItems.instance.GetAmountofItem(ItemDatabase.instance.GetItemBySlug(recipeData.NeedItem2_Slug).ID) >= recipeData.NeedItem2_Amount)//wenn man zweites Item hat
                {

                }
                else
                {
                    Chat.instance.NewWarning("Not enough resources to craft " + recipeData.GetItem_Slug + "!");
                    return;
                }
                InventoryItems.instance.RemoveItembySlug(recipeData.NeedItem2_Slug, recipeData.NeedItem2_Amount);
            }

            InventoryItems.instance.RemoveItembySlug(recipeData.NeedItem1_Slug, recipeData.NeedItem1_Amount);

            InventoryItems.instance.AddItembySlug(recipeData.GetItem_Slug, recipeData.GetItem_Amount);
        }
        else
        {
            Chat.instance.NewWarning("Not enough resources to craft " + recipeData.GetItem_Slug + "!");
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
