using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadObjects : MonoBehaviour
{
    #region Instance
    public static SaveLoadObjects instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of JSONData found!");
            return;
        }
        instance = this;
    }
    #endregion

    [Serializable]
    public class ObjectsData
    {
        [Serializable]
        public struct SzeneObject
        {
            public int id;
            public string name;

            public bool savePosition;
            public bool saveRotation;

            public Vector3 position;
            public Vector3 rotation;
        }
        public List<SzeneObject> SzeneObjectList;

        public List<string> DestroyedObjectsName;

        public List<string> InitPrefabNames;//entweder zwei listen oder mit struct (weil dictionyray geht ja net)
        public List<string> InitNames;

    }

    public List<string> alreadyDestryoedGOs = new List<string>(); //hier werden immer wenn man speichert die bereits zerstörten Gos IDs hinzugefügt

    public List<GameObject> GameObjectsToSave = new List<GameObject>();//nicht destroyed Objects

    public List<string> InitPrefabNames = new List<string>();
    public List<string> InitNames = new List<string>();

    public string Save() //wird von SaveLoadManager aufgerufen und gibt ObjectsData als String wieder
    {      
        //print("saving objects...");

        ObjectsData objectsData = new ObjectsData();

        objectsData.SzeneObjectList = new List<ObjectsData.SzeneObject>();
        objectsData.DestroyedObjectsName = new List<string>();
        objectsData.InitPrefabNames = new List<string>();
        objectsData.InitNames = new List<string>();

        //#######################################################Positions / Rotations
        GameObjectsToSave.Clear();
        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if(go.GetComponent<SaveMe>() != null)
            {
                GameObjectsToSave.Add(go);
            }
        }

        foreach (GameObject currentGO in GameObjectsToSave)
        {
            ObjectsData.SzeneObject szeneObject;

            if (currentGO.GetComponent<SaveMe>().nothingselected == true)
            {
                continue; //wenn nichts zu speichern dann weiter
            }

            if(currentGO.GetComponent<SaveMe>().SavePosition == false && currentGO.GetComponent<SaveMe>().SaveRotation == false)
            {
                continue; //wenn Position und Rotation nicht gespeichert werden sollen dann weiter
            }

            if (currentGO.GetComponent<SaveMe>().SaveRotation)
            {
                szeneObject.position = currentGO.transform.position;
                szeneObject.savePosition = true;
            }
            else
            {
                szeneObject.position = Vector3.zero;
                szeneObject.savePosition = false;
            }

            if (currentGO.GetComponent<SaveMe>().SaveRotation)
            {
                szeneObject.rotation = currentGO.transform.eulerAngles;
                szeneObject.saveRotation = true;
            }
            else
            {
                szeneObject.rotation = Vector3.zero;
                szeneObject.saveRotation = false;
            }

            szeneObject.name = currentGO.name;
            szeneObject.id = currentGO.GetInstanceID();


            //zu json liste adden
            objectsData.SzeneObjectList.Add(szeneObject);
        }

        //####################################################Destroys
        objectsData.DestroyedObjectsName = alreadyDestryoedGOs;


        //####################################################Inits
        objectsData.InitPrefabNames = InitPrefabNames;
        objectsData.InitNames = InitNames;


        return(JsonUtility.ToJson(objectsData, true));//komplette ObjectData als string zurückgeben (wird dann im SaveLoadManager in File geschrieben)
    }



    public void Load(string path)
    {
        //print("loading objects...");

        ObjectsData objectsData = JsonUtility.FromJson<ObjectsData>(File.ReadAllText(path));

        //#######################################################Inits
        if (objectsData.InitPrefabNames.Count != objectsData.InitNames.Count)
        {
            Debug.LogError("[SaveLoadObjects] InitPrefabNames.Count != InitNames.Count");
        }

        for (int i = 0; i < objectsData.InitPrefabNames.Count; i++)
        {
            GameObject go = Instantiate(Resources.Load("Prefabs/" + InitNames[i]) as GameObject);//was ist mit name etc???
            go.name = objectsData.InitNames[i];
        }

        //#######################################################Destroys
        foreach (string GoName in objectsData.DestroyedObjectsName)
        {
            Destroy(GameObject.Find(GoName));
        }

        //#######################################################Positions / Rotations
        foreach (var szeneObject in objectsData.SzeneObjectList)
        {
            GameObject currentGO = GameObject.Find(szeneObject.name);
            if (currentGO == null)
            {
                print("cant find GO: " + szeneObject.name);
                continue;
            }

            currentGO.transform.name = szeneObject.name;

            if (szeneObject.savePosition)
            {
                currentGO.transform.position = szeneObject.position;
            }

            if (szeneObject.saveRotation)
            {
                currentGO.transform.eulerAngles = szeneObject.rotation;
            }
        }
    }

    /*GameObject FindObjectWithID(int id)
    {
        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if (go.transform.GetInstanceID() == id)
            {
                return go;
            }
        }
        print("cant find Go per Instance id");
        return null;
    }*/

}