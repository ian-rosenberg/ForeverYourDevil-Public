using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public GameObject child;
    public bool inUse;
    public int quantity;

    private void Start()
    {
        child = null;
        inUse = false;
        quantity = 0;
    }

    public void SetItem(GameObject item, int qty)
    {
        child = item;
        quantity = qty;
    }

    public GameObject GetItem()
    {
        return child;
    }

    public int GetItemQuantity()
    {
        return quantity;
    }
}
