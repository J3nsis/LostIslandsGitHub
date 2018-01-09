using UnityEngine;
using UnityEngine.UI;

public class CraftingsTooltip : MonoBehaviour {

    #region Instance
    public static CraftingsTooltip instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Tooltip found!");
            return;
        }
        instance = this;
    }
    #endregion
    Recipe recipeData;

    private string data;
    public GameObject tooltip;



    void Start () 
	{        
        tooltip.SetActive(false);
	}
	
	
	void Update () 
	{
		if (tooltip.activeSelf)//wenn da
        {
            tooltip.transform.position = Input.mousePosition;
        }

        if (recipeData != null)
        {
            ConstructDataString();
        }

    }


    public void Activate(Recipe recipedata)
    {
        this.recipeData = recipedata;
        ConstructDataString();
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
        recipeData = null;
    }

    public void ConstructDataString()
    {

        data = "<b>" + recipeData.GetItem_Slug + "</b>" + "\n\nYou need: " + recipeData.NeedItem1_Amount.ToString() + "x " + recipeData.NeedItem1_Slug;

        if (recipeData.NeedItem2_Amount >= 1)
        {
            data +=  " & " + recipeData.NeedItem2_Amount + "x " + recipeData.NeedItem2_Slug;
        }
           
        tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
    }
}
