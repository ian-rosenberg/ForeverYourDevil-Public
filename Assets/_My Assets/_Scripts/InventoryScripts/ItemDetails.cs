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
    public Image blank;

    private InventoryManagement invManager;

    private void Awake()
    {
        invManager = InventoryManagement.Instance;

        itemName.text = "";
        itemDescription.text = "Description:\n";
        effectDescription.text = "Effect Description:\n";
        effects.text = "Effects:\n";

        itemImage.sprite = null;
    }

    public void Clear()
    {
        itemName.text = "";
        itemDescription.text = "Description:\n";
        effectDescription.text = "Effect Description:\n";
        effects.text = "Effects:\n";

        itemImage.sprite = null;
    }

    public void SetDetails(ItemBase item)
    {
        if (item == null)
            return;

        Color newColor = itemImage.material.color;

        newColor.a = 255;

        itemName.text = item.Name;
        effectDescription.text = "Effect Description:\n" + item.EffectDescription;
        itemDescription.text = "Description:\n" + item.Description;
        effects.text = "Effects:\n" + item.StatBonusesToString();

        itemImage.material.SetColor("_Color", newColor );

        itemImage.sprite = invManager.GetItemImage(item);

    }
}
