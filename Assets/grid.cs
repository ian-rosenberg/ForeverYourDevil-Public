using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tutorial Used: https://www.youtube.com/watch?v=VBZFYGWvm4A
public class grid : MonoBehaviour
{
    public struct NodeLayout
    {
        AStarNode[,] grid;
        public uint dimensionsX;
        public uint dimensionsZ;

        public NodeLayout(int x, int z)
        {
            dimensionsX = (uint)x;
            dimensionsZ = (uint)z;

            grid = new AStarNode[x, z];
        }

        public AStarNode GetGridNode(int x, int z)
        {
            return grid[z, x];
        }
    }

    public const int homeFromNeighborValue = 1;

    public NodeLayout nodeGrid;
    
    public Dictionary<AStarNode, AStarNode[,]> nodeDict;//key is the node, value is its neighbors
    
    public Vector3 scale = Vector2.one;
    public GameObject gridThing;

   //Get nearest grid point to whichever position is specified (ideally mouse position)
    public Vector3 NearestGridPoint(Vector3 position)
    {
        //Subtract offset from math, then add it back in
        position -= transform.position;

        //Get the point closest to where your mouse is via rounding
        int xCount = Mathf.RoundToInt(position.x / scale.x);
        int yCount = Mathf.RoundToInt(position.y / scale.y);
        int zCount = Mathf.RoundToInt(position.z / scale.z);

        //Form vector with this point and separate by scale
        Vector3 result = new Vector3(
            (float)xCount * scale.x,
            (float)yCount * scale.y,
            (float)zCount * scale.z);

        //Add offset back
        return result += transform.position;
    }

    //Draw the grid in Editor. 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        for (float x = 0; x < nodeGrid.dimensionsX; x += scale.x)
        {

            for (float z = 0; z < nodeGrid.dimensionsZ; z += scale.z)
            {
                // var point = NearestGridPoint(new Vector3(transform.position.x + x, 0f, transform.position.z + z));
                var point = NearestGridPoint(new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z));
                //Gizmos.DrawSphere(point, scale * 0.1f);
                Gizmos.DrawWireCube(point, new Vector3(scale.x * 1, 0f, scale.z * 1));
            }


        }
    }
    // Start is called before the first frame update
    void Start()
    {
        nodeDict = new Dictionary<AStarNode, AStarNode[,]>();

        nodeGrid = new NodeLayout(20, 20); 
        
        makeGrid();
    }

    // Update is called once per frame
    void makeGrid()
    {
        for (int x = 0; x < nodeGrid.dimensionsX; x++)
        {

            for (int z = 0; z < nodeGrid.dimensionsZ; z++)
            {
                // var point = NearestGridPoint(new Vector3(transform.position.x + x, 0f, transform.position.z + z));
                var point = NearestGridPoint(new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z));

                //Gizmos.DrawSphere(point, scale * 0.1f);
                Instantiate(gridThing, point, gridThing.transform.rotation);

                SetGridNodePosition(x, z, true);
            }
        }

        for (int x = 0; x < nodeGrid.dimensionsX; x++)
        {

            for (int z = 0; z < nodeGrid.dimensionsZ; z++)
            {
                AStarNode n = nodeGrid.GetGridNode(x,z);

                AStarNode [,]nList = GetKeyByValue(nodeDict, n);

                PopulateNeighbors(n, ref nList);
            }
        }
    }

    public AStarNode[,] GetKeyByValue(Dictionary<AStarNode, AStarNode[,]> dict, AStarNode val)
    {
        AStarNode[,] nList = null;

        foreach (KeyValuePair<AStarNode, AStarNode[,]> pair in dict)
        {
            if (EqualityComparer<AStarNode>.Default.Equals(pair.Key, val))
            {
                nList = pair.Value;
                break;
            }
        }
        return nList;
    }

    //Self explanatory
    private void SetGridNodePosition(int x, int z, bool walkableSpace)
    {
        AStarNode node = new AStarNode();
        AStarNode[,] nNeighbors = new AStarNode[3,3];

        node.gridX = x;
        node.gridZ = z;

        node.validSpace = walkableSpace;
        node.inCalc = false;

        nodeDict.Add(node, nNeighbors);
    }

    //Create a 2d array per node based on it's neighbors
    private void PopulateNeighbors(AStarNode node, ref AStarNode[,] nArray)
    {        
        //North
        if (node.gridZ + 1 > -1 ||
            node.gridZ + 1 < nodeGrid.dimensionsZ)
        {
            if(nodeGrid.GetGridNode(node.gridX, node.gridZ + 1).validSpace)
            {
                nArray[0, 1] = nodeGrid.GetGridNode(node.gridX, node.gridZ + 1);
            }
        }

        //East
        if (node.gridX + 1 > -1 ||
            node.gridX + 1 < nodeGrid.dimensionsX)
        {
            if (nodeGrid.GetGridNode(node.gridX, node.gridZ + 1).validSpace)
            {
                nArray[1, 0] = nodeGrid.GetGridNode(node.gridX, node.gridZ + 1);
            }
        }

        //South
        if (node.gridZ - 1 > -1 ||
            node.gridZ - 1 < nodeGrid.dimensionsZ)
        {
            if (nodeGrid.GetGridNode(node.gridX, node.gridZ + 1).validSpace)
            {
                nArray[0, -1] = nodeGrid.GetGridNode(node.gridX, node.gridZ + 1);
            }
        }

        //West
        if (node.gridX - 1 > -1 ||
            node.gridX - 1 < nodeGrid.dimensionsX)
        {
            if (nodeGrid.GetGridNode(node.gridX, node.gridZ + 1).validSpace)
            {
                nArray[-1, 0] = nodeGrid.GetGridNode(node.gridX, node.gridZ + 1);
            }
        }

        Console.WriteLine("{0},{1}",node.gridX, node.gridZ);    
    }
}