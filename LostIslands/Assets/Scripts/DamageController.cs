using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Net_ObjectsSyncHandler))]
public class  DamageController : MonoBehaviour {


    public int health; //später adden das health auch gespeuchert wird und nicht immer beim neuladen wieder voll ist
    public bool dead;

    public enum Name { Tree, Rock, Wall }; //Der Name hier drin muss auch Tag sein
    Name Object;


    void Start()
    {

        Object = (Name)System.Enum.Parse(typeof(Name), this.transform.tag); //an Hand von Tag wird Enum ausgewählt

        if (Object == Name.Tree)
        {
            health = 25;
        }
        else if (Object == Name.Rock)
        {
            health = 20;
        }
        else if (Object == Name.Wall)
        {
            health = 40;
        }
    }


    void Update()
    {
        if (health <= 0)
        {
            dead = true;

            if (!GetComponent<Net_ObjectsSyncHandler>())
            {
                gameObject.AddComponent(typeof(Net_ObjectsSyncHandler));                   
            }
            GetComponent<Net_ObjectsSyncHandler>().Net_DestroyMe(0, true);
        }
    }

    public void DamageMe(int damage)//wird von Tool ausgeführt wenn Tool erkennt das anderes Object dieses Script hat (int damage = Stärke von Tool)
    {
        if (Object == Name.Tree && dead == false) 
        {
            int r = Random.Range(1, 10);
            if (r >= 3)
            {
                int rw = Random.Range(1, 3);
                InventoryItems.instance.AddItembySlugorID("Wood", rw);
            }
            else
            {
                int rs = Random.Range(2, 4);
                InventoryItems.instance.AddItembySlugorID("Stick", rs);
            }            
        }
        else if (Object == Name.Rock && dead == false) 
        {
            int rs = Random.Range(1, 3);
            InventoryItems.instance.AddItembySlugorID("Stone", rs);
        }

        health -= damage;

    }

   
}
