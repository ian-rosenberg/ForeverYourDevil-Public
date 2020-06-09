using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;
using System;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Base Class")]
    private int slotIndices;

    public Dictionary<object, GameObject> inventorySlots;
    public int selectedIndex = 0;
    public int totalSlots;
    public int totalItems;

    public GameObject elementOwnerPrefab;//The grid space in which an item can reside
    public GameObject itemDrop;

    #region Player Actions
    private PlayerControls pControls;

    private void OnEnable()
    {
        pControls = new PlayerControls();

        pControls.UI.Cancel.performed += DropItem;

        pControls.UI.Cancel.Enable();
    }

    private void OnDisable()
    {
        pControls.UI.Cancel.performed -= DropItem;

        pControls.UI.Cancel.Disable();
    }
    #endregion

    public void AddSlot()
    {
        if (inventorySlots == null)
        {
            inventorySlots = new Dictionary<object, GameObject>();

            totalItems = 0;
        }

        GameObject slotClone = Instantiate(elementOwnerPrefab, transform);

        inventorySlots.Add(slotIndices++, slotClone);
        totalSlots++;
    }

    public void AddSlotWithItem(ItemBase item)
    {
        GameObject slotClone = Instantiate(elementOwnerPrefab, transform);
        uint currentIndex = (uint)inventorySlots.Count;

        inventorySlots.Add(currentIndex, slotClone);

        slotClone.GetComponent<InventorySlot>().child = item;

        InventorySlot slot = slotClone.GetComponent<InventorySlot>();

        slot.quantity++;

        totalItems++;

        slot.SetQuantityText();
    }

    public void AddSingleItem(ItemBase item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            GameObject go = inventorySlots[i];
            InventorySlot slot = go.GetComponent<InventorySlot>();
            ItemBase child = slot.child;

            if (child == null)
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

                totalItems++;

                slot.SetQuantityText();

                slot.inUse = true;

                return;
            }
        }

        AddSlotWithItem(item);
    }

    public void ExpandInventory(int slots)
    {
        for (int i = 0; i < slots; i++)
        {
            AddSlot();
        }

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
        item.transform.position = new Vector3(pT.position.x, pT.position.y + item.GetComponent<SpriteRenderer>().bounds.size.y / 2, pT.position.z);

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
}
