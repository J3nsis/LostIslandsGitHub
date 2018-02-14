using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(PhotonView))]
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
    public List<GameObject> Tools = new List<GameObject>();

    public GameObject currentToolObj;
    public String currentToolSlug;

    [SerializeField]
    HumanHandController humanHandController;

    string dict;


    private void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<Tool>())
            {
                Tools.Add(child.gameObject);
            }
        }
        DisableAll();
    }

    public void UseTool(string slug, int slot, Item item, ItemData itemData)
    {
        DisableAll();
        currentToolSlug = slug;

        foreach (GameObject tool in Tools)
        {
            if (tool.name == slug)
            {
                currentToolObj = tool;
                tool.SetActive(true);
                GetComponent<PhotonView>().RPC("RPC_EnableDisable", PhotonTargets.Others, true, tool.transform.GetSiblingIndex());
                tool.GetComponent<Tool>().item = item;
                tool.GetComponent<Tool>().data = itemData.data;

                humanHandController.m_RightHandObj = tool.transform.Find("HandAnchor");
            }
        }   
    }

    public void DisableAll()
    {
        foreach (GameObject tool in Tools)
        {
            tool.SetActive(false);
            GetComponent<PhotonView>().RPC("RPC_EnableDisable", PhotonTargets.Others, false, tool.transform.GetSiblingIndex());
        }

        humanHandController.m_RightHandObj = null;
        currentToolObj = null;
        currentToolSlug = "";
    }
   

    [PunRPC]
    void RPC_EnableDisable(bool enable, int childIndex)
    {
        transform.GetChild(childIndex).gameObject.SetActive(enable);
    }
   

}
