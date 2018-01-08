using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

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

    public Transform ParentRect;
    public GameObject TextPrefab;
    public GameObject WarningPrefab;
    public GameObject MessagePrefab;

    public InputField inputField;

    string lastInput;


    void Update()
    {
        if (Input.GetKey(KeyCode.T) || inputField.isFocused)
        {
            inputField.ActivateInputField();
            Cursor.lockState = CursorLockMode.None;
            PlayerController.instance.Pause = true;
        }

        if (Input.GetKey(KeyCode.Return) && inputField.text != "")//Wenn Enter
        {
            string input = "";

            input = inputField.text;
            lastInput = input;

            if (!input.StartsWith("/"))//wenn kein Befehl
            {
                NewMessage(input);
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
                else
                {
                    Error();
                    return;
                }

                 //alle commands hier müssen klein geschrieben sein, in Chat ist es egal
                command.ToLowerInvariant();//geht nicht???!!
                switch (command)
                {
                    case "/add":
                        if (inputArray.Length >= 3)
                        {
                            parameter1 = inputArray[1];
                            parameter2 = inputArray[2];
                        }
                        else
                        {
                            Error();
                            return;
                        }
                        InventoryItems.instance.AddItembySlug(parameter1, int.Parse(parameter2));
                        break;

                    case "/remove":
                        if (inputArray.Length >= 3)
                        {
                            parameter1 = inputArray[1];
                            parameter2 = inputArray[2];
                        }
                        else
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

                    default:
                        NewWarning("Unknown command, type /help for all commands");
                        break;
                }
            }

            if (InventoryManager.instance.inInventory == false)//nur wenn nicht im Inventar
            {
                Cursor.lockState = CursorLockMode.Locked;
                PlayerController.instance.Pause = false;
            }
                
            

            inputField.text = "";

        }

        if (Input.GetKey(KeyCode.UpArrow))//letzen Command wiederholen
        {
            inputField.text = lastInput;
        }
    }

    void Error()
    {
        NewWarning("unknown command");
        inputField.text = "";
    }

    void NewMessage(string info)
    {
        GameObject Message = Instantiate(MessagePrefab);
        Message.transform.SetParent(ParentRect);
        Message.GetComponent<Text>().text = info;

        Message.GetComponent<DestroyMe>().Destroy(20f);
    }

    public void NewInfo(string info)
    {
        GameObject Info = Instantiate(TextPrefab);
        Info.transform.SetParent(ParentRect);
        Info.GetComponent<Text>().text = info;

        Info.GetComponent<DestroyMe>().Destroy(20f);
    }

    public void NewWarning(string info)
    {
        GameObject InfoW = Instantiate(WarningPrefab);
        InfoW.transform.SetParent(ParentRect);
        InfoW.GetComponent<Text>().text = info;

        InfoW.GetComponent<DestroyMe>().Destroy(20f);
    }


}

