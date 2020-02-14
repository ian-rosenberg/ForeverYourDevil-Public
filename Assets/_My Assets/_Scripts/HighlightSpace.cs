using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightSpace : MonoBehaviour
{
    public int maxNodes;

    public GameObject gridSpotHighlight;

    public Dictionary<GameObject, int> hSpots;

    public void Awake()
    {
        Quaternion r = GameObject.Find("Grid").transform.rotation;
        maxNodes = 256;

        hSpots = new Dictionary<GameObject, int>();
        
        for (int i = 0; i < maxNodes; i++)
        {
            GameObject clone = Instantiate(gridSpotHighlight, new Vector3(-100, -100, -100), r);

            hSpots.Add(clone, 0);
        }
    }

    public GameObject GetUnusedSpot()
    {
        foreach (KeyValuePair<GameObject, int> pair in hSpots)
        {
            if (pair.Value == 0)
            {
                hSpots[pair.Key] = 1;

                return pair.Key;
            }
        }

        Debug.Log("Grabbing unused glow tile");
        return null;
    }

    public void ClearHighlightedSpots()
    {
        hSpots.Clear();    
    }
}