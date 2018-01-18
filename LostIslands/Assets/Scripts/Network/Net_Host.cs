using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_Host : MonoBehaviour {

    private void Start()
    {
        Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveAll();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
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

    public void Load()
    {
        GetComponent<PhotonView>().RPC("LoadAll", PhotonTargets.All);
    }

    [PunRPC]
    void LoadAll()
    {
        SaveLoadManager.instance.Load(SaveLoadManager.instance.currentSlot, PhotonNetwork.offlineMode);
    }
    
}
