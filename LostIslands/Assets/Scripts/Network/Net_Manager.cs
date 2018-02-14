using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Net_Manager : MonoBehaviour {

    #region Instance
    public static Net_Manager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of NetworkManager found!");
            return;
        }
        instance = this;
    }
    #endregion


    public GameObject PlayerPrefab;
    public Transform Spawn;

    [SerializeField]
    GameObject localPlayer;

    [SerializeField]
    string WorldDatafromHost;

    void Start()
    {
        SpawnPlayer();
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }


    private void SpawnPlayer()
    {
        GameObject Player = PhotonNetwork.Instantiate(PlayerPrefab.name, Spawn.position, Spawn.rotation, 0);
        Player.SetActive(true);
        Player.name = "NetworkPlayer";
        Player.GetComponentInChildren<Camera>().gameObject.tag = "MainCamera";
        localPlayer = Player;       
    }

    void OnMasterClientSwitched()
    {
        SceneManager.LoadScene(0);
        PhotonNetwork.Disconnect();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
           /* if (PhotonNetwork.isMasterClient)
            {
               stream.SendNext(SaveLoadManager.instance.GetCurrentWorldData());
            }*/
        }
        else
        {
            //WorldDatafromHost = (string)stream.ReceiveNext();

        }
    }

    public GameObject GetLocalPlayer()
    {
        if (localPlayer == null)
        {
            print("[Net_Manager] local player == null");
        }
        if (localPlayer.GetComponent<PhotonView>().isMine == false && PhotonNetwork.connected)
        {
            print("[Net_Manager] local players Photon view isnt mine!");
        }
        return localPlayer;
    }

    PhotonPlayer GetPhotonPlayerByNickName(string nickname)
    {
        foreach (PhotonPlayer photonPlayer in PhotonNetwork.playerList)
        {
            if (photonPlayer.NickName == nickname)
            {
                return photonPlayer;
            }
        }
        print("Cannot find PhotonPlayer with NickName " + nickname);
        return null;
    }

    public string GetWorldDataStringFromHost()//gibt immer die aktuelle WorldData vom Host zurück! (wird bei join von neuem Spieler gebraucht)
    {
        GetComponent<PhotonView>().RPC("RPC_GetWorldDataStringFromHost", PhotonTargets.MasterClient);
        
        if (WorldDatafromHost == null)//** FEHLER HIER!!!
        {
            Debug.LogWarning("WorldDatafromHost == null:" + WorldDatafromHost);
        }
        return WorldDatafromHost;
    }



    [PunRPC]
    void RPC_GetWorldDataStringFromHost()
    {
        WorldDatafromHost = SaveLoadManager.instance.GetCurrentWorldData();
    }

    
}
