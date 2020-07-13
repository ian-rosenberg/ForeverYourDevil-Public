using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class TooltipMenu : MonoBehaviour
{
    public GameObject use;
    public GameObject move;
    public GameObject drop;
    public GameObject cancel;

    public GameObject selected = null;
    public GameObject prevSelected = null;
    
    private Color selectColor;
    private Color unSelectColor;

    #region Player Actions
    public PlayerControls pControls;

    private void Start()
    {
        selected = use;
        unSelectColor = new Color(0.6431373f, 0.5764706f, 0.4470589f, 1f);
        selectColor = new Color(0.8584906f, 0.3567862f, 0f, 1f);
        selected.GetComponent<Image>().color = selectColor;

        prevSelected = selected;
    }

    private void OnEnable()
    {
        pControls = new PlayerControls();

        pControls.UI.Navigate.performed += HandleTooltipNavigation;
        pControls.UI.Interact.performed += AcceptMenuItem;

        pControls.UI.Interact.Enable();
        pControls.UI.Navigate.Enable();
    }

    private void OnDisable()
    {
        pControls.UI.Navigate.performed -= HandleTooltipNavigation;
        pControls.UI.Interact.performed -= AcceptMenuItem;

        selected = use;

        use.GetComponent<Image>().color = selectColor;
        move.GetComponent<Image>().color = unSelectColor;
        drop.GetComponent<Image>().color = unSelectColor;
        cancel.GetComponent<Image>().color = unSelectColor;


        pControls.UI.Interact.Disable();
        pControls.UI.Navigate.Disable();
    }

    private void AcceptMenuItem(InputAction.CallbackContext obj)
    {
        if (selected == drop)
        {
            Inventory current = InventoryManagement.Instance.GetCurrentInventory();

            InventorySlot slot = current.inventorySlots[current.selectedIndex].GetComponent<InventorySlot>();

            current.DropItem(obj);

            CloseMenu();

            SharedInventory sI = InventoryManagement.Instance.sharedInventory.GetComponentInChildren<SharedInventory>();

            if (sI.totalItems == 0)
                sI.CloseInventory();
        }
        else if(selected == cancel)
        {
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        InventoryManagement invMan = GetComponentInParent<InventoryManagement>();
        
        SendMessageUpwards("CloseTooltip");    
        
        invMan.EnableInventoryInput();
    }
    #endregion

    public void HandleTooltipNavigation(InputAction.CallbackContext context)
    {
        if (gameManager.Instance.gameState != gameManager.STATE.PLAYER_PAUSED)
            return;

        Vector2 movement = context.ReadValue<Vector2>();

        if (movement.x == 1)
        {
            return;
        }
        else if (movement.x == -1)
        {
            return;
        }
        else if (movement.y == -1)
        {
            if(selected == use)
            {
                selected = move;

                move.GetComponent<SelectToolTipClick>().move = true;

                prevSelected = use;
            }
            else if(selected == move)
            {
                selected = drop;

                move.GetComponent<SelectToolTipClick>().drop = true;


                prevSelected = move;
            }
            else if(selected == drop)
            {
                selected = cancel;

                move.GetComponent<SelectToolTipClick>().cancel = true;


                prevSelected = drop;
            }
            else if (selected == cancel)
            {
                return;
            }
        }
        else if (movement.y == 1)
        {
            if (selected == use)
            {
                return;
            }
            else if (selected == move)
            {
                selected = use;

                move.GetComponent<SelectToolTipClick>().use = true;


                prevSelected = move;
            }
            else if (selected == drop)
            {
                selected = move;

                move.GetComponent<SelectToolTipClick>().move = true;


                prevSelected = drop;
            }
            else if (selected == cancel)
            {
                selected = drop;

                move.GetComponent<SelectToolTipClick>().drop = true;


                prevSelected = cancel;
            }
        }

        ChangeSelectionColor();
    }

    public void ChangeSelectionColor()
    {
        selected.GetComponent<Image>().color = selectColor;

        prevSelected.GetComponent<Image>().color = unSelectColor;
    }

    public void UnSelectLastClickedOption()
    {
        SelectToolTipClick[] items = GetComponentsInChildren<SelectToolTipClick>();

        GameObject go = null; 

        foreach (SelectToolTipClick tip in items)
        {
            if (tip.currentlySelected)
            {
                go = tip.gameObject;

                tip.currentlySelected = false;

                go.GetComponent<Image>().color = unSelectColor; 
                
                if (go == use)
                {
                    prevSelected = use;
                }
                else if (go == move)
                {
                    prevSelected = move;
                }
                else if (go == drop)
                {
                    prevSelected = drop;
                }
                else if (go == cancel)
                {
                    prevSelected = cancel;
                }
            } 
        }
    }
}