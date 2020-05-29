using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Base Class")]
    private int slotIndices;

    public Dictionary<object, GameObject> inventorySlots;
    public int totalItems;

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

        inventorySlots.Add(slotIndices++, slotClone);
    }

    public void AddSlotWithItem(ItemBase item)
    {
        GameObject slotClone = Instantiate(elementOwnerPrefab, transform);
        uint currentIndex = (uint)inventorySlots.Count;

        inventorySlots.Add(currentIndex, slotClone);

        slotClone.GetComponent<InventorySlot>().child = item;
    }

    public void AddSingleItem(ItemBase item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            ItemBase child = inventorySlots[i].GetComponent<InventorySlot>().child;

            if (child == item)
            {
                inventorySlots[i].GetComponent<InventorySlot>().quantity++;
                totalItems++;
                return;
            }
        }

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            GameObject go = inventorySlots[i];
            InventorySlot slot = go.GetComponent<InventorySlot>();
            
            if (!inventorySlots[i].GetComponent<InventorySlot>().inUse)
            {
                slot.child = item;
                slot.img.sprite = Resources.Load<Sprite>(item.Icon);
                slot.inUse = true;

                return;
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
