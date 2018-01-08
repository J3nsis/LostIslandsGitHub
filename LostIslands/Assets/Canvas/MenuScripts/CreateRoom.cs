using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour {

    [SerializeField]
    InputField maxplayers, roomname;

    int MaxPlayers = 2;
    string RoomName = "DevTestRoom";

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

    public void OnCreateRoomButton()
    {
        if (PhotonNetwork.connectionState == ConnectionState.Connecting)
        {
            //return;
        }
        
        
        if (maxplayers.text != "" && roomname.text != "")
        {
            MaxPlayers = Convert.ToInt32(maxplayers.text);

            RoomName = roomname.text;
        }
        else if (maxplayers.text == "" || roomname.text == "")
        {
            return;
        }
        

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)MaxPlayers; 
        roomOptions.IsVisible = true;
        if (PhotonNetwork.CreateRoom(RoomName, roomOptions, null))
        {
            print("room created");
        }
        else
        {
            print("create room fail");
        }
    }

  
    public void OnJoinedRoom()//rest mit lobby joinen etc. in menu
    {
        MainMenuManager.instance.ShowRoomView();
        ViewRoom.instance.InitializeRoomView();
    }
}
