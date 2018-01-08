using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

    #region Instance
    public static MainMenuManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of MainMenuManage found!");
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField]
    GameObject MainSelection, OfflineSelection, SetUsername, LoadingScreen, OnlineHostSaveGameSelection, OnlineCreateRoom, OnlineRoomView, OnlineJoin;
    [SerializeField]
    Text Username, ConnectionState;
    [SerializeField]
    InputField inputField;

    [SerializeField]
    Button acceptButton;

    public Slider LoadingProzess;

    bool DevQuickJoin = false;

    public void HideAll()
    {
        LoadingScreen.SetActive(false);
        MainSelection.SetActive(false);
        OfflineSelection.SetActive(false);
        OnlineHostSaveGameSelection.SetActive(false);
        OnlineCreateRoom.SetActive(false);
        OnlineRoomView.SetActive(false);
        OnlineJoin.SetActive(false);
        SetUsername.SetActive(false);
    }

    private void Start()
    {
        HideAll();
        if (PlayerPrefs.GetString("Username") == "")
        {
            SetUsername.SetActive(true);
            MainSelection.SetActive(false);
        }
        else
        {
            SetUsername.SetActive(false);
            MainSelection.SetActive(true);
            Username.text = PlayerPrefs.GetString("Username");
        }
    }

    public void OnOffline()
    {
        MainSelection.SetActive(false);
        OfflineSelection.SetActive(true);
        LoadingScreen.SetActive(false);
        OnlineHostSaveGameSelection.SetActive(false);
        OnlineCreateRoom.SetActive(false);
        OnlineRoomView.SetActive(false);
        OnlineJoin.SetActive(false);
    }

    public void ToMainSelection()
    {
        HideAll();
        MainSelection.SetActive(true);

        PhotonNetwork.offlineMode = false;
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void OnOnlineHost()
    {
        HideAll();
        OnlineHostSaveGameSelection.SetActive(true);

    }

    public void OnOnlineJoin()
    {
        HideAll();
        OnlineJoin.SetActive(true);
        RoomsView.instance.ShowAllRooms();
    }

    public void Quit()
    {
        Application.Quit();
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    //### Username input etc.

    public void OnUsernameAccept()
    {
        if (inputField.GetComponent<InputField>().text != "")
        {
            PlayerPrefs.SetString("Username", inputField.GetComponent<InputField>().text);
            Username.text = PlayerPrefs.GetString("Username");
            ToMainSelection();
        }
    }

    private void Update()
    {
        if (inputField.text == "")
        {
            acceptButton.interactable = false;
        }
        else
        {
            acceptButton.interactable = true;
        }

        if (Input.GetKey(KeyCode.R) && MainSelection.active == true)
        {
            PlayerPrefs.SetString("Username", "");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        ConnectionState.text = PhotonNetwork.connectionStateDetailed.ToString();
    }

    //### 

    public void ShowLoadingScreen()
    {
        HideAll();
        LoadingScreen.SetActive(true);
    }

    //### offline

    
    public void ShowRoomCreate()
    {
        HideAll();
        OnlineCreateRoom.SetActive(true);
    }

    public void ShowRoomView()
    {
        HideAll();
        OnlineRoomView.SetActive(true);
    }

    public void Dev_QuickJoin()
    {
        DevQuickJoin = true;
        PhotonNetwork.ConnectUsingSettings("GameVersion");

        PhotonNetwork.JoinLobby(new TypedLobby("MyLobby", LobbyType.SqlLobby));
        PhotonNetwork.playerName = PlayerPrefs.GetString("Username");
    }

    void OnConnectedToMaster()
    {
        if (DevQuickJoin)
        {
            PhotonNetwork.CreateRoom("DevRoom");
        }
    }

    void OnJoinedRoom()
    {
        if (DevQuickJoin)
        {
            SaveLoadManager.instance.OpenGameScene();
            SaveLoadManager.instance.currentSlot = 6;
        }
    }


}
