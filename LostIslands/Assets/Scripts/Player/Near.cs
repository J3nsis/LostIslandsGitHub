using UnityEngine;

public class Near : MonoBehaviour {

    

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Water")
        {
            UIManager.instance.NewInfo("drink water (E)");
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerStats.instance.ps.Trinken += 50;
            }
        }

        if (other.transform.tag == "Bush")
        {
            UIManager.instance.NewInfo("eat berries (E)");
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerStats.instance.ps.Essen += 15;
                Destroy(other.gameObject);
                UIManager.instance.ClearInfo();
            }
        }

        if (other.transform.name == "Backpack")
        {
            UIManager.instance.NewInfo("get backpack (E)");
            if (Input.GetKey(KeyCode.E))
            {
                Chat.instance.NewInfo("Backpack equipped, press (I) to open inventory!");
                Destroy(other.gameObject);
                UIManager.instance.ClearInfo();
                PlayerStats.instance.ps.hasBackpack = true;
            }

        }

        if (other.transform.name == "StartAxe")
        {
            UIManager.instance.NewInfo("get starter axe (E)");
            if (Input.GetKey(KeyCode.E))
            {
                InventoryItems.instance.AddItembySlug("Stone_Axe");
                Destroy(other.gameObject);
                UIManager.instance.ClearInfo();
            }

        }

        if (other.transform.name == "StartBox")
        {
            if (PlayerStats.instance.ps.Level >= 5)
            {
                UIManager.instance.NewInfo("open starter chest (E)");
                if (Input.GetKey(KeyCode.E))
                {
                    Destroy(other.gameObject);
                    UIManager.instance.ClearInfo();
                }
            }
            else
            {
                UIManager.instance.NewInfo("Reach level 5 to open starter chest!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Water" || other.transform.tag == "Bush" || other.transform.name == "Backpack" || other.transform.name == "StartAxe" || other.transform.name == "StartBox")
        {
            UIManager.instance.ClearInfo();
        }

    }
}
