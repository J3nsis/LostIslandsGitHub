using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    #region Instance
    public static PlayerStats instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerStats found!");
            return;
        }
        instance = this;
    }
    #endregion

    //###### Variablen ######
    public Text GesundheitText;
    public Text AusdauerText;
    public Text EssenText;
    public Text TrinkenText;

    public GameObject GesundheitProgress;
    public GameObject AusdauerProgress;
    public GameObject EssenProgress;
    public GameObject TrinkenProgress;

    public GameObject Blood;
    public GameObject DeadMessage;

    [SerializeField]
    Transform PlayerSpawn;

    [Serializable]
    public class PlayerStatsSave
    {
        public float Gesundheit = 50; 
        public float Ausdauer = 50;
        public float Essen = 50;
        public float Trinken = 50;
        public int Level = 1;

        public bool hasBackpack;

        public Vector3 position;//wird in SaveLoadManager beim laden und speichern auf Spieler übertragen!
    }

    public bool isRunning;//wird von FPMovement script geändert
    public PlayerStatsSave ps; //wird von SaveLoadManager gespechert/geladen

    int lastLevel = 1;//um zu checken ob Level sich geändert hat!

    //###### Funktionen ######

    void Start ()
    {
        Blood.SetActive(false);
        DeadMessage.SetActive(false);
        lastLevel = ps.Level;
    }

    public void OnSwing()//wird von ToolController bei Swing ausgeführt
    {
        ps.Ausdauer -= 10f; //ca. -10
    }


    void FixedUpdate ()
    {
        if (ps.Level != lastLevel)
        {
            Chat.instance.NewInfo("Congratulations! You are now Level " + ps.Level + "!");
            lastLevel = ps.Level;
        }

        if (ps.Gesundheit < 1)
        {
            KillPlayer();
            return;
        }

        if (isRunning) //Sprinten = ausdauer runter, nicht sprinten = ausdauer hoch
        {
            ps.Ausdauer -= 0.1f;     
        }
        else //Ausdauer geht immer hoch wenn nicht rennt 
        {
            ps.Ausdauer += 0.1f;
        }               

        if (ps.Essen >= 65 && ps.Trinken >= 65) //Essen/Trinken hoch = Gesundheit hoch
        {
            ps.Gesundheit += 0.075f;
              
        }

        if (ps.Essen <= 1 || ps.Trinken <= 1) //Essen/Trinken niedrig =  Gesundheit runter
        {
            ps.Gesundheit -= 0.025f;
        }

        if (ps.Gesundheit <= 10)
        {
            Blood.SetActive(true);            
        }
        if (ps.Gesundheit > 10)
        {
            Blood.SetActive(false);
        }

        ps.Trinken -= 0.005f; //Trinken und Essen geht immer runter
        ps.Essen -= 0.002f;

        ps.Gesundheit = Mathf.Clamp(ps.Gesundheit, 0f, 100f); //geht nicht richtig
        ps.Ausdauer = Mathf.Clamp(ps.Ausdauer, 0, 100);
        ps.Essen = Mathf.Clamp(ps.Essen, 0, 100);
        ps.Trinken = Mathf.Clamp(ps.Trinken, 0, 100);

        
        GesundheitProgress.GetComponent<Image>().fillAmount = ps.Gesundheit / 100;
        AusdauerProgress.GetComponent<Image>().fillAmount = ps.Ausdauer / 100;
        EssenProgress.GetComponent<Image>().fillAmount = ps.Essen / 100;
        TrinkenProgress.GetComponent<Image>().fillAmount = ps.Trinken / 100;

        GesundheitText.text = Mathf.Round(ps.Gesundheit) + "%";
        AusdauerText.text = Mathf.Round(ps.Ausdauer) + "%";
        EssenText.text = Mathf.Round(ps.Essen) + "%";
        TrinkenText.text = Mathf.Round(ps.Trinken) + "%";
        

    }

    public void Heal()
    {
        ps.Gesundheit = 100;
        ps.Ausdauer = 100;
        ps.Essen = 100;
        ps.Trinken = 100;
    }

    public void KillPlayer()
    {        
        Net_Manager.instance.GetLocalPlayer().transform.position = PlayerSpawn.position;
        ps.Gesundheit = 50;
        ps.Ausdauer = 50;
        ps.Essen = 50;
        ps.Trinken = 50;
        StartCoroutine(ShowDeadMessage(2.5f));       
    }

    public void SetLevel(int level)
    {
        ps.Level = level;
    }


    IEnumerator ShowDeadMessage(float time)
    {
        DeadMessage.SetActive(true);
        yield return new WaitForSeconds(time);
        DeadMessage.SetActive(false);
    }
}
