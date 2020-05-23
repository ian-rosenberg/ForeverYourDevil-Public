using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Base Class")]
    private Dictionary<object, GameObject> inventorySlots;
    private int totalItems;

    public Dictionary<int, GameObject> items;

    public GameObject elementOwnerPrefab;//The grid space in which an item can reside


    // Start is called before the first frame update

    public void AddSlot()
    {
        if(inventorySlots == null)
        {
            inventorySlots = new Dictionary<object, GameObject>();

            totalItems = 0;
        }
        
        GameObject slotClone = Instantiate(elementOwnerPrefab, transform);
        uint currentIndex = (uint)inventorySlots.Count;

        inventorySlots.Add(currentIndex, slotClone);
    }
    public void AddSlotWithItem(GameObject item)
    {
        GameObject slotClone = Instantiate(elementOwnerPrefab, transform);
        uint currentIndex = (uint)inventorySlots.Count;

        inventorySlots.Add(currentIndex, slotClone);

        slotClone.GetComponent<InventorySlot>().child = item;
    }

    public void AddSingleItem(GameObject item)
    {
        if (inventorySlots.ContainsValue(item))
        {
            item.GetComponent<InventorySlot>().quantity++;

            return;
        }

        for(int i = 0; i < inventorySlots.Count; i++)
        {
            GameObject child = inventorySlots[i].GetComponent<InventorySlot>().child;

            if (child == null)
            {
                inventorySlots[i].GetComponent<InventorySlot>().child = item;
                break;
            }
        }

        AddSlotWithItem(item);

        totalItems++;
    }

    public void ExpandInventory(int slots)
    {
        for (int i = 0; i < slots; i++)
        {
            AddSlot();
        }
    }

    public void DropItem()
    {

    }
}
