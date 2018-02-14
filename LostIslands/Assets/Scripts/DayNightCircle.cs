using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DayNightCircle : MonoBehaviour {

    #region Instance
    public static DayNightCircle instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of DayNight... found!");
            return;
        }
        instance = this;
    }
    #endregion

    public GameObject TimeText;
    public GameObject DaysText;

    float lastChange = 0.0f;

    int minutes = 00;
    int hour = 12;   
    string minutesnew;
    public int Day = 1; //wird über SaveLoad gespeichert/geladen

    void Update()
    {
        if (Time.time - lastChange > 5 * Time.deltaTime) //eine Sekunde == 2.4 minuten in game
        {
            minutes++;
            if (minutes == 60)
            {
                minutes = 0;
                hour++;
                if (hour == 24)
                {
                    hour = 0;
                    Day += 1;
                }              
            }
            lastChange = Time.time;
        }

        if(minutes <= 9)
        {
            minutesnew = "0" + minutes.ToString();
        }
        else
        {
            minutesnew = minutes.ToString();
        }
    }


	
	
	void FixedUpdate ()
    {
        TimeText.GetComponent<Text>().text = hour.ToString() + ":" + minutesnew;
        DaysText.GetComponent<Text>().text = "Day: " + Day; 
    }
}



