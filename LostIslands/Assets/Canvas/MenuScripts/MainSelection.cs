using System.Collections;
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
        if (PhotonNetwork.connectionStateDetailed == ClientState.Disconnected || PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated)
        {
            PhotonNetwork.ConnectUsingSettings("GameVersion");
        }


        if (PhotonNetwork.connectionStateDetailed == ClientState.ConnectedToMaster)
        {
            PhotonNetwork.JoinLobby(new TypedLobby("Lobby", LobbyType.Default));
        }

        if (PhotonNetwork.connectionStateDetailed == ClientState.JoinedLobby)
        {
            JoinButton.interactable = true;
            HostButton.interactable = true;
            DevQuickJoin.interactable = true;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveLoadManager.instance.OpenGameScene();
            SaveLoadManager.instance.currentSlot = 6;
        }
    }

    void OnJoinedLobby()
    {
        PhotonNetwork.playerName = PlayerPrefs.GetString("Username");
    }


}
