using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
=======
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using System;
>>>>>>> Rebuilding inventory

public class Inventory : MonoBehaviour
{
    [Header("Inventory Base Class")]
    private int slotIndices;
<<<<<<< HEAD

    public Dictionary<object, GameObject> inventorySlots;
    public int totalItems;

    public GameObject elementOwnerPrefab;//The grid space in which an item can reside


    // Start is called before the first frame update
=======
    private InventorySlot selectedItem;


    public Dictionary<object, GameObject> inventorySlots;
    public int selectedIndex = 0;
    public int totalSlots;
    public int totalItems;

    public GameObject elementOwnerPrefab;//The grid space in which an item can reside
    public GameObject itemDrop;


    public bool canSelect = true;
>>>>>>> Rebuilding inventory

    public void AddSlot()
    {
        if (inventorySlots == null)
        {
            inventorySlots = new Dictionary<object, GameObject>();

            totalItems = 0;
        }

        GameObject slotClone = Instantiate(elementOwnerPrefab, transform);

        inventorySlots.Add(slotIndices++, slotClone);
<<<<<<< HEAD
=======
        totalSlots++;
>>>>>>> Rebuilding inventory
    }

    public void AddSlotWithItem(ItemBase item)
    {
        GameObject slotClone = Instantiate(elementOwnerPrefab, transform);
        uint currentIndex = (uint)inventorySlots.Count;

        inventorySlots.Add(currentIndex, slotClone);

        slotClone.GetComponent<InventorySlot>().child = item;

        InventorySlot slot = slotClone.GetComponent<InventorySlot>();

        slot.quantity++;

<<<<<<< HEAD
=======
        totalItems++;

>>>>>>> Rebuilding inventory
        slot.SetQuantityText();
    }

    public void AddSingleItem(ItemBase item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            GameObject go = inventorySlots[i];
            InventorySlot slot = go.GetComponent<InventorySlot>();
            ItemBase child = slot.child;

<<<<<<< HEAD
            if (child == null || !slot.inUse)
=======
            if (child == null)
>>>>>>> Rebuilding inventory
                continue;

            if (child.Name == item.Name)
            {
                slot.quantity++;

                slot.SetQuantityText();

                totalItems++;
                return;
            }
        }

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            GameObject go = inventorySlots[i];
            InventorySlot slot = go.GetComponent<InventorySlot>();

            if (!slot.inUse)
            {
                slot.child = item;
                slot.img.sprite = Resources.Load<Sprite>(item.Icon);

                slot.quantity++;
<<<<<<< HEAD
=======

>>>>>>> Rebuilding inventory
                totalItems++;

                slot.SetQuantityText();

                slot.inUse = true;

                return;
            }
        }

        AddSlotWithItem(item);
<<<<<<< HEAD

        totalItems++;
=======
>>>>>>> Rebuilding inventory
    }

    public void ExpandInventory(int slots)
    {
        for (int i = 0; i < slots; i++)
        {
            AddSlot();
        }
<<<<<<< HEAD
    }

    public void DropItem()
    {

=======

        foreach (KeyValuePair<object, GameObject> pair in inventorySlots)
        {
            pair.Value.GetComponent<InventorySlot>().ownerInventory = this.gameObject;
        }
    }

    public InventorySlot GetLastSelected()
    {
        foreach (KeyValuePair<object, GameObject> pair in inventorySlots)
        {
            InventorySlot slot = pair.Value.GetComponent<InventorySlot>();

            if (slot.Selected())
                return slot;
        }

        return null;
    }
    
    public InventorySlot GetSelected()
    {
        return inventorySlots[selectedIndex].GetComponent<InventorySlot>();
    }

    public void DropItem(InputAction.CallbackContext context)
    {
        InventorySlot curSlot = inventorySlots[selectedIndex].GetComponent<InventorySlot>();
        Transform pT = gameManager.Instance.player.transform;

        if (curSlot == null)
            return;

        if (curSlot.child == null)
            return;

        if (curSlot.quantity < 1)
            return;

        curSlot.quantity--;


        GameObject item = Instantiate(itemDrop, pT);

        item.transform.SetParent(gameManager.Instance.gameObject.transform);
        item.GetComponent<ItemDropped>().SetItem(curSlot.child);
        item.transform.position = new Vector3(pT.position.x, pT.position.y + item.GetComponent<SpriteRenderer>().bounds.size.y / 4, pT.position.z);

        if (curSlot.quantity == 0)
        {
            curSlot.img.sprite = null;

            curSlot.EmptySlot();
        }
        else
            curSlot.SetQuantityText();
    }

    public void SetIndex(int val)
    {
        selectedIndex += val;
    }

    public void DisableSelection()
    {
        canSelect = false;
    }

    public void SelectItemByIndex(int index)
    {
        if (index <= inventorySlots.Count && index >= 0)
            selectedIndex = 0;
        else
            selectedIndex = index;

        selectedItem = inventorySlots[index].GetComponent<InventorySlot>();
>>>>>>> Rebuilding inventory
    }
}
