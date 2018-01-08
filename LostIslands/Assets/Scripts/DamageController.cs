using System.Collections.Generic;
using UnityEngine;

public class  DamageController : MonoBehaviour {


    public int health; //später adden das health auch gespeuchert wird und nicht immer beim neuladen wiede voll ist
    public bool dead;
    GameObject Player;

    public enum Name { Tree, Rock, Wall }; //Der Name hier drin muss auch Tag sein
    Name Object;


    void Start()
    {
        Player = Net_Manager.instance.GetLocalPlayer();

        if (Player == null)
        {
            print("Player null");
        }

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
            if (Object != Name.Tree)//wenn nicht Baum
            {
                Destroy(this.gameObject, 0.25f);//normal zerstören
            }
            else //Baum umfallen etc.
            {
                if (!GetComponent<Rigidbody>())
                {
                    this.gameObject.AddComponent(typeof(Rigidbody));
                }
                
                this.gameObject.GetComponent<Rigidbody>().AddForce(transform.rotation.eulerAngles);
                Destroy(this.gameObject, 3f);
            }
            
        }

    }

    public void DamageMe(int damage)//wird von Axt ausgeführt wenn Axt erkennt das anderes Object dieses Script hat (int damage = Stärke von Axt)
    {
        if (Object == Name.Tree && dead == false) 
        {
            int r = Random.Range(1, 10);
            if (r >= 3)
            {
                int rw = Random.Range(1, 3);
                InventoryItems.instance.AddItembySlug("Wood", rw);
            }
            else
            {
                int rs = Random.Range(2, 4);
                InventoryItems.instance.AddItembySlug("Stick", rs);
            }            
        }
        else if (Object == Name.Rock && dead == false) 
        {
            int rs = Random.Range(1, 3);
            InventoryItems.instance.AddItembySlug("Stone", rs);
        }

        health -= damage;

    }

   
}
