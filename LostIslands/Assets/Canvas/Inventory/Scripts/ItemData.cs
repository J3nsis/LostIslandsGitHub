using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler{


    //### Standart Data/Info über Item von json
    public Item item;

    [Serializable]
    public class ThisItemData //Custom Data
    {
        public int ItemID;

        public int amount;
        public int slot;
        public bool inHotbar;
        public int currentdurability;       
    }
    public ThisItemData data;//wird über InventoryItems und dann über SaveLoadManager gespeichert/geladen

    //### sonstiges
    public bool newAdded;

    private InventoryItems inv;
    private ItemsTooltip tooltip;
    private Vector2 offset;
    
    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<InventoryItems>();
        tooltip = inv.GetComponent<ItemsTooltip>();

        if (newAdded)//newaddd wird auf true gesetzt (von Inventory) wenn man manuell Item Hinzufügt
        {
            if (item.Type == "tool")//Haltbarkeit zu standarthaltbarkeit setzen
            {
                data.currentdurability = item.Durability;
                newAdded = false;
            }
        }
    }

    void Update()
    {
        if (item.Stackable)
        {
            this.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
        }
    }

    void LateUpdate()
    {
        if ((item.Stackable && data.amount <= 0) || (item.Type == "tool" && data.currentdurability == 0))
        {
            DeleteSlot();
        }
    }

    void DeleteSlot()
    {
        print("Deleted slot:" + data.slot);       
        InventoryItems.instance.items[data.slot] = new Item();
        tooltip.Deactivate();

        Destroy(this.gameObject);
    }

    public void OnClick()
    {

        if (item.Type == "tool" || item.Type == "hammer")//nur zum testen
        {
            HandManager.instance.UseTool(item.Slug, data.slot, item, this);
        }   
    }

    public void DropItem()
    {
        //drop Item in ItemPackage on Ground
        //initialise Itempackage
        //ItemPackage.Item = this.Item
        //etc.
        print("drop Item");
    }


    //################################################### Drag n Drop ##################################################
    public void OnBeginDrag(PointerEventData eventData)
    {       
        if (item != null)//nur wenn wirklich Item da
        {
            offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);//offset = MausPosition - Slot Position, damit wenn man itm unten rechts "anfasst" nicht die maus sich in der mitte vom object locked
            this.transform.SetParent(this.transform.parent.parent.parent);//Item als Child von "allen Slots Object" machen damit nicht beim draggen dahinter ist
            this.transform.position = eventData.position;
            GetComponent<CanvasGroup>().blocksRaycasts = false;//damit in Slot snappt, sonst gehts net wegen Maus oder so
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)//nur wenn wirklich Item da
        {
            this.transform.position = eventData.position - offset;;
            if (data.inHotbar)
            {
                //print("slot cleared, h");
                inv.HotbarItems[data.slot] = new Item(); //slot wo item herkommt clearen damit man es auch wieder darauf plazieren kann
            }
            else
            {
                //print("slot cleared, n");
                inv.items[data.slot] = new Item(); //slot wo item herkommt clearen damit man es auch wieder darauf plazieren kann
            }
        }       
    }

    public void OnEndDrag(PointerEventData eventData)//wenn man loslässt??
    {
        //print("onenddrag");
        if (!data.inHotbar)
        {
            this.transform.SetParent(inv.slots[data.slot].transform);//zurück in Slot
            this.transform.position = inv.slots[data.slot].transform.position;
            inv.items[data.slot] = item;
        }
        else
        {
            this.transform.SetParent(inv.HotbarSlots[data.slot].transform);//zurück in Slot
            this.transform.position = inv.HotbarSlots[data.slot].transform.position;
            inv.HotbarItems[data.slot] = item;
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData) //wenn Maus drüber geht
    {
        tooltip.Activate(item, this);
    }

    public void OnPointerExit(PointerEventData eventData) //Wenn Maus wieder weg
    {
        tooltip.Deactivate();
    }

}
