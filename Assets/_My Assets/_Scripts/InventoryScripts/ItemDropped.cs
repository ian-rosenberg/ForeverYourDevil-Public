using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropped : MonoBehaviour
{
    public float itemScale = 3f;

    private SpriteRenderer sRen = null;
    private ItemBase item = null;
    private bool pickupFlag = false;

    private void Awake()
    {
        sRen = GetComponent<SpriteRenderer>();
    }

    public void SetItem(ItemBase i)
    {
        sRen.sprite = InventoryManagement.Instance.GetItemImage(i);
        item = i;

        transform.localScale *= itemScale;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        transform.Rotate(Vector3.up, Time.deltaTime * 100f);
    }

    public ItemBase GetItem()
    {
        return item;
    }
}
