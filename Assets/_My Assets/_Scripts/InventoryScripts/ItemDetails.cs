using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetails : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI effectDescription;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI effects;

    public Image itemImage;

    private void Awake()
    {
        itemName.text = "";
        itemDescription.text = "Description: ";
        effectDescription.text = "Effect Description: ";
        effects.text = "Effects: ";

        itemImage.sprite = null;
    }

    public void SetDetails(ItemBase item)
    {
        if (item == null)
            return;
        
        itemName.text = item.Name;
        effectDescription.text = "Effect Description: " + item.EffectDescription;
        itemDescription.text = "Description: " + item.Description;
        effects.text = "Effects: \n" + item.StatBonusesToString();

        itemImage.sprite = InventoryManagement.Instance.GetItemImage(item);
    }
}
