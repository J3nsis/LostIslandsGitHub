using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour {

    [SerializeField]
    InputField maxplayers, roomname;

    [SerializeField]
    Button CreateButton;


    int MaxPlayers = 2;
    string RoomName = "DevTestRoom";

    private void Update()
    {
        if (maxplayers.text == "" || roomname.text == "" || maxplayers.text == "1")
        {
            CreateButton.interactable = false;
        }
        else
        {
            CreateButton.interactable = true;
        }
    }

    public void OnCreateRoomButton()
    {
        CreateButton.interactable = false;
        if (PhotonNetwork.connectionStateDetailed != ClientState.JoinedLobby)
        {
            Debug.LogWarning("cant create room when not joined the lobby");
            return;
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

        foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())//wenn raumname vergeben
        {
            if (roomInfo.Name == RoomName)
            {
                print("Room with this name already taken");
                roomname.text = "";
                return;
            }
        }


        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)MaxPlayers,
            IsVisible = true,
            IsOpen = true         
        };
        if (!PhotonNetwork.CreateRoom(RoomName, roomOptions, TypedLobby.Default))//wenn fehler beim raum erstellen
        {
            MainMenuManager.instance.ToMainSelection();
        }
        else
        {
            print("Room sucessfully created");
        }
    }

  
    public void OnJoinedRoom()//rest mit lobby joinen etc. in menu
    {
        MainMenuManager.instance.ShowRoomView();
        ViewRoom.instance.InitializeRoomView();
    }
}
