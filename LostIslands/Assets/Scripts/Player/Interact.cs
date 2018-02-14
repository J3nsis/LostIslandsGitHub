using UnityEngine;

public class Interact : MonoBehaviour {

    FirstPersonPlayerMovement fppm;

    float maxRaycastDistance = 5;

    private void Start()
    {
        fppm = Net_Manager.instance.GetLocalPlayer().GetComponent<FirstPersonPlayerMovement>();
    }

    private void Update()
    {
        if (fppm.Pause)return;

        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, maxRaycastDistance))        
        {
            if (hitInfo.transform.tag == "Water")
            {
                UIManager.instance.NewInfo("drink water (E)");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlayerStats.instance.ps.Trinken += 50;
                }
            }

            if (hitInfo.transform.tag == "Bush")
            {
                UIManager.instance.NewInfo("eat berries (E)");
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PlayerStats.instance.ps.Essen += 15;
                    Destroy(hitInfo.transform.gameObject);
                    UIManager.instance.ClearInfo();
                }
            }

            if (hitInfo.transform.name == "Backpack")
            {
                UIManager.instance.NewInfo("get backpack (E)");
                if (Input.GetKey(KeyCode.E))
                {
                    PlayerStats.instance.ps.CollectedObjectNames.Add(hitInfo.transform.gameObject.name);
                    Chat.instance.NewInfo("Backpack equipped, press (I) to open inventory!");
                    Destroy(hitInfo.transform.gameObject);
                    UIManager.instance.ClearInfo();                  
                }

            }

            if (hitInfo.transform.name == "StartAxe")
            {
                UIManager.instance.NewInfo("get stone axe (E)");
                if (Input.GetKey(KeyCode.E))
                {
                    PlayerStats.instance.ps.CollectedObjectNames.Add(hitInfo.transform.gameObject.name);
                    InventoryItems.instance.AddItembySlugorID("Stone_Axe");
                    Destroy(hitInfo.transform.gameObject.gameObject);
                    UIManager.instance.ClearInfo();                    
                }

            }

            if (hitInfo.transform.name == "StartBox")
            {
                if (PlayerStats.instance.ps.Level >= 5)
                {
                    UIManager.instance.NewInfo("open starter chest (E)");
                    if (Input.GetKey(KeyCode.E))
                    {
                        Destroy(hitInfo.transform.gameObject.gameObject);
                        UIManager.instance.ClearInfo();
                    }
                }
                else
                {
                    UIManager.instance.NewInfo("Reach level 5 to open starter chest!");
                }
            }

            if (hitInfo.transform.tag == "ItemPackage")
            {
                UIManager.instance.NewInfo("get item package (E)");
                if (Input.GetKey(KeyCode.E) && !hitInfo.transform.gameObject.GetComponent<ItemPackage>().looted)
                {
                    if (InventoryItems.instance.Slotfree())
                    {
                        InventoryItems.instance.AddItembySlugorID(hitInfo.transform.gameObject.GetComponent<ItemPackage>().item.Slug,
                            hitInfo.transform.gameObject.GetComponent<ItemPackage>().itemData.amount,
                            hitInfo.transform.gameObject.GetComponent<ItemPackage>().itemData.currentdurability);
                        hitInfo.transform.gameObject.GetComponent<ItemPackage>().looted = true;
                        if (PhotonNetwork.offlineMode)
                        {
                            hitInfo.transform.gameObject.GetComponent<Net_ObjectsSyncHandler>().DestroyMe(0, true);
                        }
                        else
                        {                           
                            hitInfo.transform.gameObject.GetComponent<Net_ObjectsSyncHandler>().Net_DestroyMe(0, true);
                        }
                        UIManager.instance.ClearInfo();
                    }
                    else
                    {
                        Chat.instance.NewWarning("Your inventory is full, cannot add item!");
                    }
                }
            }

            if (!(hitInfo.transform.tag == "Water" || hitInfo.transform.tag == "Bush" || hitInfo.transform.name == "Backpack" || hitInfo.transform.name == "StartAxe" || hitInfo.transform.name == "StartBox" || hitInfo.transform.tag == "ItemPackage"))
            {
                UIManager.instance.ClearInfo();
            }
        }
        else//wenn nichts hittet
        {
            UIManager.instance.ClearInfo();
        }
    }
}
