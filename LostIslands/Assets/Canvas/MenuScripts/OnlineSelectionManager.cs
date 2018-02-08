using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineSelectionManager : MonoBehaviour {

    #region Instance
    public static OnlineSelectionManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of OnlineSelectionManager found!");
            return;
        }
        instance = this;
    }
    #endregion

    public GameObject LoadButtonsParent;

    void OnEnable()
    {
        FillLoadButtons();
    }

    public void FillLoadButtons()
    {

        for (int i = 0; i < LoadButtonsParent.transform.childCount; i++)
        {
            GameObject LoadButton = LoadButtonsParent.transform.GetChild(i).gameObject;
            int slot = i + 1;

            SaveLoadManager.WorldData worldData = SaveLoadManager.instance.GetWorldbyJsonsPath(Application.dataPath + "/SaveGames/Online/Host/slot" + slot);

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
            LoadButton.GetComponent<Button>().onClick.AddListener(delegate () { OnSlotChosed(slot); });
            LoadButton.transform.Find("Clear").GetComponent<Button>().onClick.AddListener(delegate () { OnClearSlotPressed(slot); });
        }
    }

    public void OnSlotChosed(int slot)//wird von Load button in Menu aufgerufen
    {
        if (slot == 0)
        {
            Debug.LogWarning("Slot == 0, loading stopped!");
            return;
        }
        SaveLoadManager.instance.currentSlot = slot;
        MainMenuManager.instance.ShowRoomCreate();
    }

    int clear = 0;

    public void OnClearSlotPressed(int slot)
    {
        clear += 1; //dann muss man zweimal zum löschen drücken
        if (clear == 2)
        {
            SaveLoadManager.instance.ClearSlot(slot, false);
            clear = 0;
            print("Slot " + slot + " cleared!");
        }
        
    }
}
