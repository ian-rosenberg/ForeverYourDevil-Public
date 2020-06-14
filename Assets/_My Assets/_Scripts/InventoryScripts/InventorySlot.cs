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

    public ItemDetails detailsObj;
    public ItemBase child;
    public bool inUse;
    public int quantity;
    public int maxQuantity;
    public Sprite childItem;
    public Sprite emptySlot;
    public Sprite itemSlot;
    public Sprite selectedSlot;
    public Image currentImg;

    public GameObject ownerInventory;


    private void Awake()
    {
        child = null;
        inUse = false;
        quantity = 0;
        selected = false;
        currentImg = GetComponent<Image>();
        childItem = GetComponent<Sprite>();
        emptySlot = GetComponent<Sprite>();
        selected = GetComponent<Sprite>();

        currentImg.sprite = emptySlot;

        ownerInventory = this.transform.parent.gameObject;

        GameObject details = GameObject.FindGameObjectWithTag("ItemDetails");  
        detailsObj = details.GetComponentInChildren<ItemDetails>();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Set up for right click to instantiate a shortcut menu
        if(pointerEventData.button == PointerEventData.InputButton.Right)
        {
            Inventory i = GetComponentInParent<Inventory>();
            InputAction.CallbackContext nullObj = new InputAction.CallbackContext();

            if (i.GetSelected() != null)
                SendMessageUpwards("AcceptSelection", nullObj);

            return;
        }    
        
        if(!ownerInventory.GetComponent<Inventory>().canSelect)
        {
            if (selected)
                UnSelect();

            return;
        }
        InventorySlot slot = ownerInventory.GetComponent<Inventory>().GetLastSelected();

        if (slot != null)
            slot.UnSelect();

        
        Dictionary<object, GameObject> itemDict = ownerInventory.GetComponent<Inventory>().inventorySlots;

        foreach(KeyValuePair<object, GameObject> pair in itemDict)
        {
            bool s = pair.Value.GetComponent<InventorySlot>().selected;
            
            if(s)
                pair.Value.GetComponent<InventorySlot>().UnSelect();
            
        }

        GetComponentInChildren<HighlightSelf>().Highlight(Color.green);

        selected = true;

        if(child == null)
        {
            detailsObj.Clear();
        }
        else
        {
            detailsObj.SetDetails(child);
        }
    }

    public void UnSelect()
    {
        selected = false;

        GetComponentInChildren<HighlightSelf>().Highlight(Color.black);
    }

    public void Select()
    {
        selected = true;

        GetComponent<Image>().sprite = selectedSlot;


        InventoryManagement invMan = GetComponentInParent<InventoryManagement>();
        HighlightSelf s = GetComponentInChildren<HighlightSelf>();


        s.GetComponent<Image>().sprite = invMan.GetItemImage(child);

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

        currentImg.sprite = emptySlot;
    }

    public void EmptyDetails()
    {
        detailsObj.effectDescription.text = "Effect Description:\n";
        detailsObj.effects.text = "Effects:\n";
        detailsObj.itemDescription.text = "Description:\n";
        detailsObj.itemImage.sprite = null;
        detailsObj.itemName.text = "";
    }
}
