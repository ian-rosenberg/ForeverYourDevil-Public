using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropped : MonoBehaviour
{
    public GameObject pickupIndicator;

    public float itemScale = 3f;

    private SpriteRenderer sRen = null;
    private ItemBase item = null;

    private void Awake()
    {
        sRen = GetComponent<SpriteRenderer>();
    }

    public void SetItem(ItemBase i)
    {
        sRen.sprite = InventoryManagement.Instance.GetItemImage(i);
        item = i;

        transform.localScale *= itemScale;

        pickupIndicator = GetComponentInChildren<Animator>().gameObject;
        pickupIndicator.SetActive(false);

        pickupIndicator.transform.localScale /= (itemScale + 1);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            pickupIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            pickupIndicator.SetActive(false);
        }
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
