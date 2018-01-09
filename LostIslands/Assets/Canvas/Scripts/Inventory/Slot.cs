using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {

    private int slot;//oder SlotNr
    public bool HotbarSlot;
    public InventoryItems inv;
    public bool full;

    void Start()
    {
        slot = Int32.Parse(this.gameObject.name.Replace("Slot", string.Empty));// slot soll Name nur ohne "Slot" sein, also z.B.: "Slot1" -> 1
        inv = GameObject.Find("Inventory").GetComponent<InventoryItems>();
    }

    public void Update()
    {
        if (this.transform.childCount >= 1)//um fehler zu vermeiden
        {
            //Item item = this.transform.GetChild(0).GetComponent<ItemData>().item;
            //inv.items[slot].ID = item.ID;
        }

        if (HotbarSlot)
        {
            if (inv.HotbarItems[slot].ID == -1)
            {
                full = false;
            }
            else
            {
                full = true;
            }
        }
        else
        {
            if (inv.items[slot].ID == -1)
            {
                full = false;
            }
            else
            {
                full = true;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)//wenn man Maus auf Slot loslässt
    { 
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        //print(inv.items[slot].ID);
        if ((HotbarSlot == false && inv.items[slot].ID == -1) || (HotbarSlot == true && inv.HotbarItems[slot].ID == -1))///wenn dieser Slot hier leer (also der wo man loslässt)
        {
            if (!HotbarSlot)
            {               
                inv.items[slot] = droppedItem.item;
                droppedItem.data.slot = slot;
                droppedItem.data.inHotbar = false;  
            }
            else
            {
                if (droppedItem.item.Type == "tool" || droppedItem.item.Type == "Structure")//nur diese Typen können in Hotbar
                {
                    inv.HotbarItems[slot] = droppedItem.item;
                    droppedItem.data.slot = slot;
                    droppedItem.data.inHotbar = true;
                }
                else
                {
                    inv.items[droppedItem.data.slot] = droppedItem.item;
                }
            }
            
        }
        else if (droppedItem.data.slot != slot)//wenn Slot (der hier) voll wo man los lässt und das gedroppte Item nicht auf diesen hier gehört: Items tauschen
        {
            if (!HotbarSlot && !droppedItem.data.inHotbar)//Swap in normalem Inventar
            {
                Transform item = this.transform.GetChild(0);
                item.GetComponent<ItemData>().data.slot = droppedItem.data.slot;
                item.transform.SetParent(inv.slots[droppedItem.data.slot].transform);
                item.transform.position = inv.slots[droppedItem.data.slot].transform.position;

                inv.items[droppedItem.data.slot] = item.GetComponent<ItemData>().item;
                inv.items[slot] = droppedItem.item;
                droppedItem.data.slot = slot;

            }
            else if (HotbarSlot && droppedItem.data.inHotbar)//swap in Hotbar
            {
                Transform item = this.transform.GetChild(0);
                item.GetComponent<ItemData>().data.slot = droppedItem.data.slot;
                item.transform.SetParent(inv.HotbarSlots[droppedItem.data.slot].transform);
                item.transform.position = inv.HotbarSlots[droppedItem.data.slot].transform.position;

                inv.HotbarItems[droppedItem.data.slot] = item.GetComponent<ItemData>().item;
                inv.HotbarItems[slot] = droppedItem.item;
                droppedItem.data.slot = slot;

            }
        


            
            
        }
    }

}
