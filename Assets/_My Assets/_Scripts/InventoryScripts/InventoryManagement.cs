using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Newtonsoft.Json;
using System;
<<<<<<< HEAD
=======
using UnityEngine.InputSystem;
>>>>>>> Rebuilding inventory

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

<<<<<<< HEAD
    private List<GameObject> inventoryObjs;//The inventories in use;
    private GameObject sharedInventory;
=======
    public List<GameObject> inventoryObjs;//The inventories in use, may have to change to dict to support the different characters
    public GameObject sharedInventory;
>>>>>>> Rebuilding inventory

    public GameObject blurShader;
    public GameObject sharedInventoryPrefab;
    public GameObject personalInventoryPrefab;

<<<<<<< HEAD

    public int numInventories;

=======
    public GameObject tooltipMenu;

    public int numInventories;

    private Inventory currentInventory;

    private bool secondFire = false;


    #region Player Actions
    public PlayerControls pControls;

    private void OnEnable()
    {
        pControls = new PlayerControls();

        pControls.UI.Navigate.performed += HandleUIKeypress;
        pControls.UI.Interact.performed += AcceptSelection;

        pControls.UI.Interact.Enable();
        pControls.UI.Navigate.Enable();
    }

    private void OnDisable()
    {
        pControls.UI.Navigate.performed -= HandleUIKeypress;
        pControls.UI.Interact.performed -= AcceptSelection;

        pControls.UI.Interact.Disable();
        pControls.UI.Navigate.Disable();
    }
    #endregion
>>>>>>> Rebuilding inventory

    // Start is called before the first frame update
    void Start()
    {
        inventoryObjs = new List<GameObject>();
<<<<<<< HEAD
        
        //for(int i = 0; i < numInventories; i++)
        //{
            //AddInventory();
=======

        //for(int i = 0; i < numInventories; i++)
        //{
        //AddInventory();
>>>>>>> Rebuilding inventory
        //}

        CreateItemDatabase("itemList");

        CreateSharedInventory();

        SetInventoriesInactive();
    }

<<<<<<< HEAD
    public void AddPorridge()
    {
        SharedInventory sI = sharedInventory.GetComponentInChildren<SharedInventory>(); 
=======
    public void HandleUIKeypress(InputAction.CallbackContext context)
    {
        if (secondFire)
        {
            secondFire = false;
            return;
        }

        if (gameManager.Instance.gameState != gameManager.STATE.PAUSED)
            return;

        Inventory inv = currentInventory.GetComponentInChildren<Inventory>();

        int oldIndex = inv.selectedIndex;

        Vector2 movement = context.ReadValue<Vector2>();

        if (movement.x == 1)
        {
            if (inv.selectedIndex < inv.totalSlots - 1)
                inv.SetIndex(1);
        }
        else if (movement.x == -1)
        {
            if (inv.selectedIndex > 0)
                inv.SetIndex(-1);
        }
        else if (movement.y == -1)
        {
            if (inv.selectedIndex + 4 < inv.totalSlots)
                inv.SetIndex(4);
        }
        else if (movement.y == 1)
        {
            if (inv.selectedIndex - 4 >= 0)
                inv.SetIndex(-4);
        }

        if (inv.inventorySlots[inv.selectedIndex] != null)
        {
            GameObject newSlotObj = inv.inventorySlots[inv.selectedIndex];
            GameObject oldSlotObj = inv.inventorySlots[oldIndex];

            InventorySlot slot = newSlotObj.GetComponent<InventorySlot>();
            InventorySlot oldSlot = oldSlotObj.GetComponent<InventorySlot>();

            if (oldSlot.Selected())
            {
                oldSlot.UnSelect();
            }

            slot.Select();
        }

        secondFire = true;
    }

    public void AddPorridge()
    {
        SharedInventory sI = sharedInventory.GetComponentInChildren<SharedInventory>();
>>>>>>> Rebuilding inventory
        sI.AddSingleItem(itemList.Consumables[0]);
    }

    private void CreateItemDatabase(string path)
    {
        TextAsset json = Resources.Load<TextAsset>(path);

        itemImages = new Dictionary<object, Sprite>();
<<<<<<< HEAD
=======
        itemImages = new Dictionary<object, Sprite>();
>>>>>>> Rebuilding inventory

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
<<<<<<< HEAD
                itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
            } 
=======

                if (item.Name != "" && item.Name != null)
                    itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
            }
