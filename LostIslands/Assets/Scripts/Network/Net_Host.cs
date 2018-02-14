using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_Host : MonoBehaviour {


    private void Awake()
    {
        /*if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("Net_Host is enabled obwohl kein MasterClient! Jetzt deaktiviert!");
            this.enabled = false;
            return;
        }*/
    }

    private void Start()
    {
        /*if (!PhotonNetwork.isMasterClient && GetComponentInParent<FirstPersonPlayerMovement>().gameObject.tag == "Host")
        {
            Debug.LogError("Net_Host is enabled obwohl kein MasterClient! Jetzt deaktiviert!");
            this.enabled = false;
            return;
        }

        if (!PhotonNetwork.isMasterClient) { return; }
   */

        if (PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        {
            SaveLoadManager.instance.Load(Application.dataPath + "/SaveGames/Offline/slot" + SaveLoadManager.instance.currentSlot, true);
        }
        else if ((!PhotonNetwork.offlineMode) && PhotonNetwork.isMasterClient)
        {
            //nur beim Host wird beim start alles geladen, die Clients laden wenn sie joinen dann (und bekommen WorldData vom Host)
            SaveLoadManager.instance.Load(Application.dataPath + "/SaveGames/Online/Host/slot" + SaveLoadManager.instance.currentSlot, true);
            print("now time for loading!");
            return;
        }
    }

    private void Update()
    {
        /*if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("Net_Host is enabled obwohl kein MasterClient! Jetzt deaktiviert!");
            this.enabled = false;
            return;
        }*/
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

    private void OnPlayerConnected(NetworkPlayer player)
    {
        Chat.instance.NewMessage("New player connected!", "Server", true);
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Chat.instance.NewMessage("A player disconnected!", "Server", true);
    }

}
