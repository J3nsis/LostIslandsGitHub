using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomsList: MonoBehaviour
{

    #region Instance
    public static RoomsList instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of RoomsList found!");
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField]
    GameObject RoomPrefab;

    [SerializeField]
    Text NoRooms;

    [SerializeField]
    Transform RoomParent;

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
            if (RoomParent.transform.GetChild(i).gameObject != null)
            {
                Destroy(RoomParent.transform.GetChild(i).gameObject);
            }       
        }

        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            Debug.LogWarning("No rooms in this lobby!");
            NoRooms.text = "There are no rooms in this lobby!";
            return;
        }
        NoRooms.text = "";

        foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
        {
            GameObject roomPrefab = Instantiate(RoomPrefab);
            roomPrefab.transform.SetParent(RoomParent);
            roomPrefab.GetComponentInChildren<Button>().onClick.AddListener(delegate () { JoinRoom(roomInfo.Name); });
            roomPrefab.GetComponentInChildren<Text>().text = roomInfo.Name + " | " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers + " player";
        }
    }

    void OnReceivedRoomListUpdate()
    {
        ShowAllRooms();
    }
}
