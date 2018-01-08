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

            SaveLoadManager.SlotData slotData = SaveLoadManager.instance.GetSlotDatabySlot(slot, false);

            LoadButton.transform.GetChild(0).GetComponent<Text>().text = "Slot: " + slot.ToString();//Slot Nr (Child 0)

            if (slotData != null)
            {
                LoadButton.transform.GetChild(1).GetComponent<Text>().text = slotData.lastSave;//last Save (Child 1)
                LoadButton.transform.GetChild(2).GetComponent<Text>().text = "Day: " + slotData.Day;
            }
            else
            {
                LoadButton.transform.GetChild(1).GetComponent<Text>().text = "Emty slot!";
                LoadButton.transform.GetChild(2).GetComponent<Text>().text = "";
            }

            LoadButton.GetComponent<Button>().onClick.AddListener(delegate () { OnSlotChosed(slot); });
            LoadButton.GetComponentInChildren<Button>().onClick.AddListener(delegate () { OnClearSlotPressed(slot); });
        }
    }

    public void OnSlotChosed(int slot)//wird von Load button in Menu aufgerufen
    {

        SaveLoadManager.instance.currentSlot = slot;
        MainMenuManager.instance.ShowRoomCreate();
        
        //später!
        //SaveLoadManager.instance.OpenGameScene(); //schon drin
        //SaveLoadManager.instance.currentSlot = slot;
    }

    public void OnClearSlotPressed(int slot)
    {
        SaveLoadManager.instance.ClearSlot(slot, false);
    }
}
