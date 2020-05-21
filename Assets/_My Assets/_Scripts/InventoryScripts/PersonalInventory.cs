using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Threading;
/*
 * InventoryItem - Instance of an item in the inventory.
 * 
 * Author - Ian Rosenberg
 */

public class PersonalInventory : MonoBehaviour
{
	[Header("Personal Inventory")]
	private Dictionary<object, GameObject> inventoryItems;
	
	public GameObject elementOwnerPrefab;//The grid space in which an item can reside
	public uint numSlots;//how many slots do we start with?
	
	private void Start()
	{
		inventoryItems = new Dictionary<object, GameObject>();
		
		Populate();
	}

	private void Update()
	{

	}

	private void Populate()
	{
		ExpandInventory((int)numSlots);
	}

	private void AddSlot()
	{
		GameObject slotClone = Instantiate(elementOwnerPrefab, transform);
		uint currentIndex = (uint)inventoryItems.Count();
		//string n = slotClone.GetComponent<InventoryItem>().itemName;

		inventoryItems.Add(currentIndex, slotClone);
		//inventoryItems.Add(n, slotClone);
	}

	public void ExpandInventory(int slots)
	{
		for (int i = 0; i < slots; i++)
		{
			AddSlot();
		}
	}
}
