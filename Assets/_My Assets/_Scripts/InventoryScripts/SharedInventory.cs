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
        AddSingleItem(InventoryManagement.Instance.itemList.Consumables[0]);
        AddSingleItem(InventoryManagement.Instance.itemList.Consumables[1]);

        GridLayoutGroup glg = GetComponent<GridLayoutGroup>();

        GetComponent<Inventory>().rowSpacing = glg.cellSize.y + glg.spacing.y;
        GetComponent<Inventory>().rTransform = GetComponent<RectTransform>();
    }

    public void CloseInventory()
    {
        InventoryManagement.Instance.SetInventoriesInactive();

        gameManager.Instance.SetCanPause(true);

        gameManager.Instance.pauseMenu.SetActive(true);
    }

}
