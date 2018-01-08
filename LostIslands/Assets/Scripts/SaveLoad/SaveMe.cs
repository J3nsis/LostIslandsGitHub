using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class SaveMe : MonoBehaviour
{

    public bool SaveInit, SaveDestroy, SaveRotation, SavePosition;

    public string PrefabName;//name des Prefabs bei init, damit es gefunden werden kann beim laden

    [HideInInspector]
    public bool nothingselected;

    int anzahl;

    void Start()
    {       
        if (SaveInit)
        {
            if (PrefabName != null)
            {
                SaveLoadObjects.instance.InitPrefabNames.Add(PrefabName); //name ect muss auch noch gespeichert werden, oder?
                SaveLoadObjects.instance.InitNames.Add(this.gameObject.name);
            }
            else
            {
                print("cant save" + this.gameObject.name + "because no prefab name");
            }
        }

       
        foreach (var gameObj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (gameObj.name == this.transform.name)
            {
                anzahl += 1;
            }
        }
        if (anzahl > 1)
        {
            Debug.LogWarning("[SAVEME] more than one Object with name: " + this.transform.name);
        }
    }   

    private void Update()
    {
        if (!SaveInit && !SaveDestroy && !SaveRotation && !SaveRotation)
        {
            nothingselected = true;
        }
        else
        {
            nothingselected = false;
        }
    }

    private void OnMouseDown()
    {
        //Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        
        if (SaveDestroy == true)
        {
            if (!SaveLoadObjects.instance.alreadyDestryoedGOs.Contains(gameObject.transform.name))
            {
                SaveLoadObjects.instance.alreadyDestryoedGOs.Add(gameObject.transform.name);
            }
        }
    }
}

