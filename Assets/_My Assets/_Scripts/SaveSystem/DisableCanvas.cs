using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCanvas : MonoBehaviour
{
    public GameObject canvas;

    public void disableCanvas(float delay)
    {
        StartCoroutine(delayedDisable(delay));
    }

    IEnumerator delayedDisable(float delay)
    {
        yield return new WaitForSeconds(delay);
        canvas.SetActive(false);
    }
}
