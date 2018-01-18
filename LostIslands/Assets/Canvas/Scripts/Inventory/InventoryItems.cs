using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class InventoryItems : MonoBehaviour {
    
    #region Instance
    public static InventoryItems instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of InventoryItems found!");
            return;
        }
        instance = this;
    }
    #endregion
    public GameObject InventoryPanel;
    [SerializeField] GameObject slotPanel, hotbarSlotPanel;
    public GameObject InventorySlot;
    public GameObject InventoryItem;
    public ItemDatabase database;

    public List<Item> items = new List<Item>(); //alle items als Liste, quasi als backend Liste, vordergrund sind dann die richtigen Items
    public List<GameObject> slots = new List<GameObject>();//alle slots GameObjects

    public List<Item> HotbarItems = new List<Item>(); //alle items als Liste, quasi als backend Liste, vordergrund sind dann die richtigen Items
    public List<GameObject> HotbarSlots = new List<GameObject>();//alle slots GameObjects

    [Serializable]
    public class InventoryItemsSave
    {
        [Serializable]
        public struct Slot
        {
            public int SlotNr;
            public bool inHotbar;
            public ItemData.ThisItemData ItemDataSave;
        }
        public List<Slot> SlotsList = new List<Slot>();
    }

    void Start () 
	{
        database = GetComponent<ItemDatabase>();

        InventoryPanel.SetActive(true);

        for (int i = 0; i < slotPanel.transform.childCount; i++)
        {
            slots.Add(slotPanel.transform.GetChild(i).gameObject);//Liste mit Slots füllen 
            items.Add(new Item());//Liste mit leeren Items füllen
        }

        for (int i = 0; i < hotbarSlotPanel.transform.childCount; i++)
        {
            if (hotbarSlotPanel.transform.GetChild(i).gameObject.transform.name == "Slot" + i)
            {
                hotbarSlotPanel.transform.GetChild(i).gameObject.GetComponent<Slot>().HotbarSlot = true;
                HotbarSlots.Add(hotbarSlotPanel.transform.GetChild(i).gameObject);//Liste mit Slots füllen 
                HotbarItems.Add(new Item());//Liste mit leeren Items füllen
            }          
        }
    }


    public void AddItembySlug(string slug, int amount = 1)//für von außer übersichtlicher als mit IDs
    {
        //hinzufügen das überprüft ob überhaupt da, also den namen den man eigibt
        if (Slotfree()) //nur adden wenn nicht voll
        {
            AddItem(database.GetItemBySlug(slug).ID, amount);
            Chat.instance.NewInfo(amount + " " + database.GetItemBySlug(slug).Name + " added!");
        }
        else
        {
            Chat.instance.NewWarning("Inventory full!");
        }       
    }

    public void RemoveItembySlug(string slug, int amount = 1)//geht nur für Inventory nicht für hotbar
    {
        if (CheckIfItemIsInInventory(database.GetItemBySlug(slug)))
        {
            RemoveItem(database.GetItemBySlug(slug).ID, amount);
        }           
    }

    private void AddItem(int id, int amount = 1)//für neue Items ohne vorhandene data wie z.B. durability
    {
        Item ItemToAdd = database.FetchItemByID(id); 
        if (ItemToAdd.Stackable && CheckIfItemIsInInventory(ItemToAdd))//wenn schon drin und stackable
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)//überprüft, wo das Objekt im Invenatr schon ist und addiert amount
                {
                    ItemData.ThisItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>().data; //data = ItemData.ThisItemDate klasse aus script an Item auf Slot
                    data.amount += amount;
                    break;
                }
            }
        }
        else//wenn nicht drin und oder nicht stackable
        {
            for (int i = 0; i < items.Count; i++)//alle slots durchgehen
            {
                if (items[i].ID == -1)//wenn leer: Item da rein machen, Image und Name ändern
                {
                    items[i] = ItemToAdd;
                    GameObject itemObj = Instantiate(InventoryItem);
                    itemObj.transform.SetParent(slots[i].transform);
                    ItemData itemObjdata = slots[i].transform.GetChild(0).GetComponent<ItemData>();
                    itemObjdata.item = ItemToAdd;//Item übergeben
                    itemObjdata.data.ItemID = ItemToAdd.ID;
                    itemObjdata.data.slot = i; //slot übergeben
                    itemObjdata.data.amount = amount;
                    itemObjdata.newAdded = true;//damit weiß ob das Item neu ist
                    itemObj.transform.SetParent(slots[i].transform);//Item child vom leeren slot machen
                    itemObj.GetComponent<Image>().sprite = ItemToAdd.Sprite;
                    itemObj.transform.localPosition = new Vector2(0, 0);
                    itemObj.name = ItemToAdd.Name;
                    break;
                }
            }
        }
    }

    private void AddItemWithData(int ItemId, ItemData.ThisItemData data)//für schon vorhandenes/aus Load importiertes Item mit ItemData
	{
        Item ItemToAdd = database.FetchItemByID(ItemId);

        items[data.slot] = ItemToAdd;
        GameObject itemObj = Instantiate(InventoryItem);
        ItemData itemObjData = itemObj.GetComponent<ItemData>();
        itemObjData.data = data;//hier ist eigentlich alles wichtige (customdata) drin
        itemObjData.item = ItemToAdd;        

        if (data.inHotbar)
        {
            itemObj.transform.SetParent(HotbarSlots[data.slot].transform);//Item child vom leeren slot machen
        }
        else
        {
            itemObj.transform.SetParent(slots[data.slot].transform);//Item child vom leeren slot machen
        }
        itemObjData.newAdded = false;            
        itemObj.GetComponent<Image>().sprite = ItemToAdd.Sprite;
        itemObj.transform.localPosition = new Vector2(0, 0);
        itemObj.name = ItemToAdd.Name;          
    }



    private void RemoveItem(int id, int amount = 1)//brauchen wir eigentlich nicht, hier nur für Test Taste
    {
        Item ItemToRemove = database.FetchItemByID(id);
        if (ItemToRemove.Stackable && CheckIfItemIsInInventory(ItemToRemove))
        {
            for (int j = 0; j < items.Count; j++)
            {
                if (items[j].ID == id)
                {
                    ItemData itemData = slots[j].transform.GetChild(0).GetComponent<ItemData>();
                    itemData.data.amount -= amount;
                    itemData.transform.GetChild(0).GetComponent<Text>().text = itemData.data.amount.ToString();
                    if (itemData.data.amount == 0)
                    {
                        Destroy(slots[j].transform.GetChild(0).gameObject);
                        items[j] = new Item();
                        break;
                    }
                    if (itemData.data.amount == 1)
                    {
                        slots[j].transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = "";
                        break;
                    }
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID != -1 && items[i].ID == id)
                {
                    Destroy(slots[i].transform.GetChild(0).gameObject);
                    items[i] = new Item();
                    break;
                }
            }               
        } 
    }


    //############################################# Paar nützliche Funktionen 
    bool CheckIfItemIsInInventory(Item item)
    {
        if (items.Contains(item))
        {     
            return true;                      
        }
        return false;
    }

    bool CheckIfItemIsInHotbar(Item item)
    {
        if (HotbarItems.Contains(item))
        {
            return true;
        }
        return false;
    }

    public int GetAmountofItem(int id)
    {
        if (CheckIfItemIsInInventory(ItemDatabase.instance.FetchItemByID(id)))//nur wenn auch drin checken
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ID == id)
                {
                    ItemData itemData = slots[i].transform.GetChild(0).GetComponent<ItemData>(); //data = ItemData script an Item auf Slot
                    return itemData.data.amount;
                }
            }
        }
        else
        {
            return 0;
        }
        throw new InvalidOperationException("Cant get Amount or not in inv!");


    }

    bool Slotfree()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == -1)
            {
                return true;
            }
        }
        return false;
    }

    public ItemData GetItemDataFromID(int id)//wird z.B. in Tooltip für Infos benötigt wie currentDurability
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == id)
            {
                return slots[i].transform.GetChild(0).GetComponent<ItemData>(); 
            }    
        }
        throw new InvalidOperationException("No ItemData found!");
    }

    public Item GetItembySlot(int slotId, bool Hotbar = false)//wird z.B. in Tooltip für Infos benötigt wie currentDurability
    {
        if (!Hotbar)
        {
            if (slots[slotId].transform.childCount == 1)
            {
                return slots[slotId].transform.GetChild(0).GetComponent<ItemData>().item;
            }
            else
            {
                return null;
            }
        }
        else
        {
            if (HotbarSlots[slotId].transform.childCount == 1)
            {
                return HotbarSlots[slotId].transform.GetChild(0).GetComponent<ItemData>().item;
            }
            else
            {
                return null;
            }
        }
        
    }

    public ItemData GetItemDatabySlot(int slotId)//wird z.B. in Tooltip für Infos benötigt wie currentDurability
    {
        return slots[slotId].transform.GetChild(0).GetComponent<ItemData>();
    }

    public int GetSlotWhereItemisInInventory(int id)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].gameObject.transform.GetChild(0).GetComponent<ItemData>().item.ID == id)
            {
                return i;
            }
        }
        throw new InvalidOperationException("No Slot with this ID of Item found!");
    }

    //############################################# Save/Load

    public InventoryItemsSave SaveInventory()//gibt Class mit allen Slot für SaveLoadManager wieder
    {
        InventoryItemsSave ItemsSave = new InventoryItemsSave();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0)//wenn Item in Slot drin
            {
                InventoryItemsSave.Slot slotData = new InventoryItemsSave.Slot
                {
                    SlotNr = i,
                    ItemDataSave = slots[i].GetComponentInChildren<ItemData>().data,
                    inHotbar = false
                };
                ItemsSave.SlotsList.Add(slotData);
            }          
        }
        for (int i = 0; i < HotbarSlots.Count; i++)//save Hotbar
        {
            if (HotbarSlots[i].transform.childCount > 0)//wenn Item in Slot drin
            {
                InventoryItemsSave.Slot slotData = new InventoryItemsSave.Slot {
                    SlotNr = i,
                    ItemDataSave = HotbarSlots[i].GetComponentInChildren<ItemData>().data,
                    inHotbar = true
                };
                ItemsSave.SlotsList.Add(slotData);
            }
        }
        return ItemsSave;       
    }



    public void LoadInventory(InventoryItemsSave ItemsSave)
    {
        ClearInventory();
        if (ItemsSave == null)
        {
            Debug.LogWarning("Cant load Items because nothing in json file");
            return;
        }
        foreach (InventoryItemsSave.Slot slotItem in ItemsSave.SlotsList)
        {
            AddItemWithData(slotItem.ItemDataSave.ItemID, slotItem.ItemDataSave);
        }
    }

    void ClearInventory()//alle Slots etc. löschen
    {
        items.Clear();
        for (int i = 0; i < slots.Count; i++)
        {
            items.Add(new Item());//Liste mit leeren Items füllen
        }
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject parent = slots[i];
            foreach (Transform child in parent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        HotbarItems.Clear();
        for (int i = 0; i < HotbarSlots.Count; i++)
        {
            HotbarItems.Add(new Item());//Liste mit leeren Items füllen
        }
        for (int i = 0; i < HotbarSlots.Count; i++)
        {
            GameObject parent = HotbarSlots[i];
            foreach (Transform child in parent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            HandManager.instance.DisableAll();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseToolFromHotbar(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseToolFromHotbar(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseToolFromHotbar(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseToolFromHotbar(3);
        }
    }

    private void UseToolFromHotbar(int slot)
    {
        if (HotbarSlots[slot].GetComponentInChildren<ItemData>() != null)
        {
            HotbarSlots[slot].GetComponentInChildren<ItemData>().OnClick();
        }
        else
        {
            HandManager.instance.DisableAll();
        }
    }






}
