using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscMenu : MonoBehaviour {

    public GameObject EscPanel;

    public Button SaveButton;
    public GameObject NormalEsc;
    public GameObject SettingsEsc;

    public bool inEsc;

    public AudioMixer audioMixer;
    public GameObject directionalLight;

	void Start () {
        CloseEsc();
        LoadSettings();

        if (!PhotonNetwork.isMasterClient)
        {
            SaveButton.interactable = false;
        }
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inEsc) 
            {
                CloseEsc();
            }
            else
            {
                OpenEsc();
            }

        }
        SaveButton.GetComponentInChildren<Text>().text = "SAVE (Slot:" + SaveLoadManager.instance.currentSlot + ")";
    }

    public void toMenu()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }       
        SceneManager.LoadScene(0);
        SaveLoadManager.instance.currentSlot = 0;
    }
        
    public void Save()
    {
        Net_Manager.instance.GetLocalPlayer().GetComponentInChildren<Net_Host>().SaveAll();
    }

    public void ToSettings()
    {
        NormalEsc.SetActive(false);
        SettingsEsc.SetActive(true);
    }

    public void BacktoEsc()
    {
        NormalEsc.SetActive(true);
        SettingsEsc.SetActive(false);
    }

    public void Close()//Close Game
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.offlineMode = false;
        Application.Quit();
    }

    public void Continue()//back to game
    {
        CloseEsc();
    }

    public void OpenEsc()
    {
        InventoryManager.instance.CloseInventory();
        UIManager.instance.HideHealthbar();
        UIManager.instance.HideChat();
        UIManager.instance.HideCrosshair();
        UIManager.instance.HideMiddleinfo();      
        Cursor.lockState = CursorLockMode.None;
        PlayerController.instance.Pause = true;
        inEsc = true;
        EscPanel.SetActive(true);
        NormalEsc.SetActive(true);
        SettingsEsc.SetActive(false);
        this.GetComponent<RectTransform>().localPosition = Vector3.zero;
        EscPanel.GetComponent<RectTransform>().localPosition = Vector3.zero;
        this.GetComponent<Image>().enabled = true;
    }

    public void CloseEsc()
    {
        Cursor.lockState = CursorLockMode.Locked;
        UIManager.instance.ShowHealthbar();
        UIManager.instance.ShowChat();
        UIManager.instance.ShowCrosshair();
        UIManager.instance.ShowMiddleinfo();
        PlayerController.instance.Pause = false;
        inEsc = false;
        EscPanel.SetActive(false);
        NormalEsc.SetActive(true);
        SettingsEsc.SetActive(false);
        this.GetComponent<Image>().enabled = false;
    }

    //### Settings:

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefsX.SetBool("Fullscreen", isFullscreen);
    }

    public void SetBrightness(float brightness)
    {
        directionalLight.GetComponent<Light>().intensity = brightness;
        PlayerPrefs.SetFloat("Brightness", brightness);
    }

    void LoadSettings()
    {
        audioMixer.SetFloat("volume", PlayerPrefs.GetFloat("Volume"));
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualityLevel"));
        Screen.fullScreen = PlayerPrefsX.GetBool("Fullscreen");
        directionalLight.GetComponent<Light>().intensity = PlayerPrefs.GetFloat("Brightness");

        //jetzt noch Slider ändern!
    }

    
}
