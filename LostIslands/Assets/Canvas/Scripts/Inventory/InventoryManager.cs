using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    #region Instance
    public static InventoryManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of InventoryController found!");
            return;
        }
        instance = this;
    }
    #endregion

    public GameObject InventoryPanel;
    public bool inInventory;

    public GameObject ChestTabButton;
    public GameObject ChestTab;
    public GameObject PlayerTab;
    public GameObject CraftingTab;

    public InputField ChatInputField;


    void Start () 
	{
        CloseInventory();

        ChestTabButton.SetActive(false);
        OpenTabPlayer();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !ChatInputField.GetComponent<InputField>().isFocused)
        {
            if (PlayerStats.instance.ps.hasBackpack) //nur wenn Rucksack vorhanden
            {
                if (inInventory == false) //rein
                {
                    OpenInventory();
                }
                else if (inInventory == true) //raus
                {
                    CloseInventory();
                }
            }
            else
            {
                Chat.instance.NewWarning("Get a backpack!");
            }
        }
    }

    public void OpenInventory()
    {
        UIManager.instance.HideHealthbar();
        UIManager.instance.ShowChatinInventory();
        UIManager.instance.HideCrosshair();
        UIManager.instance.HideMiddleinfo();
        InventoryPanel.transform.localPosition= new Vector3(0, 0);
        PlayerController.instance.Pause = true;
        Cursor.lockState = CursorLockMode.None;
        inInventory = true;
    }

    public void CloseInventory()
    {
        UIManager.instance.ShowHealthbar();
        UIManager.instance.ShowChat();
        UIManager.instance.ShowCrosshair();
        UIManager.instance.ShowMiddleinfo();
        InventoryPanel.transform.localPosition = new Vector3(0, 1250);
        PlayerController.instance.Pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        inInventory = false;
    }

    public void OpenTabPlayer()
    {
        ChestTabButton.SetActive(false);

        PlayerTab.SetActive(true);
        CraftingTab.SetActive(false);
        ChestTab.SetActive(false);
    }

    public void OpenTabCrafting()
    {
        ChestTabButton.SetActive(false);

        PlayerTab.SetActive(false);
        CraftingTab.SetActive(true);
        ChestTab.SetActive(false);
    }

    void OpenTabChest()
    {
        ChestTabButton.SetActive(true);

        ChestTab.SetActive(true);
        PlayerTab.SetActive(false);
        CraftingTab.SetActive(false);
    }
}
