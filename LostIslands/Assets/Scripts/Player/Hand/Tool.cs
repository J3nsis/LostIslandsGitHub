using UnityEngine;

[RequireComponent(typeof(ToolController))]
public class Tool : MonoBehaviour {

    //public Item thisItem; //damage dura etc.                //wird bei drücken auf das Item in Inventory befüllt
    public ItemData.ThisItemData data;//current dura etc.
    public Item item;
    public int slot;
    public bool inHotbar;
   
    public bool Swinging = false; //wird von ToolManager geändert

    void Start()
    {
        item = ItemDatabase.instance.FetchItemByID(data.ItemID);//wirlich bei start?

        if (GameObject.Find(PhotonNetwork.player.NickName).GetComponentInChildren<Tool>() != this)//wenn nicht an lokalem Player
        {
            //Destroy(this);
            //animator.enabled = false;
        }
    }


    private void Update()
    {
        if (InventoryItems.instance.GetItembySlot(slot, inHotbar) == null || InventoryItems.instance.GetItembySlot(slot, inHotbar).ID != data.ItemID)//erkennt ob Slot von dem aus dieses Item benutzt wird sich veändert/leer wird
        {
            HandManager.instance.DisableAll();
        }
    }

    void OnCollisionEnter(UnityEngine.Collision other)
    {
        if (Swinging == true)//nur wenn wirklich im Schwung und nicht wenn man dagegen läuft mit Axt
        {
            if (other.gameObject.GetComponent<DamageController>() != null)//Wenn Damage Controller hat
            {
                other.gameObject.GetComponent<DamageController>().DamageMe(item.Damage);
            }
        }
        data.currentdurability -= 1;
    }

}


