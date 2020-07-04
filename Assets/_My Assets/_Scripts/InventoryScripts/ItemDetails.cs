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

    public Sprite blank;

    public Image itemImage;
    
    private InventoryManagement invManager;

    private void Awake()
    {
        invManager = InventoryManagement.Instance;

        Image[] imgs = GetComponentsInChildren<Image>();
        foreach(Image img in imgs)
        {
            if(img.gameObject != this.gameObject)
            {
                itemImage = img;
            }
        }


        itemName.text = "";
        itemDescription.text = "Description:\n";
        effectDescription.text = "Effect Description:\n";
        effects.text = "Effects:\n";

        itemImage.sprite = blank;
    }

    public void Clear()
    {
        itemName.text = "";
        itemDescription.text = "Description:\n";
        effectDescription.text = "Effect Description:\n";
        effects.text = "Effects:\n";

        itemImage.sprite = blank;
    }

    public void SetDetails(ItemBase item)
    {
        Color newColor = itemImage.material.color;

        newColor.a = 255;

        itemName.text = item.Name;
        effectDescription.text = "Effect Description:\n" + item.EffectDescription;
        itemDescription.text = "Description:\n" + item.Description;
        effects.text = "Effects:\n" + item.StatBonusesToString();

        itemImage.material.SetColor("_Color", newColor );

        itemImage.sprite = invManager.GetItemImage(item);
    }
    
    public void SetBlankImage()
    {
        Color newColor = itemImage.material.color;

        newColor.a = 255;

        itemImage.material.SetColor("_Color", newColor);
    }
}
