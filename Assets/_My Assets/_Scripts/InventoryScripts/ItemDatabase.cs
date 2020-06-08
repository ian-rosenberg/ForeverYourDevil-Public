using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<InventoryItem> itemList;

    public void Start()
    {
        itemList = new List<InventoryItem>();

        DontDestroyOnLoad(this.gameObject);
    }
}
