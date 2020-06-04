using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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

    public string StatBonusesToString()
    {
        string bonuses = "HP: " + Bonuses.HP +
            "\t Stamina: " + Bonuses.Stamina + "\n" +
            "Attack: " + Bonuses.Attack +
            "\t Tolerance: " + Bonuses.Tolerance;

        return bonuses;
    }
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

    private Dictionary<object, Sprite> itemImages;
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

        itemImages = new Dictionary<object, Sprite>();

        itemList = new Items();

        itemList.Abilities = new List<ItemBase>();
        itemList.Concoctions = new List<ItemBase>();
        itemList.Consumables = new List<ItemBase>();
        itemList.Equipment = new List<ItemBase>();
        itemList.Ingredients = new List<ItemBase>();

        itemList = JsonConvert.DeserializeObject<Items>(json.text);

        if (itemList.Abilities.Count > 0)
        {
            foreach (ItemBase item in itemList.Abilities)
            {
                itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
            } 
        }

        if (itemList.Concoctions.Count > 0)
        {
            foreach (ItemBase item in itemList.Concoctions)
            {
                itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
            }
        }

        if (itemList.Consumables.Count > 0)
        {
            foreach (ItemBase item in itemList.Consumables)
            {
                itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
            }
        }

        if (itemList.Equipment.Count > 0)
        {
            foreach (ItemBase item in itemList.Equipment)
            {
                itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
            }
        }

        if (itemList.Ingredients.Count > 0)
        {
            foreach (ItemBase item in itemList.Ingredients)
            {
                itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
            }
        }
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

        /*foreach(GameObject inventory in inventoryObjs)
        {
            inventory.SetActive(false);
        }*/
    }

    public void SetSharedInventoryActive(bool flag)
    {
        sharedInventory.SetActive(flag);
    }

    public Sprite GetItemImage(ItemBase item)
    {
        return itemImages[item];
    }
}
