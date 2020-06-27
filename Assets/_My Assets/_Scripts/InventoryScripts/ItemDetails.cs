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

<<<<<<< HEAD
    public Image itemImage;
<<<<<<< HEAD

    private void Awake()
    {
        itemName.text = "";
        itemDescription.text = "Description: ";
        effectDescription.text = "Effect Description: ";
        effects.text = "Effects: ";
=======
    public Image blank;
=======
    public Sprite blank;
>>>>>>> UI update in progress for Shared Inventory

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
>>>>>>> Rebuilding inventory

        itemImage.sprite = blank;
    }

    public void SetDetails(ItemBase item)
    {
<<<<<<< HEAD
        if (item == null)
            return;
<<<<<<< HEAD
        
        itemName.text = item.Name;
        effectDescription.text = "Effect Description: " + item.EffectDescription;
        itemDescription.text = "Description: " + item.Description;
        effects.text = "Effects: \n" + item.StatBonusesToString();

        itemImage.sprite = InventoryManagement.Instance.GetItemImage(item);
=======

=======
>>>>>>> UI update in progress for Shared Inventory
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

<<<<<<< HEAD
>>>>>>> Rebuilding inventory
=======
        itemImage.material.SetColor("_Color", newColor);
>>>>>>> UI update in progress for Shared Inventory
    }
}
