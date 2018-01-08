using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class HandManager : MonoBehaviour {

    #region Instance
    public static HandManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            print("More than one instance of HandManager found!");
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField]
    public GameObject Stone_Axe, Stone_Pick, Iron_Axe, Iron_Pick, Hammer;

    public GameObject currentToolObj;
    public String currentToolSlug;

    [SerializeField]
    HumanHandController humanHandController;


    private void Start()
    {
        DisableAll();
    }

    private void UseTool2(GameObject go, int slot, Item item, ItemData itemData)
    {
        go.SetActive(true);
        go.GetComponent<Tool>().item = item;
        go.GetComponent<Tool>().data = itemData.data;
        go.GetComponent<Tool>().slot = itemData.data.slot;

        humanHandController.RightHandObj = currentToolObj.transform.GetChild(0).transform;
        
    }


    public void UseTool(string slug, int slot, Item item, ItemData itemData)
    {
        DisableAll();
        GameObject go = null;
        currentToolSlug = slug;
        switch (slug)
        {
            case "Stone_Axe":
                go = Stone_Axe;
                break;
            case "Stone_Pick":
                go = Stone_Pick;
                break;
            case "Iron_Axe":
                go = Iron_Axe;
                break;
            case "Iron_Pick":
                go = Iron_Pick;
                break;

            case "Hammer":
                Hammer.SetActive(true);
                return;
        }
        if (go == null)
        {
            Debug.LogWarning("Cant find tool!");
            return;
        }
        currentToolObj = go;
        UseTool2(go, slot, item, itemData);      
    }

    public void DisableAll()
    {
        Hammer.SetActive(false);
        Stone_Axe.SetActive(false);
        Stone_Pick.SetActive(false);
        Iron_Axe.SetActive(false);
        Iron_Pick.SetActive(false);

        humanHandController.RightHandObj = null;
        currentToolObj = null;
        currentToolSlug = "";
    }

}
