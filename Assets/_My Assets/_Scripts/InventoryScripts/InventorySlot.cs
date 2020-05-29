using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemBase child;
    public bool inUse;
    public int quantity;
    public Image img;

    private void Awake()
    {
        child = null;
        inUse = false;
        quantity = 0;
        img = GetComponent<Image>();
    }
}
