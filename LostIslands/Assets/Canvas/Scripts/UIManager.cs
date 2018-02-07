using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    #region Instance
    public static UIManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of UIManager found!");
            return;
        }
        instance = this;
    }
    #endregion

    public Transform Chat;
    public GameObject HealthbarPanel;
    public GameObject Crosshair;
    public Transform MiddelInfo;

    public Transform InventoryPanel;
    public Text MiddelInfoText;

    void Start () 
	{
        ShowChat();
        ShowHealthbar();
        ClearInfo();
    }

    public void NewInfo(string info)
    {
        MiddelInfoText.text = info;
    }

    public void ClearInfo()
    {
        MiddelInfoText.text = "";
    }


    public void HideChat()
    {
        Chat.transform.localPosition = new Vector3(-1317, -95, 0);
        Chat.SetParent(this.gameObject.transform);
    }

    public void ShowChat()
    {
        Chat.transform.localPosition = new Vector3(-651, -95, 0);
        Chat.SetParent(this.gameObject.transform);
    }

    public void ShowChatinInventory()
    {         
        Chat.SetParent(InventoryPanel);
        Chat.transform.localPosition = new Vector3(-68, -95, 0);
    }

    public void HideHealthbar()
    {
        HealthbarPanel.transform.localPosition = new Vector2(0, 700);
    }


    public void ShowHealthbar()
    {
        HealthbarPanel.transform.localPosition = new Vector2(0, 465);
    }

    public void HideMiddleinfo()
    {
        MiddelInfo.transform.localPosition = new Vector2(0, -611);
    }


    public void ShowMiddleinfo()
    {
        MiddelInfo.transform.localPosition = new Vector2(0, -475);
    }


    public void HideCrosshair()
    {
        Crosshair.SetActive(false);
    }

    public void ShowCrosshair()
    {
        Crosshair.SetActive(true);
    }
}
