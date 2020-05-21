using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * InventoryItem - Instance of an item in the inventory.
 * 
 * Author - Ian Rosenberg
 */

public class InventoryItem : MonoBehaviour
{
    //items
    [Header("Inventory Item")]
    private int id; //id referencing an item from 
    private ItemType itemType;// Categorization for switch cases
    private GameObject owner;//who owns this item?
    private GameObject ownerSlot;//the inventory slot that holds this item

    public string itemName;//another way to reference item, by name

    public enum ItemType//What kind of item are we?
    {
        Concoction,
        Equipment,
        Ability
    }

    
    public void UseItem()
    {
        //lets apply any effects on the player
        //owner.GetComponent<PlayerController>().ApplyEffect();

        switch(itemType)
        {
            case ItemType.Concoction:
                //affect stats
                break;

            case ItemType.Equipment:
                //Wear equipment
                break;

            case ItemType.Ability:
                //Toggle Equip ability
                break;

            default://shouldn't get here
                break;

        }
    }
}
