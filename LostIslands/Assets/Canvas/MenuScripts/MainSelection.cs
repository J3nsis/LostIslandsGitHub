﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSelection : MonoBehaviour {

    [SerializeField]
    Button JoinButton, HostButton, OfflineButton, DevQuickJoin;

    private void OnEnable()
    {
        JoinButton.interactable = false;
        DevQuickJoin.interactable = false;
        HostButton.interactable = false;
    }


    private void Update()
    {
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings("GameVersion");
            PhotonNetwork.JoinLobby(new TypedLobby("MyLobby", LobbyType.SqlLobby));
        }

        if (PhotonNetwork.connectionStateDetailed == ClientState.ConnectedToMaster)
        {
            JoinButton.interactable = true;
            HostButton.interactable = true;
            DevQuickJoin.interactable = true;
        }
    }

    void OnJoinedLobby()
    {
        PhotonNetwork.playerName = PlayerPrefs.GetString("Username");
    }


}
