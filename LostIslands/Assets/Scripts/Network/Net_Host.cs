using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net_Host : MonoBehaviour {

    private PhotonView photonView;

    string WorldDataString;


    private void Start()//nur MasterClient hat dieses Script hier aktiviert!
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("Net_Host is enabled obwohl kein MasterClient! Jetzt deaktiviert!");
            this.enabled = false;
            return;
        }
        if (PhotonNetwork.offlineMode)
        {
            SaveLoadManager.instance.Load(Application.dataPath + "/SaveGames/Offline/slot" + SaveLoadManager.instance.currentSlot, true);
        }
        else //online
        {
            Chat.instance.NewInfo("Loading for " + PhotonNetwork.room.PlayerCount + "Player");
            GetComponent<PhotonView>().RPC("LoadAllRPC", PhotonTargets.All); //später das wenn neuer Spieler connected auch er den aktuellen stand der map läd etc.
            WorldDataString = SaveLoadManager.instance.GetWorldDataStringFromLocal();
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
            WorldDataString = SaveLoadManager.instance.GetWorldDataStringFromLocal();
        }
        else//only Client
        {
            SaveLoadManager.instance.Save(Application.dataPath + "/SaveGames/Online/Join/" + PhotonNetwork.masterClient.NickName + "/slot" + SaveLoadManager.instance.currentSlot, false);
        }
       
    }



    [PunRPC]
    void LoadAllRPC()
    {       
        if (PhotonNetwork.isMasterClient)
        {
            SaveLoadManager.instance.Load(Application.dataPath + "/SaveGames/Online/Host/slot" + SaveLoadManager.instance.currentSlot, true);
        }
        else//only Client
        {
            if (WorldDataString == "")
            {
                Debug.LogWarning("WorldDataString == null, cannot load World on this client!");
            }
            SaveLoadManager.instance.Load(Application.dataPath + "/SaveGames/Online/Join/" + PhotonNetwork.masterClient.NickName + "/slot" + SaveLoadManager.instance.currentSlot, false, WorldDataString);
        }
        
    }

    //### Player Identity


    
}
