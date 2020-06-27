using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightSelf : MonoBehaviour
{
    public Sprite selected;
    public Sprite notSelected;
    public Sprite empty;

    private Sprite s;

    private void Awake()
    {
        s = transform.parent.GetComponent<Image>().sprite;

        SetEmptySlotImage();
    }

    public void Highlight()
    {
        s = selected;
        transform.parent.GetComponent<Image>().sprite = s;
    }

    public void UnHighlight()
    {
        s = notSelected;
        transform.parent.GetComponent<Image>().sprite = s;
    }

    public void SetEmptySlotImage()
    {
        s = empty;
        transform.parent.GetComponent<Image>().sprite = s;
    }
}