>>>>>>> Rebuilding inventory
        }

        if (itemList.Concoctions.Count > 0)
        {
            foreach (ItemBase item in itemList.Concoctions)
            {
<<<<<<< HEAD
                itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
=======
                if (item.Name != "" && item.Name != null)
                    itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
>>>>>>> Rebuilding inventory
            }
        }

        if (itemList.Consumables.Count > 0)
        {
            foreach (ItemBase item in itemList.Consumables)
            {
<<<<<<< HEAD
                itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
=======
                if (item.Name != "" && item.Name != null)
                    itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
>>>>>>> Rebuilding inventory
            }
        }

        if (itemList.Equipment.Count > 0)
        {
            foreach (ItemBase item in itemList.Equipment)
            {
<<<<<<< HEAD
                itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
=======
                if (item.Name != "" && item.Name != null)
                    itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
>>>>>>> Rebuilding inventory
            }
        }

        if (itemList.Ingredients.Count > 0)
        {
            foreach (ItemBase item in itemList.Ingredients)
            {
<<<<<<< HEAD
                itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
=======
                if (item.Name != "" && item.Name != null)
                    itemImages.Add(item, Resources.Load<Sprite>(item.Icon));
>>>>>>> Rebuilding inventory
            }
        }
    }

    private void AddInventory()
    {
        GameObject inventoryClone = Instantiate(personalInventoryPrefab, transform);
<<<<<<< HEAD
        
=======

>>>>>>> Rebuilding inventory
        inventoryObjs.Add(inventoryClone);
    }

    private void CreateSharedInventory()
    {
        sharedInventory = Instantiate(sharedInventoryPrefab, transform);
<<<<<<< HEAD
=======

        currentInventory = sharedInventory.GetComponentInChildren<Inventory>();

        currentInventory.SelectItemByIndex(0);
>>>>>>> Rebuilding inventory
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
<<<<<<< HEAD
=======

    public void AcceptSelection(InputAction.CallbackContext obj)
    {

        Inventory i = currentInventory.GetComponent<Inventory>();
        InventorySlot selected = i.GetSelected();

        if (!selected.Selected())
            return;

        tooltipMenu.SetActive(true);

        DisableInventoryInput();

        tooltipMenu.transform.SetAsLastSibling();

        currentInventory.GetComponentInChildren<Inventory>().DisableSelection();
    }

    public Inventory GetCurrentInventory()
    {
        return currentInventory;
    }

    public void CloseTooltip()
    {
        tooltipMenu.SetActive(false);
    }

    public void ChangedStateTo(gameManager.STATE newState)
    {
        switch (newState)
        {
            case gameManager.STATE.START:
                Debug.LogError("Cannot switch GM State to START. This should not happen.");
                break;
            case gameManager.STATE.TRAVELING:
                DisableInventoryInput();
                break;
            case gameManager.STATE.COMBAT:
                //open personal inventory
                break;
            case gameManager.STATE.PAUSED:
                EnableInventoryInput();
                break;
            case gameManager.STATE.TALKING:
                DisableInventoryInput();
                break;
        }
    }

    public void EnableInventoryInput()
    {
        pControls.UI.Navigate.performed += HandleUIKeypress;
        pControls.UI.Interact.performed += AcceptSelection;
    }

    public void DisableInventoryInput()
    {
        pControls.UI.Navigate.performed -= HandleUIKeypress;
        pControls.UI.Interact.performed -= AcceptSelection;
    }
>>>>>>> Rebuilding inventory
}
