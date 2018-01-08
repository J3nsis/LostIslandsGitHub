using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            if (pp.IsMasterClient)
            {
                go.GetComponent<Text>().text = pp.NickName + " [Host]";
            }
            else
            {
                go.GetComponent<Text>().text = pp.NickName;
            }
            
            go.transform.SetParent(PlayerList);
        }
        Players.text = PhotonNetwork.room.PlayerCount + "/" + PhotonNetwork.room.MaxPlayers + " Player";

        if (PhotonNetwork.isMasterClient)
        {
            JoinGameButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            JoinGameButton.GetComponent<Button>().interactable = false;
            JoinGameButton.GetComponentInChildren<Text>().text += "Only host can start game!";

        }
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        InitializeRoomView();
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        InitializeRoomView();
    }

    public void JoinGame()
    {
        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("JoinGameRPC", PhotonTargets.All);

    }

    [PunRPC]
    void JoinGameRPC()
    {
        SaveLoadManager.instance.OpenGameScene();
	}
}
