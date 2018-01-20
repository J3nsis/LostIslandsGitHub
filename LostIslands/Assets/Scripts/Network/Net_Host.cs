using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_Host : MonoBehaviour {

    private PhotonView photonView;


    private void Start()
    {
        LoadAll();
    }

    public void SaveAll()
    {
        if (PhotonNetwork.connected)
        {
            GetComponent<PhotonView>().RPC("SaveAllRPC", PhotonTargets.All);
        }      
    }

    [PunRPC]
    void SaveAllRPC()
    {
        SaveLoadManager.instance.Save(PhotonNetwork.offlineMode);
    }

    public void LoadAll()
    {
        GetComponent<PhotonView>().RPC("LoadAllRPC", PhotonTargets.All);
    }

    [PunRPC]
    void LoadAllRPC()
    {
        SaveLoadManager.instance.Load(SaveLoadManager.instance.currentSlot, PhotonNetwork.offlineMode);
    }

    //### Player Identity


    
}
