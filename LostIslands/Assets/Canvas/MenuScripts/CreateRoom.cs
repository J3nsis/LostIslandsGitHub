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
        if (PhotonNetwork.connectionState != ConnectionState.Connected)
        {
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

        foreach (Room room in PhotonNetwork.GetRoomList())//wenn raumname vergeben
        {
            if (room.Name == RoomName)
            {
                print("Room with this name already taken");
                roomname.text = "";
            }
        }


        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)MaxPlayers; 
        roomOptions.IsVisible = true;
        if (!PhotonNetwork.CreateRoom(RoomName, roomOptions, null))//wenn fehler beim raum erstellen
        {
            MainMenuManager.instance.ToMainSelection();
        }
    }

  
    public void OnJoinedRoom()//rest mit lobby joinen etc. in menu
    {
        MainMenuManager.instance.ShowRoomView();
        ViewRoom.instance.InitializeRoomView();
    }
}
