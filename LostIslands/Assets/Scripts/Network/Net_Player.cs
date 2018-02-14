using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Net_Player : MonoBehaviour {

    [SerializeField] GameObject [] GOsToDisable;
    [SerializeField] MonoBehaviour[] ScriptsToDisable;
    [SerializeField] GameObject HumanHead, Hand;

    private PhotonView photonView;

    public bool initialized;

    string WorldDatafromHost;


    private void Awake()
    {
        if (GameObject.Find("Player"))
        {
            Destroy(GameObject.Find("Player"));
        }
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        InitializePlayer();
        if (GameObject.Find("Player"))
        {
            Destroy(GameObject.Find("Player"));
        }

        if (photonView.isMine)
        {
            if (!PhotonNetwork.isMasterClient)//Masterclient läd bei sich alles über Net_Host! Hier nur für Clients
            {
                WorldDatafromHost = Net_Manager.instance.GetWorldDataStringFromHost();
            }
        }
    }

    private void Update()
    {
        print(WorldDatafromHost);
    }

    void InitializePlayer()
    {
        if (photonView.isMine)
        {
            photonView.owner.NickName = PlayerPrefs.GetString("Username");

            HumanHead.layer = 8;
            foreach (Transform child in HumanHead.transform)//damit player sich selbst nicht sieht
            {
                child.gameObject.layer = 8;
            }

            GetComponentInChildren<TextMesh>().text = "";
        }
        else
        {
            foreach (GameObject go in GOsToDisable)
            {
                go.SetActive(false);
                continue;
            }
            foreach (MonoBehaviour m in ScriptsToDisable)
            {
                m.enabled = false;
                continue;
            }

            GetComponent<AudioSource>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;
            GetComponentInChildren<FlareLayer>().enabled = false;
            GetComponentInChildren<GUILayer>().enabled = false;
            Hand.GetComponent<HandManager>().enabled = false;
            GetComponentInChildren<HumanHandController>().enabled = false;

            for (int i = 0; i < Hand.transform.childCount; i++)
            {
                GameObject go = Hand.transform.GetChild(i).gameObject;
                go.SetActive(true);
            }

            foreach (Tool tool in Hand.transform.GetComponentsInChildren<Tool>())
            {
                tool.enabled = false;
            }
            foreach (ToolSwinging toolSwinging in Hand.transform.GetComponentsInChildren<ToolSwinging>())
            {
                toolSwinging.enabled = false;
            }
            foreach (Animator animator in Hand.transform.GetComponentsInChildren<Animator>())
            {
                animator.enabled = false;
            }

            for (int i = 0; i < Hand.transform.childCount; i++)
            {
                GameObject go = Hand.transform.GetChild(i).gameObject;
                go.SetActive(false);
            }
        }

        if (photonView.isMine)
        {
            if (!PhotonNetwork.isMasterClient)//Masterclient läd bei sich alles über Net_Host! Hier nur für Clients
            {
                SaveLoadManager.instance.Load(Application.dataPath + "/SaveGames/Online/Join/" + PhotonNetwork.masterClient.NickName + "/slot" + SaveLoadManager.instance.currentSlot,
                                              false,
                                              WorldDatafromHost);
            }
            else
            {
                GetComponent<Net_Host>().enabled = true;
            }
        }

        photonView.RPC("SetPlayerIdentityInSceneRPC", PhotonTargets.All);
    }

    [PunRPC]
    void SetPlayerIdentityInSceneRPC()
    {
        print("SetPlayerIdentityInSceneRPC" + GetComponent<PhotonView>().owner.NickName);

        name = GetComponent<PhotonView>().owner.NickName;

        foreach (PhotonPlayer pp in PhotonNetwork.playerList)
        {
            if (pp.NickName == name)
            {
                if (pp.IsMasterClient)
                {
                    tag = "Host";
                    GetComponent<Net_Host>().enabled = false; //immer false (nur wenn der lokale Client masterclient und der Spieler)
                }
                else
                {
                    tag = "Player";
                    GetComponent<Net_Host>().enabled = false;
                }
            }
            continue;
        }

        initialized = true;
    }


    Vector3 screenPos;

    [PunRPC]
    void OnGUI()
    {
        if (photonView.isMine)
        {
            return;
        }
        screenPos = Camera.main.WorldToScreenPoint(transform.position);
        screenPos.y = Screen.height - screenPos.y;

        GetComponentInChildren<TextMesh>().text = this.name;
        GetComponentInChildren<TextMesh>().gameObject.transform.LookAt(Camera.main.transform);
        GetComponentInChildren<TextMesh>().gameObject.transform.Rotate(0, 180, 0);

    }
}
