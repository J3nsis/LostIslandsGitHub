using UnityEngine;
using UnityEngine.UI;

public class ItemsTooltip : MonoBehaviour {

    #region Instance
    public static ItemsTooltip instance;

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

    private Item item;
    private string data;
    public GameObject tooltip;

    ItemData Itemdata; //Damit Infos wie currentDurability von Objekt holen kann

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

        if (item != null)
        {
            ConstructDataString();
        }

    }


    public void Activate(Item item, ItemData Itemdata)
    {
        this.item = item;
        this.Itemdata = Itemdata;
        ConstructDataString();
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
        item = null;
        Itemdata = null;
    }

    public void ConstructDataString()
    {
        data = "<b>" + item.Name + "</b>\n" + "\n" + item.Description;

        if(item.Type == "tool")
        {
            data += "\n" + "Durability: " + Itemdata.data.currentdurability + "/" + item.Durability;
        }
           
        tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
    }
}
