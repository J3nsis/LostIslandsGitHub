using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomsView : MonoBehaviour {

    #region Instance
    public static RoomsView instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of RoomsView found!");
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField]
    GameObject RoomPrefab;

    [SerializeField]
    Transform RoomParent;

    void OnEnable()
    {
        PhotonNetwork.ConnectUsingSettings("GameVersion");
    }

    public void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(new TypedLobby("MyLobby", LobbyType.SqlLobby));
        PhotonNetwork.playerName = PlayerPrefs.GetString("Username");
    }

    void OnJoinedLobby()
    {
        //print("connected to lobby");
    }

    public void JoinRoom(string roomName)//per Liste und Button Room joinen
    {        
        PhotonNetwork.JoinRoom(roomName);
    }


    public void OnJoinedRoom()
    {
        MainMenuManager.instance.ShowRoomView();
        ViewRoom.instance.InitializeRoomView();
    }

    public void ShowAllRooms()
    {
        for (int i = 0; i < RoomParent.transform.childCount; i++)
        {
            Destroy(RoomParent.transform.GetChild(i).gameObject);
        }

        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            Debug.LogWarning("No rooms in this Lobby!");
        }
        foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
        {
            GameObject roomPrefab = Instantiate(RoomPrefab);
            roomPrefab.transform.SetParent(RoomParent);
            roomPrefab.GetComponentInChildren<Button>().onClick.AddListener(delegate () { JoinRoom(roomInfo.Name); });
            roomPrefab.GetComponent<Text>().text = roomInfo.Name + " | " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers + " Player";
        }
    }


    void OnReceivedRoomListUpdate()
    {
        ShowAllRooms();
    }
}
