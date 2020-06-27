using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
<<<<<<< HEAD
using TMPro;
=======
using UnityEngine.InputSystem;
using TMPro;
using System;
>>>>>>> Rebuilding inventory

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    private bool selected;
    private HighlightSelf hs;

    public ItemDetails detailsObj;
    public ItemBase child;
    public bool inUse;
    public int quantity;
    public int maxQuantity;
    public Image img;

    public GameObject ownerInventory;

    private void Awake()
    {
        child = null;
        inUse = false;
        quantity = 0;
        selected = false;
        hs = GetComponentInChildren<HighlightSelf>();
        img = hs.GetComponent<Image>();
        hs.SetEmptySlotImage();
        ownerInventory = this.transform.parent.gameObject;

        GameObject details = GameObject.FindGameObjectWithTag("ItemDetails");  
        detailsObj = details.GetComponentInChildren<ItemDetails>();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
<<<<<<< HEAD
        if (selected)
            return;
=======
        if(!ownerInventory.GetComponent<Inventory>().canSelect)
        {
            if (selected)
                UnSelect();

            return;
        }
        InventorySlot slot = ownerInventory.GetComponent<Inventory>().GetLastSelected();

        if (slot != null)
            slot.UnSelect();

        //Set up for right click to instantiate a shortcut menu
        //if(Mouse.current.rightButton.ReadValue() != 0)

>>>>>>> Rebuilding inventory
        
        Dictionary<object, GameObject> itemDict = ownerInventory.GetComponent<Inventory>().inventorySlots;

        foreach(KeyValuePair<object, GameObject> pair in itemDict)
        {
            bool s = pair.Value.GetComponent<InventorySlot>().selected;
            
            if(s)
                pair.Value.GetComponent<InventorySlot>().UnSelect();
            
        }

        hs.Highlight();

        selected = true;

<<<<<<< HEAD
        detailsObj.SetDetails(child);
=======
        if(child == null)
        {
            detailsObj.Clear();
        }
        else
        {
            detailsObj.SetDetails(child);
        }
>>>>>>> Rebuilding inventory
    }

    public void UnSelect()
    {
        selected = false;

        if (child == null)
            hs.SetEmptySlotImage();
        else
            hs.UnHighlight();
    }

<<<<<<< HEAD
=======
    public void Select()
    {
        selected = true;

        hs.Highlight();

        if (child != null)
            detailsObj.SetDetails(child);
        else
            EmptyDetails();
    }
    

>>>>>>> Rebuilding inventory
    public void SetQuantityText()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = quantity.ToString();
    }
<<<<<<< HEAD
=======

    public bool Selected()
    {
        return selected;
    }

    public void EmptySlot()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = "";

        child = null;

        EmptyDetails();

        inUse = false;

        hs.SetEmptySlotImage();
    }

    public void EmptyDetails()
    {
        detailsObj.effectDescription.text = "Effect Description:\n";
        detailsObj.effects.text = "Effects:\n";
        detailsObj.itemDescription.text = "Description:\n";
        detailsObj.itemImage.sprite = detailsObj.blank;
        detailsObj.itemName.text = "";
    }
>>>>>>> Rebuilding inventory
}
