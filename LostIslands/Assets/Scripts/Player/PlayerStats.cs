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

    public Text GesundheitText;
    public Text AusdauerText;
    public Text EssenText;
    public Text TrinkenText;

    public GameObject GesundheitProgress;
    public GameObject AusdauerProgress;
    public GameObject EssenProgress;
    public GameObject TrinkenProgress;

    public GameObject Blood;

    [Serializable]
    public class PlayerStatsSave
    {
        public float Gesundheit = 50; 
        public float Ausdauer = 50;
        public float Essen = 50;
        public float Trinken = 50;
        public int Level = 1;

        public bool hasBackpack;
    }

    public bool isRunning;//wird von FPMovement script geändert

    public PlayerStatsSave ps; //wird von SaveLoadManager gespechert/geladen



    void Start ()
    {
        Blood.SetActive(false);
    }

    public void OnSwing()//wird von ToolController bei Swing ausgeführt
    {
        ps.Ausdauer -= 10f; //ca. -10
    }


    void FixedUpdate ()
    {
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
            ps.Gesundheit -= 0.02f;
        }

        if (ps.Gesundheit <= 10)
        {
            Blood.SetActive(true);
            
        }
        else
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


}
