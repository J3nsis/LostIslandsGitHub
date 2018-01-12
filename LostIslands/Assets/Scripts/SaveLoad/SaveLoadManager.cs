using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour {

    #region Instance
    public static SaveLoadManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            //Debug.LogWarning("More than one instance of SaveLoadManager found!");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }
    #endregion

    bool inMenu;
    bool inGame;
    bool alreadyLoaded;

    public int currentSlot;//wird von Button in Menu befüllt

    string pathToSlot;

    [SerializeField]
    Slider LoadingScreenProzessSlider;

    [Serializable]
    public class SlotData
    {
        public string lastSave;
        public int Day;
    }

    private void Start()
    {
        LoadingScreenProzessSlider = MainMenuManager.instance.LoadingProzess;
    }

    void Update()
    {

    switch (SceneManager.GetActiveScene().name)
        {
            case "Menu":
                inGame = false;
                inMenu = true;
                alreadyLoaded = false;
                break;

            case "Game":
                inGame = true;
                inMenu = false;
                break;

            default:
                break;
        }


        if (inGame && alreadyLoaded == false)//wenn inGame und noch nicht geladen
        {
            if (currentSlot >= 1 || currentSlot <= 6)
            {
                Load(currentSlot, PhotonNetwork.offlineMode);
            }
        }

        if (LoadingScreenProzessSlider == null && inMenu)
        {
            LoadingScreenProzessSlider = MainMenuManager.instance.LoadingProzess;
        }
    }
	
	public void Save(bool Offline)
    {
        string OnlineOffline = "Online";
        if (Offline) { OnlineOffline = "Offline"; }

        pathToSlot = Application.dataPath + "/SaveGames/" + OnlineOffline + "/slot" + currentSlot;

        if (inMenu)
        {
            print("cannot save when in menu!"); 
            return;
        }
        if (currentSlot < 1 || currentSlot > 6)
        {
            print("no or wrong slot id, saving canceled");
            return;
        }

        print("Saving...");

        //### Ordner erstellen
        if (!Directory.Exists(Application.dataPath + "/SaveGames"))
        {
            Directory.CreateDirectory(Application.dataPath + "/SaveGames");
        }
        if (!Directory.Exists(pathToSlot))
        {
            Directory.CreateDirectory(pathToSlot);
        }
        //###

        //### SlotData in json speichern
        if (!File.Exists(pathToSlot + "/SlotData.json"))
        {
            File.WriteAllText(pathToSlot + "/SlotData.json", "");//erstellt file
        }

        SlotData slotData = new SlotData
        {
            lastSave = DateTime.Now.ToString(),
            Day = DayNightCircle.instance.Day //hier später mit Game verbinden
        };

        File.WriteAllText(pathToSlot + "/SlotData.json", JsonUtility.ToJson(slotData, true));
        //###
        
        //### ObjectsData in json speichern
        if (!File.Exists(pathToSlot + "/ObjectsData.json"))
        {
            File.WriteAllText(pathToSlot + "/ObjectsData.json", "");//erstellt file
        }
        File.WriteAllText(pathToSlot + "/ObjectsData.json", SaveLoadObjects.instance.Save());
        //###

        //### PlayerStats (Health etc.) abspeichern
        if(!File.Exists(pathToSlot + "/PlayerStats.json"))
        {
            File.WriteAllText(pathToSlot + "/PlayerStats.json", "");//erstellt file
        }
        File.WriteAllText(pathToSlot + "/PlayerStats.json", JsonUtility.ToJson(PlayerStats.instance.ps, true));
        //###

        //### InventarSlots abspeichern
        if (!File.Exists(pathToSlot + "/InventoryItems.json"))
        {
            File.WriteAllText(pathToSlot + "/InventoryItems.json", "");//erstellt file
        }
        File.WriteAllText(pathToSlot + "/InventoryItems.json", JsonUtility.ToJson(InventoryItems.instance.SaveInventory(), true));
        //###
    }

    public void Load(int slot, bool Offline)
    {
        string OnlineOffline = "Online";
        if (Offline) { OnlineOffline = "Offline"; }

        pathToSlot = Application.dataPath + "/SaveGames/" + OnlineOffline + "/slot" + currentSlot;

        if (inMenu)
        {
            print("cannot load in menu!");
            return;
        }
        if (!File.Exists(pathToSlot + "/SlotData.json"))//Wenn nichts in slot ordner also wenn nichts gespeichert wurde
        {
            print("no saveGame to load in slot" + currentSlot);
            return;
        }
        alreadyLoaded = true;

        print("Loading...");

        //### SlotData laden
        SlotData slotData = JsonUtility.FromJson<SlotData>(File.ReadAllText(pathToSlot + "/SlotData.json"));
        DayNightCircle.instance.Day = slotData.Day;
        //###

        //### ObjectsData laden
        SaveLoadObjects.instance.Load(pathToSlot + "/ObjectsData.json");
        //###

        //### PlayerStats laden
        PlayerStats.instance.ps = JsonUtility.FromJson<PlayerStats.PlayerStatsSave>(File.ReadAllText(pathToSlot + "/PlayerStats.json"));
        //###

        //### InventarSlots laden
        InventoryItems.instance.LoadInventory(JsonUtility.FromJson<InventoryItems.InventoryItemsSave>(File.ReadAllText(pathToSlot + "/InventoryItems.json")));
        //###
    }

    public void OpenGameScene()
    {
        if (inGame)
        {
            print("cant open scene when in Game,return to menu!");
            return;
        } 
        StartCoroutine(OpenGameSceneIE());
    }

    IEnumerator OpenGameSceneIE()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Game");

        while (!operation.isDone)
        {
            PhotonNetwork.isMessageQueueRunning = false;
            MainMenuManager.instance.ShowLoadingScreen();
            

            float progress = Mathf.Clamp01(operation.progress / .9f);

            LoadingScreenProzessSlider.value = progress;

            yield return null;
        }
        if (operation.isDone)
        {
            PhotonNetwork.isMessageQueueRunning = true;
        }
    }

    public SlotData GetSlotDatabySlot(int slot, bool Offline)//für loading buttons
    {
        string OnlineOffline = "Online";
        if (Offline) { OnlineOffline = "Offline"; }
       
        if (File.Exists(Application.dataPath + "/SaveGames/" + OnlineOffline + "/slot" + slot + "/SlotData.json"))
        {
            return JsonUtility.FromJson<SlotData>(File.ReadAllText(Application.dataPath + "/SaveGames/" + OnlineOffline + "/slot" + slot + "/SlotData.json"));
        }

        return null;        
    }

    public void ClearSlot(int slot, bool Offline)
    {
        string OnlineOffline = "Online";
        if (Offline) { OnlineOffline = "Offline"; }

        if (Directory.Exists(Application.dataPath + "/SaveGames/" + OnlineOffline + "/slot" + slot))
        {
            Directory.Delete(Application.dataPath + "/SaveGames/" + OnlineOffline + "/slot" + slot, true);
            if (Offline)
            {
                OfflineSelectionManager.instance.FillLoadButtons();
            }
            else
            {
                OnlineSelectionManager.instance.FillLoadButtons();
            }
        }
        
    }

    public void ClearAllSlots(bool Offline)
    {
        for (int i = 1; i < 7; i++)
        {
            ClearSlot(i, Offline);
        }
    }

}
