using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_Host : MonoBehaviour {


    private void Awake()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            //Debug.LogError("Net_Host is enabled obwohl kein MasterClient! Jetzt deaktiviert!");
            this.enabled = false;
            return;
        }
    }

    private void Start()
    {
        if (!PhotonNetwork.isMasterClient) { return; }

        if (PhotonNetwork.offlineMode)
        {
            SaveLoadManager.instance.Load(Application.dataPath + "/SaveGames/Offline/slot" + SaveLoadManager.instance.currentSlot, true);
        }
        else
        {
            //nur beim Host wird beim start alles geladen, die Clients laden wenn sie joinen dann (und bekommen WorldData vom Host)
            SaveLoadManager.instance.Load(Application.dataPath + "/SaveGames/Online/Host/slot" + SaveLoadManager.instance.currentSlot, true);  
            return;
        }
    }


    public void SaveAll()//wird von Esc Menü Save aufgerufen
    {
        if (PhotonNetwork.offlineMode)
        {
            SaveLoadManager.instance.Save(Application.dataPath + "/SaveGames/Offline/slot" + SaveLoadManager.instance.currentSlot, true);
            return;
        }

        if (PhotonNetwork.connected)
        {
            GetComponent<PhotonView>().RPC("SaveAllRPC", PhotonTargets.All);
        }      
    }

    [PunRPC]
    void SaveAllRPC()
    {
        if (PhotonNetwork.isMasterClient)
        {
            SaveLoadManager.instance.Save(Application.dataPath + "/SaveGames/Online/Host/slot" + SaveLoadManager.instance.currentSlot, true);
        }
        else//only Client
        {
            SaveLoadManager.instance.Save(Application.dataPath + "/SaveGames/Online/Join/" + PhotonNetwork.masterClient.NickName + "/slot" + SaveLoadManager.instance.currentSlot, false);
        }
       
    }
 
}
