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
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }



    public void Save()
    {
        GetComponent<PhotonView>().RPC("SaveAll", PhotonTargets.All);
    }

    [PunRPC]
    void SaveAll()
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
