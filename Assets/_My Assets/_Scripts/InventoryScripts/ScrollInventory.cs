using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollInventory : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool up;
    public Color selectColor = new Color(.55f, 0.01f, 0.1f, 1f);

    private Color oColor;
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
        oColor = img.color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (up)
        {
            InventoryManagement.Instance.sharedInventory.GetComponentInChildren<Inventory>().SetIndex(-4);
        }
        else
        {
            InventoryManagement.Instance.sharedInventory.GetComponentInChildren<Inventory>().SetIndex(4);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        img.color = selectColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.color = oColor;
    }
}
