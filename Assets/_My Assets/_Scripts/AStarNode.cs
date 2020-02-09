using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public AStarNode [,]neighbors;

    //gird coords
    public int gridX;
    public int gridZ;

    public Vector3 worldPosition;

    public int distToTarget;
    public int distToInitial;
    public float totalCost;

    //Can we use this neigbor?
    public bool validSpace;
    //
    public bool inCalc;
    
    // Start is called before the first frame update
    void Awake()
    {
        neighbors = new AStarNode[3, 3];

        gridX = -1;
        gridZ = -1;
        worldPosition = new Vector3(-1, -1, -1);
        distToTarget = -1;
        distToInitial = -1;

        totalCost = -1f;

        validSpace = true;
        inCalc = false;
    }

    void BubbleSortList(ref List<AStarNode> nodes)
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
}
