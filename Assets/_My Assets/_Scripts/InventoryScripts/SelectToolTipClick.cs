using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class SelectToolTipClick : MonoBehaviour, IPointerClickHandler
{
    private TooltipMenu parentObj;

    public bool currentlySelected;

    public bool use;
    public bool drop;
    public bool move;
    public bool cancel;

    // Start is called before the first frame update
    void Start()
    {
        parentObj = GetComponentInParent<TooltipMenu>();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(!currentlySelected)
        {
            parentObj.UnSelectLastClickedOption();

            currentlySelected = true;

            parentObj.ChangeSelectionColor();
            
            return;
        }

        if (use)
            parentObj.selected = parentObj.use;
        else if (move)
            parentObj.selected = parentObj.move;
        else if (drop)
            parentObj.selected = parentObj.drop;
        else if (cancel)
            parentObj.selected = parentObj.cancel;
        
        if (use)
        {
            parentObj.UnSelectLastClickedOption();

            if (use)
                parentObj.selected = parentObj.use;
            else if (move)
                parentObj.selected = parentObj.move;
            else if (drop)
                parentObj.selected = parentObj.drop;
            else if (cancel)
                parentObj.selected = parentObj.cancel;

            parentObj.ChangeSelectionColor();

            return;
        }
        else if (drop)
        {
            parentObj.UnSelectLastClickedOption();

            if (use)
                parentObj.selected = parentObj.use;
            else if (move)
                parentObj.selected = parentObj.move;
            else if (drop)
                parentObj.selected = parentObj.drop;
            else if (cancel)
                parentObj.selected = parentObj.cancel;

            parentObj.ChangeSelectionColor();

            Inventory current = InventoryManagement.Instance.GetCurrentInventory();

            InventorySlot slot = current.inventorySlots[current.selectedIndex].GetComponent<InventorySlot>();

            current.DropItem();

            parentObj.CloseMenu();
        }
        else if (move)
        {
            parentObj.UnSelectLastClickedOption();

            if (use)
                parentObj.selected = parentObj.use;
            else if (move)
                parentObj.selected = parentObj.move;
            else if (drop)
                parentObj.selected = parentObj.drop;
            else if (cancel)
                parentObj.selected = parentObj.cancel;

            parentObj.ChangeSelectionColor();

            parentObj.CloseMenu();
        }
        else if (cancel)
        {
            parentObj.UnSelectLastClickedOption();

            if (use)
                parentObj.selected = parentObj.use;
            else if (move)
                parentObj.selected = parentObj.move;
            else if (drop)
                parentObj.selected = parentObj.drop;
            else if (cancel)
                parentObj.selected = parentObj.cancel;

            parentObj.ChangeSelectionColor();

            parentObj.CloseMenu();
        }

        SharedInventory sI = InventoryManagement.Instance.sharedInventory.GetComponentInChildren<SharedInventory>();

        InventorySlot s = sI.GetSelected();
        
        s.UnSelect();
        sI.SelectItemByIndex(0);       
    }
}
