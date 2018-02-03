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

    public int currentSlot;//wird von Button in Menu gesetzt (bei Host), 

    //string pathToSlot;

    [SerializeField]
    Slider LoadingScreenProzessSlider;

    [Serializable]
    public class WorldData //Welt speichert nur Host
    {
        public SaveLoadObjects.ObjectsData objectsData;

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
                if (currentSlot == 0)
                {
                    Debug.LogError("[SaveLoadManager] Slot == 0!");
                }
                break;

            default:
                break;
        }

        if (LoadingScreenProzessSlider == null && inMenu)
        {
            LoadingScreenProzessSlider = MainMenuManager.instance.LoadingProzess;
        }
        
    }

    private void LateUpdate()
    {
        print(currentSlot);
    }

    public void Save(string pathforJsons, bool SaveWorldData)//geladen und gespeichert wird über Net_Host, was am Host hängt
    {
        
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
        print("Saving... in "  + pathforJsons);
        Chat.instance.NewInfo("Saving... to " + pathforJsons);

        //###Ordner erstellen
        CreateDirectoryIfNotExists(Application.dataPath + "/SaveGames");
        CreateDirectoryIfNotExists(Application.dataPath + "/SaveGames/Online");
        CreateDirectoryIfNotExists(Application.dataPath + "/SaveGames/Online/Host");
        CreateDirectoryIfNotExists(Application.dataPath + "/SaveGames/Online/Join");
        CreateDirectoryIfNotExists(Application.dataPath + "/SaveGames/Offline");
        
        if (!Directory.Exists(pathforJsons))
        {
            Directory.CreateDirectory(pathforJsons);//Slot Ordner erstellen
        }
        //###

        //### WorldData abspeichern (sollte nur Host)
        if (SaveWorldData)
        {
            CreateFileIfNotExists(pathforJsons + "/WorldData.json");
            WorldData worldData = new WorldData
            {
                objectsData = SaveLoadObjects.instance.GetObjectsData(),
                lastSave = DateTime.Now.ToString(),
                Day = DayNightCircle.instance.Day //hier später mit Game verbinden
            };
            File.WriteAllText(pathforJsons + "/WorldData.json", JsonUtility.ToJson(worldData, true));
        }
        //###

        //### PlayerStats (Health etc.) abspeichern
        CreateFileIfNotExists(pathforJsons + "/PlayerStats.json");
        File.WriteAllText(pathforJsons + "/PlayerStats.json", JsonUtility.ToJson(PlayerStats.instance.ps, true));
        //###

        //### InventarSlots abspeichern
        CreateFileIfNotExists(pathforJsons + "/InventoryItems.json");
        File.WriteAllText(pathforJsons + "/InventoryItems.json", InventoryItems.instance.GetInventoryItemsSaveAsString());
        //###
    }

    public void Load(string pathforJsons, bool LoadWorldDataFromLocal, string WorldDataString = "")//geladen und gespeichert wird über Net_Host, was am Host hängt
    {
        alreadyLoaded = true;

        if (inMenu)
        {
            print("cannot load in menu!");
            return;
        }
        if (!File.Exists(pathforJsons + "/WorldData.json") && !File.Exists(pathforJsons + "/PlayerStats.json"))//Wenn noch nichts abgespeichert wurde
        {
            print("no saveGame to load in Directory: " + pathforJsons);
            return;
        }
        print("Loading... from " + pathforJsons);
        Chat.instance.NewInfo("Loading... from " + pathforJsons);

        //### WorldData laden (sollte/kann nur Host, weil nur er WorldData abspeichert)
        if (LoadWorldDataFromLocal)
        {
            if (WorldDataString != "")
            {
                Debug.LogWarning("WorldDataString != null but LoadWorldDataFromLocal == true. Now loaded from local!");
            }
            WorldData worldData = JsonUtility.FromJson<WorldData>(File.ReadAllText(pathforJsons + "/WorldData.json"));

            SaveLoadObjects.instance.Load(worldData.objectsData);
            DayNightCircle.instance.Day = worldData.Day;
        }
        
        if (WorldDataString != "")
        {
            WorldData worldData = JsonUtility.FromJson<WorldData>(WorldDataString);

            SaveLoadObjects.instance.Load(worldData.objectsData);
            DayNightCircle.instance.Day = worldData.Day;
        }


        //### PlayerStats laden
        if (File.Exists(pathforJsons + "/PlayerStats.json")) print("exists");
        PlayerStats.instance.ps = JsonUtility.FromJson<PlayerStats.PlayerStatsSave>(File.ReadAllText(pathforJsons + "/PlayerStats.json"));
        //###

        //### InventarSlots laden
        InventoryItems.instance.LoadInventory(JsonUtility.FromJson<InventoryItems.InventoryItemsSave>(File.ReadAllText(pathforJsons + "/InventoryItems.json")));
        //###
    }

    public void OpenGameScene()
    {
        if (inGame)
        {
            print("Cant open scene when in game, return to menu!");
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

    public WorldData GetWorldbyJsonsPath(string pathforJsons)//für loading buttons
    {       
        if (File.Exists(pathforJsons + "/WorldData.json"))
        {
            return JsonUtility.FromJson<WorldData>(File.ReadAllText(pathforJsons + "/WorldData.json"));
        }
        return null;      
    }

    public void ClearSlot(int slot, bool Offline)
    {
        string pathToSlot;
        if (Offline)
        {
            pathToSlot = Application.dataPath + "/SaveGames/Offline/slot" + slot;
        }
        else
        {
            pathToSlot = Application.dataPath + "/SaveGames/Online/Host/slot" + slot;
        }
         
        if (Directory.Exists(pathToSlot))
        {
            Directory.Delete(pathToSlot, true);
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

    void CreateDirectoryIfNotExists(string pathWithDirectoryName)
    {
        if (!Directory.Exists(pathWithDirectoryName))
        {
            Directory.CreateDirectory(pathWithDirectoryName);
        }
    }

    void CreateFileIfNotExists(string pathWithFileName)
    {
        if (!File.Exists(pathWithFileName))
        {
            File.WriteAllText(pathWithFileName, "");//erstellt file
        }
    }

    public string GetWorldDataStringFromLocal()
    {
        if (PhotonNetwork.isMasterClient)
        {
            string pathToWorldData = Application.dataPath + "/SaveGames/Online/Host/slot" + currentSlot + "/WorldData.json";
            if (File.Exists(pathToWorldData))
            {
                return JsonUtility.ToJson(JsonUtility.FromJson<WorldData>((File.ReadAllText(pathToWorldData))));
            }
            Debug.LogWarning("Cannot get Json of WorldData because the file does not exist: " + pathToWorldData);
            return null;

            
        }
        else
        {
            Debug.LogWarning("Cannot get WorldDataString because you are not the Host/MasterClient!");            
        }
        return null;
    }
}
