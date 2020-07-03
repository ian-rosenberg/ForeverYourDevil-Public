using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightSelf : MonoBehaviour
{
    public void Highlight(Color col)
    {
        GetComponent<Image>().color = col;
    }
}
