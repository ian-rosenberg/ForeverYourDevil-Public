using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public GameObject child;
    public bool inUse;

    private void Start()
    {
        child = null;
        inUse = false;
    }

    public void SetItem(GameObject item)
    {
        child = item;
    }

    public GameObject GetItem()
    {
        return child;
    }
}
