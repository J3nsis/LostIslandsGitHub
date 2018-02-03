using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OfflineSelectionManager : MonoBehaviour {

    #region Instance
    public static OfflineSelectionManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of OfflineSelectionManager found!");
            return;
        }
        instance = this;
    }
    #endregion

    public GameObject LoadButtonsParent;

    bool Load;

    void OnEnable()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
        PhotonNetwork.offlineMode = true;
        PhotonNetwork.CreateRoom("OfflineRoomName");

        FillLoadButtons();
    }

    private void OnDisable()
    {
        if (!Load)
        {
            PhotonNetwork.offlineMode = false;
        }
    }

    public void FillLoadButtons()
    {
        for (int i = 0; i < LoadButtonsParent.transform.childCount; i++)
        {
            GameObject LoadButton = LoadButtonsParent.transform.GetChild(i).gameObject;
            int slot = i + 1;

            SaveLoadManager.WorldData worldData = SaveLoadManager.instance.GetWorldbyJsonsPath(Application.dataPath + "/SaveGames/Offline/slot" + slot);

            LoadButton.transform.GetChild(0).GetComponent<Text>().text = "Slot: " + slot.ToString();//Slot Nr (Child 0)

            if (worldData != null)
            {
                LoadButton.transform.GetChild(1).GetComponent<Text>().text = worldData.lastSave;//last Save (Child 1)
                LoadButton.transform.GetChild(2).GetComponent<Text>().text = "Day: " + worldData.Day;
            }
            else
            {
                LoadButton.transform.GetChild(1).GetComponent<Text>().text = "Emty slot!";
                LoadButton.transform.GetChild(2).GetComponent<Text>().text = "";
            }

            LoadButton.GetComponent<Button>().onClick.RemoveAllListeners();
            LoadButton.transform.Find("Clear").GetComponent<Button>().onClick.RemoveAllListeners();
            LoadButton.GetComponent<Button>().onClick.AddListener(delegate () { OpenGameSceneAndLoad(slot); });
            LoadButton.transform.Find("Clear").GetComponent<Button>().onClick.AddListener(delegate () { OnClearSlotPressed(slot); });
        }
    }

    public void OpenGameSceneAndLoad(int slot)//wird von Load button in Menu aufgerufen
    {
        if (slot == 0)
        {
            Debug.LogWarning("Slot == 0, loading stopped!");
            return;
        }
        Load = true;
        SaveLoadManager.instance.OpenGameScene();
        SaveLoadManager.instance.currentSlot = slot;
    }

    int clear = 0;

    public void OnClearSlotPressed(int slot)
    {
        clear += 1; //dann muss man zweimal zum löschen drücken
        if (clear == 2) SaveLoadManager.instance.ClearSlot(slot, true); clear = 0; print("Slot " + slot + " cleared!");
    }
}


