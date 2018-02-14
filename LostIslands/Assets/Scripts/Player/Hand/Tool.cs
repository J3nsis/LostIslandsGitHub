using System;
using UnityEngine;

[RequireComponent(typeof(ToolSwinging))]
public class Tool : MonoBehaviour {

    //wird bei drücken auf das Item in Inventory befüllt
    public ItemData.ThisItemData data;//current dura etc.
    public Item item;



    //WICHTIG: Tool name im Inspector muss einem Item insprechen!

    private void Start()
    {
        if (ItemDatabase.instance.GetItemBySlug(this.name) == null)
        {
            Debug.LogWarning("Tool Item not found in db!!");
        }

    }



    private void Update()
    {
        if (data.slot == -1)//wenn gedropped wird
        {
            HandManager.instance.DisableAll();
            data = new ItemData.ThisItemData();
            item = new Item();
            return;
        }

        if ( InventoryItems.instance.GetItembySlot(data.slot, data.inHotbar) == null 
            || InventoryItems.instance.GetItembySlot(data.slot, data.inHotbar).ID != data.ItemID)//wenn Item in Slots verschoben oder gelöscht wurde
        {
            HandManager.instance.DisableAll();
            data = new ItemData.ThisItemData();
            item = new Item();
        }
    }

}


