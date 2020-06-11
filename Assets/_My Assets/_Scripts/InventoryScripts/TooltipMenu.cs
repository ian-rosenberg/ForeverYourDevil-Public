using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class TooltipMenu : MonoBehaviour
{
    public Color selectColor;
    public Color unSelectColor;

    public GameObject use;
    public GameObject move;
    public GameObject drop;

    private GameObject selected = null;
    private GameObject prevSelected = null;

    #region Player Actions
    public PlayerControls pControls;

    private void OnEnable()
    {
        pControls = new PlayerControls();

        pControls.UI.Cancel.performed += ReturnToPreviousMenu;
        pControls.UI.Navigate.performed += HandleTooltipNavigation;
        pControls.UI.Interact.performed += AcceptMenuItem;

        selected = use;
        selected.GetComponent<Image>().color = selectColor;
        unSelectColor = GetComponent<Image>().color;

        pControls.UI.Interact.Enable();
        pControls.UI.Navigate.Enable();
        pControls.UI.Cancel.Enable();
    }

    private void OnDisable()
    {
        pControls.UI.Cancel.performed -= ReturnToPreviousMenu;
        pControls.UI.Navigate.performed -= HandleTooltipNavigation;
        pControls.UI.Interact.performed -= AcceptMenuItem;

        selected.GetComponent<Image>().color = unSelectColor;
        selected = null;


        pControls.UI.Interact.Disable();
        pControls.UI.Navigate.Disable();
        pControls.UI.Cancel.Disable();
    }

    private void AcceptMenuItem(InputAction.CallbackContext obj)
    {
        if(selected == drop)
        {
            Inventory current = InventoryManagement.Instance.GetCurrentInventory();

            InventorySlot slot = current.inventorySlots[current.selectedIndex].GetComponent<InventorySlot>();

            current.DropItem(obj);

            if(!slot.inUse)
            {
                gameObject.SetActive(false);
            }

            CloseMenu();
        }
    }

    private void CloseMenu()
    {
        InventoryManagement invMan = GetComponentInParent<InventoryManagement>();
        invMan.EnableInventoryInput();

        SendMessageUpwards("CloseTooltip");        
    }

    private void ReturnToPreviousMenu(InputAction.CallbackContext obj)
    {
        
    }
    #endregion

    public void HandleTooltipNavigation(InputAction.CallbackContext context)
    {
        if (gameManager.Instance.gameState != gameManager.STATE.PAUSED)
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
                prevSelected = use;
            }
            else if(selected == move)
            {
                selected = drop;
                prevSelected = move;
            }
            else if (selected == drop)
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
                prevSelected = move;
            }
            else if (selected == drop)
            {
                selected = move;
                prevSelected = drop;
            }
        }

        selected.GetComponent<Image>().color = selectColor;
            
        prevSelected.GetComponent<Image>().color = unSelectColor;
        
    }
}