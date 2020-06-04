using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    private bool selected;

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
        img = GetComponent<Image>();
        ownerInventory = this.transform.parent.gameObject;

        GameObject details = GameObject.FindGameObjectWithTag("ItemDetails");  
        detailsObj = details.GetComponentInChildren<ItemDetails>();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (selected)
            return;
        
        Dictionary<object, GameObject> itemDict = ownerInventory.GetComponent<Inventory>().inventorySlots;

        foreach(KeyValuePair<object, GameObject> pair in itemDict)
        {
            bool s = pair.Value.GetComponent<InventorySlot>().selected;
            
            if(s)
                pair.Value.GetComponent<InventorySlot>().UnSelect();
            
        }

        GetComponentInChildren<HighlightSelf>().Highlight(Color.green);

        selected = true;

        detailsObj.SetDetails(child);
    }

    public void UnSelect()
    {
        selected = false;

        GetComponentInChildren<HighlightSelf>().Highlight(Color.black);
    }

    public void SetQuantityText()
    {
        GetComponentInChildren<TextMeshProUGUI>().text = quantity.ToString();
    }
}
