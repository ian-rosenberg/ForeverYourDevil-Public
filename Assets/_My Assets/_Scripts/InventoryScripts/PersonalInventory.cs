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

public class PersonalInventory : Inventory
{
	[Header("Personal Inventory")]
	public uint numSlots;//how many slots do we start with?
	public GameObject characterOwner;
	
	private void Start()
	{
		Populate();
	}

	private void Populate()
	{
		ExpandInventory((int)numSlots);
	}
}
