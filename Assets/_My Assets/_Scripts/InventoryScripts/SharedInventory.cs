using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharedInventory : Inventory
{

    // Start is called before the first frame update
    void Awake()
    {
        inventorySlots = new Dictionary<object, GameObject>();

        ExpandInventory(24);

        totalItems = 0;

        AddSingleItem(InventoryManagement.Instance.itemList.Consumables[0]);
    }
}
