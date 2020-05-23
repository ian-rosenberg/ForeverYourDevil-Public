using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManagement : MonoBehaviour
{
    #region Inventory Management Instance
    private static InventoryManagement instance;
    public static InventoryManagement Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<InventoryManagement>();
            return instance;
        }
    }
    #endregion

    private List<GameObject> inventoryObjs;//The inventories in use;
    private GameObject sharedInventory;

    public GameObject blurShader;
    public GameObject sharedInventoryPrefab;
    public GameObject personalInventoryPrefab;
    public GameObject battleInventoryPrefab;

    public int numInventories;


    // Start is called before the first frame update
    void Start()
    {
        inventoryObjs = new List<GameObject>();

        GameObject inventoryClone = Instantiate(sharedInventoryPrefab, this.transform);

        inventoryObjs.Add(inventoryClone);

        //for(int i = 0; i < numInventories; i++)
        //{
        //AddInventory();
        //}

        //CreateSharedInventory();

        //SetInventoriesInactive();
    }

    private void AddInventory()
    {
        GameObject inventoryClone = Instantiate(personalInventoryPrefab, transform);
        
        inventoryObjs.Add(inventoryClone);
    }

    private void CreateSharedInventory()
    {
        sharedInventory = Instantiate(sharedInventoryPrefab, transform);
    }

    public void SetInventoriesInactive()
    {
        sharedInventory.SetActive(false);

        foreach(GameObject inventory in inventoryObjs)
        {
            inventory.SetActive(false);
        }
    }

    public void SetSharedInventoryActive()
    {
        sharedInventory.SetActive(true);
    }
}
