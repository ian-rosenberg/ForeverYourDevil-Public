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

public enum VisiblePosition{
    Top,
    Middle, 
    Bottom
}

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

    [Header("Inventory Scrolling")]
    private int numRows;
    private int numCols;
    
    public int[] visibleRows;

    public float rowSpacing;
    

    public VisiblePosition vPos;

    public RectTransform rTransform;

    public int selectedIndex = 0;
    public int oldRow = 0;
    public int currentRow = 0;
    
    public Dictionary<object, GameObject> inventorySlots;

    public int totalSlots;
    public int totalItems;

    public GameObject elementOwnerPrefab;//The grid space in which an item can reside
    public GameObject itemDrop;

    public bool canSelect = true;
<<<<<<< HEAD
>>>>>>> Rebuilding inventory

=======
    
>>>>>>> Shared Inventory scrolls up and down by keypress
    public void AddSlot()
    {
        if (inventorySlots == null)
        {
            inventorySlots = new Dictionary<object, GameObject>();

            totalItems = 0;
        }

        GameObject slotClone = Instantiate(elementOwnerPrefab, transform);
        slotClone.GetComponentInChildren<HighlightSelf>().SetEmptySlotImage();

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
                slot.GetComponentInChildren<HighlightSelf>().UnHighlight();

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

        numCols = GetComponent<GridLayoutGroup>().constraintCount;

        numRows = slots / numCols;
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
            curSlot.EmptySlot();
        else
            curSlot.SetQuantityText();
    }

    public void SetIndex(int val)
    {
        oldRow = currentRow;

        selectedIndex += val;

        selectedItem = inventorySlots[selectedIndex].GetComponent<InventorySlot>();

        currentRow = selectedIndex / numCols;

        if(selectedIndex >= totalSlots)
        {
            selectedIndex = totalSlots - 1;

            return;
        }
        else if(selectedIndex < 0)
        {
            selectedIndex = 0;

            return;
        }

        if (oldRow == currentRow)
            return;

        if (val == numCols)
        {
            if (oldRow == visibleRows[2] &&
                oldRow != currentRow &&
                currentRow < numRows)
            {
                for (int i = 0; i < visibleRows.Length; i++)
                {
                    visibleRows[i] += 1;
                }

                ShiftInventory(true);
            }  
        }
        else if (val == -numCols)
        {
            if (oldRow == visibleRows[0] &&
                oldRow != currentRow &&
                currentRow >= 0)
            {
                for (int i = 0; i < visibleRows.Length; i++)
                {
                    visibleRows[i] -= 1;
                }

                ShiftInventory(false);
            } 
        }
        else if(Mathf.Abs(val) == 1)
        {
            if(currentRow != oldRow)
            {
                if(val == 1 &&
                    oldRow != currentRow &&
                    oldRow == visibleRows[2])
                {
                    for (int i = 0; i < visibleRows.Length; i++)
                    {
                        visibleRows[i] += 1;
                    }

                    ShiftInventory(true);
                }
                else if(val == -1 &&
                     oldRow != currentRow &&
                     oldRow == visibleRows[0])
                {
                     for (int i = 0; i < visibleRows.Length; i++)
                     {
                         visibleRows[i] -= 1;
                     }

                     ShiftInventory(false);
                }
            }
        }
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

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
        selectedItem = inventorySlots[index].GetComponent<InventorySlot>();
>>>>>>> Rebuilding inventory
=======
        InventorySlot iS = inventorySlots[index].GetComponent<InventorySlot>();

        iS.Select();
>>>>>>> Inventory updated to select the first item slot on open.
=======
        selectedItem = inventorySlots[index].GetComponent<InventorySlot>();
>>>>>>> Revert "Inventory updated to select the first item slot on open."
=======
        InventorySlot iS = inventorySlots[index].GetComponent<InventorySlot>();

        selectedItem = iS;

        iS.Select();
>>>>>>> Cleaning up Inventory UI, item racks are now under each row
    }

    public void ShiftInventory(bool shiftDown)
    {
        if (shiftDown)
        {
            rTransform.localPosition += new Vector3(0, rowSpacing, 0);  
        }
        else
        {
            rTransform.localPosition -= new Vector3(0, rowSpacing, 0);
        }
    }
}
