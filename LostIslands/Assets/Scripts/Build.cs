using UnityEngine;
using System;
using System.Collections;

public class Build : MonoBehaviour {

    #region Instance
    public static Build instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Build found!");
            return;
        }
        instance = this;
    }
    #endregion

    GameObject Player;

    GameObject Structure;
    string currentStructureName;
    public Color normalcolor;


    bool blocked;
    bool ausgerichtet;

    enum State { none, Placing}; // none auch wenn fertig gebaut
    State BuildState;



    void Start () 
	{
        Player = FindObjectOfType<Net_Player>().gameObject;

        BuildState = State.none;
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            //ShowStructuretoPlace("Wall");
        }
        
                
         //print(blocked);

        if (BuildState == State.Placing)
        {
            PlayerController.instance.onlyBlockMoving = true;

            if (ausgerichtet == false)
            {
                Vector3 vec = Structure.transform.eulerAngles;
                vec.x = Mathf.Round(vec.x / 90) * 90;
                vec.y = Mathf.Round(vec.y / 90) * 90;
                vec.z = Mathf.Round(vec.z / 90) * 90;
                Structure.transform.eulerAngles = vec;
            }
            #region Move
            
            int xzGridDistance = 1;
            int yGridDistance = 1;

            if (Input.GetKeyDown(KeyCode.W)) //vor
            {
                Structure.transform.Translate(0,0, xzGridDistance);
            }
            if (Input.GetKeyDown(KeyCode.S)) //zurück
            {
                Structure.transform.Translate(0, 0, -xzGridDistance);
            }
            if (Input.GetKeyDown(KeyCode.A)) //links
            {
                Structure.transform.Translate(-xzGridDistance, 0, 0);
            }
            if (Input.GetKeyDown(KeyCode.D)) //rechts
            {
                Structure.transform.Translate(xzGridDistance, 0, 0);
            }
            if (Input.GetKeyDown(KeyCode.T)) //hoch
            {
                Structure.transform.Translate(0, yGridDistance, 0);
            }
            if (Input.GetKeyDown(KeyCode.G)) //runter
            {
                Structure.transform.Translate(0, -yGridDistance, 0);
            }

            if (Input.GetKeyDown(KeyCode.R)) 
            {
                Structure.transform.Rotate(Structure.transform.rotation.x, Structure.transform.rotation.y + 45, Structure.transform.rotation.z);
            }
            #endregion

            if (blocked == false)
            {
                Structure.GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                Structure.GetComponent<Renderer>().material.color = Color.red;
            }
            
            Structure.GetComponent<Collider>().isTrigger = true;

            if (Input.GetMouseButtonDown(0))//wenn im platzier Modus  maus gedrückt wird dann bauen
            {
                BuildStructure(currentStructureName);
            }

            blocked = !Structure.GetComponent<TriggerTest>().Trigger; //von Trigger Checker script nehmen
        }

        if ((PlayerController.instance.Pause == true && BuildState == State.Placing) || PlayerController.instance.onlyBlockMoving == false)
        {
            CancleBuild();
        }

        

    }

    public void ShowStructuretoPlace(string StructureName)//nur ghost vorschau & wird bei klicken auf struktur im Inventar aufgerufen
    {
        if (BuildState == State.none)
        {
            print("building");
            Structure = Instantiate(Resources.Load("Structures/" + StructureName, typeof(GameObject))) as GameObject;
            Structure.transform.position = Player.transform.position + Player.transform.forward * 3; //vor player
            Structure.transform.rotation = Player.transform.rotation; //in selber rotation wie player

            Structure.transform.position = new Vector3(Mathf.Round(Structure.transform.position.x), Mathf.Round(Structure.transform.position.y), Mathf.Round(Structure.transform.position.z));

            Structure.name = StructureName;
            currentStructureName = StructureName;
            normalcolor = Structure.GetComponent<Renderer>().material.GetColor("_Color");

            BuildState = State.Placing;
        }
        
            
    }

    void BuildStructure(string StructureName)//dann richtig auf Boden gebaut
    {
        if (BuildState == State.Placing) //nur wenn in Platzier Modus
        {
            if (blocked == false)
            {
                PlayerController.instance.onlyBlockMoving = false;
                Structure.name = StructureName + "*" + Time.time; //damit identifizierbar, * damit später getrennt werden kann nur und nur Richtiger Name zum Prefab laden bnutzt wird also nicht die Zeit

                Structure.GetComponent<Renderer>().material.color = normalcolor;
                Structure.GetComponent<Collider>().isTrigger = false;
                BuildState = State.none;
                ausgerichtet = false;

                //InventoryItems.instance.RemoveItemFromInventory(StructureName, 1);
            }
            else
            {
                Chat.instance.NewWarning("Structure must be placed on Ground!");
            }  
        }      
    }

    void CancleBuild()
    {
        if (BuildState == State.Placing)
        {
            BuildState = State.none;
            if (Structure != null) { Destroy(Structure.gameObject); }
            currentStructureName = "";
            PlayerController.instance.onlyBlockMoving = false;
            ausgerichtet = false;
        }           
    }

   
}
