using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD;
using FMODUnity;

/*
 * InventoryItem - Instance of an item in the inventory.
 * 
 * Author - Ian Rosenberg
 */

public class InventoryItem : MonoBehaviour
{
    public string ID;
    public string Icon;
    public string Name;
    public bool Consumable;
    public int Cooldown;
    public int Duration;
    public string EffectDescription;
    public string Description;
    public string SFX;
    public ItemStats Bonuses;
}

public class ItemStats
{
    int HP;
    int Stamina;
    int Tolerance;
    int Attack;

}