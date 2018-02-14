using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class ItemPackage : MonoBehaviour
{

    public ItemData.ThisItemData itemData;
    public Item item;

    [SerializeField]
    GameObject InfoText;

    public bool looted = false;//damit nicht mehrmals gelooted werden kann (wird in near geregelt)

    private void Start()
    {       
        itemData.inHotbar = false;
        itemData.slot = -1;
        
        //sonst InfoText verkehrtrum!
        InfoText.transform.localScale = new Vector3(-InfoText.transform.localScale.x, InfoText.transform.localScale.y, InfoText.transform.localScale.z);
    }


    private void Update()//Aufheben etc. wird in Near geregelt!
    {
        if (Camera.main != null)
        {
            InfoText.transform.LookAt(Camera.main.transform.position);
        }
        
        
        if (itemData.ItemID != -1)//wenn ItemData schon syncronisiert wurde
        {
            item = ItemDatabase.instance.FetchItemByID(itemData.ItemID);
            InfoText.GetComponent<TextMesh>().text = itemData.amount + "x " + item.Name;
            InfoText.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }
    }

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(itemData.amount);
            stream.SendNext(itemData.currentdurability);
            stream.SendNext(itemData.ItemID);//braucht nur ID, denn rest kann er sich ja selbst aus db holen
            stream.SendNext(looted);
        }
        else
        {
            itemData.amount = (int)stream.ReceiveNext();
            itemData.currentdurability = (int)stream.ReceiveNext();
            itemData.ItemID = (int)stream.ReceiveNext();
            looted = (bool)stream.ReceiveNext();
        }

    }
}
    

