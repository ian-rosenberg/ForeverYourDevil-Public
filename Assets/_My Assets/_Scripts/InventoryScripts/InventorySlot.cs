using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    private bool selected;
    private HighlightSelf hs;

    public Sprite blank;
    public ItemDetails detailsObj;
    public ItemBase child;
    public bool inUse;
    public int quantity;
    public int maxQuantity;
    public Image img;

    public Inventory ownerInventory;

    private void Awake()
    {
        child = null;
        inUse = false;
        quantity = 0;
        selected = false;
        hs = GetComponentInChildren<HighlightSelf>();
        img = hs.GetComponent<Image>();
        hs.SetEmptySlotImage();
        ownerInventory = GetComponentInParent<Inventory>();

        GameObject details = GameObject.FindGameObjectWithTag("ItemDetails");  
        detailsObj = details.GetComponentInChildren<ItemDetails>();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(!ownerInventory.GetComponent<Inventory>().canSelect)
        {
            if (selected)
                UnSelect();
        }

        InventorySlot slot = ownerInventory.GetComponent<Inventory>().GetLastSelected();

        if (slot != null)
            slot.UnSelect();
    
        Dictionary<object, GameObject> itemDict = ownerInventory.inventorySlots;

        foreach(GameObject val in itemDict.Values)
        {
            bool s = val.GetComponent<InventorySlot>().selected;
            
            if(s)
                val.GetComponent<InventorySlot>().UnSelect();
            
        }

        hs.Highlight();

        selected = true;

        if(child == null)
        {
            detailsObj.Clear();
        }
        else
        {
            detailsObj.SetDetails(child);
        }

        ownerInventory.GetComponent<Inventory>().GetIndexBySlot(gameObject);
    }

    public void UnSelect()
    {
        selected = false;

        if (child == null)
            hs.SetEmptySlotImage();
        else
            hs.UnHighlight();
    }

    public void Select()
    {
        selected = true;

        hs.Highlight();

        if (child != null)
            detailsObj.SetDetails(child);
        else
            EmptyDetails();
    }
    
    public void SetQuantityText()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = quantity.ToString();
    }

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
        img.sprite = blank;
    }

    public void EmptyDetails()
    {
        detailsObj.effectDescription.text = "Effect Description:\n";
        detailsObj.effects.text = "Effects:\n";
        detailsObj.itemDescription.text = "Description:\n";
        detailsObj.itemImage.sprite = detailsObj.blank;
        detailsObj.itemName.text = "";
    }
}
