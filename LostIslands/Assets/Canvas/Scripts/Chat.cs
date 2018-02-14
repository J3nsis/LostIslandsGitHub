using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

[RequireComponent(typeof(PhotonView))]
public class Chat : MonoBehaviour{
    #region Instance
    public static Chat instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Chat found!");
            return;
        }
        instance = this;
    }
    #endregion

    public Transform ParentRect;//Messages Parent
    public GameObject TextPrefab;
    public GameObject WarningPrefab;
    public GameObject MessagePrefab;

    public InputField inputField;

    string lastInput;

    public bool inChat;

    FirstPersonPlayerMovement fppm;


    private void Start()
    {
        fppm = Net_Manager.instance.GetLocalPlayer().GetComponent<FirstPersonPlayerMovement>();
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.T) || inputField.isFocused)
        {
            inputField.ActivateInputField();
            Cursor.lockState = CursorLockMode.None;
            fppm.Pause = true;
        }

        if (Input.GetKey(KeyCode.Return) && inputField.text != "")//Wenn Enter
        {
            string input = "";

            input = inputField.text;
            lastInput = input;

            if (!input.StartsWith("/"))//wenn kein Befehl
            {
                if (PhotonNetwork.offlineMode)
                {
                    NewMessage(input, PhotonNetwork.player.NickName, false);
                }
                else
                {
                    NewMessage(input, PhotonNetwork.player.NickName, true);
                }
               
            }
            else if (input.StartsWith("/"))//wenn Befehl
            {
                string[] inputArray;              
                string command = "";
                string parameter1 = "";
                string parameter2 = "";

                inputArray = input.Split(' ');               

                if (inputArray.Length >= 1)
                {
                    command = inputArray[0];
                }
                if(inputArray.Length >= 2)
                {
                    parameter1 = inputArray[1];
                }
                if (inputArray.Length == 3)
                {
                    parameter2 = inputArray[2];
                }
                if (inputArray.Length <= 0 || inputArray.Length > 3)
                {
                    Error();
                    return;
                }

                command = command.Replace("/", "");
                command = command.ToLower();
                command = "/" + command;

                switch (command)//alle commands klein!
                {
                    case "/add":
                        if (parameter1 == "" || parameter2 == "")
                        {
                            Error();
                            return;
                        }
                        InventoryItems.instance.AddItembySlugorID(parameter1, int.Parse(parameter2));
                        break;

                    case "/remove":
                        if (parameter1 == "" || parameter2 == "")
                        {
                            Error();
                            return;
                        }
                        InventoryItems.instance.RemoveItembySlug(parameter1, int.Parse(parameter2));
                        break;
                    case "/heal":
                        PlayerStats.instance.Heal();
                        NewInfo("You were healed!");
                        break;
                    case "/help":
                        NewInfo("This is not implemented yet");
                        break;
                    case "/kill":
                        PlayerStats.instance.KillPlayer();
                        break;
                    case "/setlevel":
                        if (parameter1 == "")
                        {
                            Error();
                            return;
                        }
                        PlayerStats.instance.SetLevel(int.Parse(parameter1));
                        break;

                    default:
                        NewWarning("Unknown command, type /help for all commands");
                        break;
                }
            }

            if (InventoryManager.instance.inInventory == false)//wenn enter und nicht im Inventar
            {
                Cursor.lockState = CursorLockMode.Locked;
                fppm.Pause = false;
            }
            inputField.text = "";
        }

        if (Input.GetKey(KeyCode.UpArrow))//letzen Command wiederholen
        {
            inputField.text = lastInput;
        }

        foreach (GameObject Go in GameObject.FindGameObjectsWithTag("ChatMessage"))//damit Messages von anderen Spielern auch richtigen Parent haben weil dieser beim Message Instatiate nicht sync wird!
        {
            if (Go.transform.parent != ParentRect)
            {
                Go.transform.SetParent(ParentRect);
            }
        }
    }

    void Error()
    {
        NewWarning("unknown command");
        inputField.text = "";
    }

    public void NewMessage(string info, string PlayerName = "", bool Online = false)
    {
        if (Online)
        {
            GameObject Message = PhotonNetwork.Instantiate("ChatMessage", Vector3.zero, Quaternion.identity,0);//geht noch nicht Online, weil Nachricht Parent und Text nicht sync werden!!!
            Message.transform.SetParent(ParentRect);
            
            if (PlayerName == null)
            {
                Message.GetComponent<Text>().text = info;
            }
            else
            {
                Message.GetComponent<Text>().text = "[" + PlayerName + "]: " + info;
            }
            Message.GetComponent<Net_SyncMessage>().MessageText = Message.GetComponent<Text>().text;
            Message.GetComponent<Net_ObjectsSyncHandler>().Net_DestroyMe(20f);
        }
        else
        {
            GameObject Message = Instantiate(MessagePrefab);
            Message.transform.SetParent(ParentRect);
            if (PlayerName == null)
            {
                Message.GetComponent<Text>().text = info;
            }
            else
            {
                Message.GetComponent<Text>().text = "[" + PlayerName + "]: " + info;
            }

            Message.GetComponent<Net_ObjectsSyncHandler>().Net_DestroyMe(20f);
        }

        
    }


    public void NewInfo(string info)
    {
        GameObject Info = Instantiate(TextPrefab);
        Info.transform.SetParent(ParentRect);
        Info.GetComponent<Text>().text = info;

        Info.GetComponent<Net_ObjectsSyncHandler>().DestroyMe(20f); 
    }

    public void NewWarning(string info)
    {
        GameObject InfoW = Instantiate(WarningPrefab);
        InfoW.transform.SetParent(ParentRect);
        InfoW.GetComponent<Text>().text = info;

        InfoW.GetComponent<Net_ObjectsSyncHandler>().DestroyMe(20f);
    }


}

