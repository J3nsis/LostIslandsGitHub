using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPackage : MonoBehaviour
{

    //neu mit Item (aus Json) machen!!! und ItemData etc.

    public GameObject InfoText;
    public string ItemName; //rest wird von near und Inventory Slot gemacht
    public int ItemValue;

    

    private void FixedUpdate()
    {
        InfoText.transform.Rotate(Vector3.up * Time.deltaTime * 10);
        InfoText.transform.eulerAngles = new Vector3(0,InfoText.transform.rotation.y,0);
        InfoText.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);

        InfoText.GetComponent <TextMesh>().text = ItemName + " : " + ItemValue;
    }



}
    

