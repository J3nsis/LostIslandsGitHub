using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof (PhotonView))]
public class ViewRoom : MonoBehaviour {

    #region Instance
    public static ViewRoom instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of ViewRoom found!");
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField]
    GameObject PlayerNamePrefab, JoinGameButton;

    [SerializeField]
    Text RoomName, Players;

    [SerializeField]
    Transform PlayerList;


    void Start()
    {

    }

    public void InitializeRoomView()
    {
        for (int i = 0; i < PlayerList.transform.childCount; i++)
        {
            Destroy(PlayerList.transform.GetChild(i).gameObject);
        }

        RoomName.text = "Roomname: " + PhotonNetwork.room.Name;

        foreach (PhotonPlayer pp in PhotonNetwork.playerList)
        {
            GameObject go = Instantiate(PlayerNamePrefab);
            if (PhotonNetwork.isMasterClient)//wenn selber Host
            {
                go.GetComponentInChildren<Button>().interactable = true;
                go.GetComponentInChildren<Button>().onClick.AddListener(delegate () { OnKickPlayer(pp.NickName); });
            }
            else go.GetComponentInChildren<Button>().interactable = false;

            if (pp.IsMasterClient)
            {
                go.GetComponentInChildren<Text>().text = pp.NickName + " [Host]";
                go.GetComponentInChildren<Button>().gameObject.SetActive(false);
            }
            else
            {
                go.GetComponentInChildren<Text>().text = pp.NickName;               
            }

            

            go.transform.SetParent(PlayerList);
        }
        Players.text = PhotonNetwork.room.PlayerCount + "/" + PhotonNetwork.room.MaxPlayers + " Players";

        if (PhotonNetwork.isMasterClient)
        {
            JoinGameButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            JoinGameButton.GetComponent<Button>().interactable = false;
            JoinGameButton.GetComponentInChildren<Text>().text = "Start game (Only host can start game!)";
        }
    }

    void OnMasterClientSwitched()
    {
        PhotonNetwork.Disconnect();
        MainMenuManager.instance.ToMainSelection();
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        InitializeRoomView();
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        InitializeRoomView();
    }

    public void OnKickPlayer(string NickName)
    {
        /*foreach (PhotonPlayer pp in PhotonNetwork.playerList)
        {
            if (pp.NickName == NickName && pp.NickName != PhotonNetwork.player.NickName)
            {
                PhotonNetwork.CloseConnection(pp);
            }
        }*/ //noch viele Fehler!
        print("Work in progress!");
    }

    public void StartGameforAll()
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("StartGameforAllRPC", PhotonTargets.All);
    }

    [PunRPC]
    void StartGameforAllRPC()
    {
        SaveLoadManager.instance.OpenGameScene();
	}
}
