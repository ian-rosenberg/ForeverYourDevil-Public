using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

#region Item JSON Deserialization

[System.Serializable]
public class Items
{
    public List<ItemBase> Ingredients;
    public List<ItemBase> Consumables;
    public List<ItemBase> Concoctions;
    public List<ItemBase> Equipment;
    public List<ItemBase> Abilities;
}

[System.Serializable]
public class ItemBase
{
    public string ID;
    public string Icon;
    public string Name;
    public bool Consumable;
    public int Cooldown;
    public int Duration;
    public string EffectDescription;
    public string Description;
    public string SFX;
    public StatBonus Bonuses;
}

[System.Serializable]
public class StatBonus
{
    public int HP;
    public int Stamina;
    public int Tolerance;
    public int Attack;
}
#endregion

public class InventoryManagement : MonoBehaviour
{
    #region Inventory Management Instance
    private static InventoryManagement instance;
    public static InventoryManagement Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<InventoryManagement>();
            return instance;
        }
    }
    #endregion

    #region Item Master List
    public Items itemList;
    #endregion

    private List<GameObject> inventoryObjs;//The inventories in use;
    private GameObject sharedInventory;

    public GameObject blurShader;
    public GameObject sharedInventoryPrefab;
    public GameObject personalInventoryPrefab;


    public int numInventories;


    // Start is called before the first frame update
    void Start()
    {
        inventoryObjs = new List<GameObject>();
        
        //for(int i = 0; i < numInventories; i++)
        //{
            //AddInventory();
        //}

        CreateItemDatabase("itemList");

        CreateSharedInventory();

        //SetInventoriesInactive();
    }

    public void AddPorridge()
    {
        SharedInventory sI = sharedInventory.GetComponentInChildren<SharedInventory>(); 
        sI.AddSingleItem(itemList.Consumables[0]);
    }

    private void CreateItemDatabase(string path)
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        itemList = new Items();
        
        itemList.Abilities = new List<ItemBase>();
        itemList.Concoctions = new List<ItemBase>();
        itemList.Consumables = new List<ItemBase>();
        itemList.Equipment = new List<ItemBase>();
        itemList.Ingredients = new List<ItemBase>();

        itemList = JsonConvert.DeserializeObject<Items>(json.text);
    }

    private void AddInventory()
    {
        GameObject inventoryClone = Instantiate(personalInventoryPrefab, transform);
        
        inventoryObjs.Add(inventoryClone);
    }

    private void CreateSharedInventory()
    {
        sharedInventory = Instantiate(sharedInventoryPrefab, transform);
    }

    public void SetInventoriesInactive()
    {
        sharedInventory.SetActive(false);

        foreach(GameObject inventory in inventoryObjs)
        {
            inventory.SetActive(false);
        }
    }

    public void SetSharedInventoryActive()
    {
        sharedInventory.SetActive(true);
    }
}
