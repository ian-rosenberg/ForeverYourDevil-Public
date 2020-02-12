using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public AStarNode [,]neighbors;

    public int id;

    //gird coords
    public int gridX = -1;
    public int gridZ = -1;

    public Vector3 worldPosition;

    public float distToTarget;
    public float distToInitial;
    public float hCost;
    public float gCost;
    public float totalCost;

    //Can we use this neigbor?
    public bool validSpace;
    //Are we being used in pathfinding at this moment?
    public bool inCalc;

    public void ToggleSpaceValid()
    {
        validSpace = !validSpace;
    }

    public void BubbleSortList(ref List<AStarNode> nodes)
    {
        bool done = false;
        int i = 0;

        while (!done)
        {
            for(;i< nodes.Count - 1; i++)
            {
                if(nodes[i].totalCost > nodes[i+1].totalCost)
                {
                    AStarNode temp = nodes[i];
                    nodes[i] = nodes[i + 1];
                    nodes[i + 1] = temp;

                    continue;
                }

                done = true;
            }
        }
    }

    public void SetID(int i)
    {
        this.id = i;
    }

    public void SetCoords(int x, int z)
    {
        this.gridX = x;
        this.gridZ = z;
    }
}
