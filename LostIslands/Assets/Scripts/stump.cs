using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stump : MonoBehaviour {

    int stumpHealth;
    GameObject Player;


    void Start()
    {
        stumpHealth = 4;
        Player = GameObject.Find("Player");
    }

    void Update()
    {
        if (stumpHealth <= 0)
        {
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Axe" && PlayerPrefs.GetInt("Swing") == 1)
        {
            stumpHealth -= 1;
        }
    }
}
