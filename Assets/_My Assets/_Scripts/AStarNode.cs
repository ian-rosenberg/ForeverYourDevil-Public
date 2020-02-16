using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{ 
    public AStarNode parent;

    public int id;

    //gird coords
    public int gridX = -1;
    public int gridZ = -1;

    public Vector3 worldPosition;

    public float hCost; // cost to move from start to square, following generated path
    public float gCost; //cost to get from current node to target
    public float fCost; //final cost, f + h

    //Can we use this neigbor?
    public bool validSpace;

    private GameObject highlightClone;

    public void ToggleSpaceValid()
    {
        validSpace = !validSpace;
    }

    public void SetID(int i)
    {
        this.id = i;
    }

    public void SetCoords(int x, int z, GameObject clone)
    {
        this.parent = null;
        this.gridX = x;
        this.gridZ = z;
        this.validSpace = true;
        this.highlightClone = clone;
        this.fCost = float.MaxValue; 
        this.gCost = float.MaxValue;
        this.hCost = float.MaxValue;
        this.worldPosition = Vector3.zero;
    }

    public void Highlight(bool ans)
    {
        if(ans)
        {
            this.highlightClone.transform.position = this.worldPosition;
        }
        else
        {
            this.highlightClone.transform.position = new Vector3(-100, -100, -100);
        }
    }

    public void SetGlowSpot(GameObject clone)
    {
        this.highlightClone = clone;
    }
}
