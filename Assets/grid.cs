using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tutorial Used: https://www.youtube.com/watch?v=VBZFYGWvm4A
public class grid : MonoBehaviour
{
    //First thought for resolving circular dependecies, feel free to change
    Dictionary<AStarNode, List<AStarNode>> nodeDict;
    
    public Vector3 scale = Vector2.one;
    public uint dimensionsX;
    public uint dimensionsZ;
    public GameObject gridThing;

    public void Awake()
    {
        nodeDict = new Dictionary<AStarNode, List<AStarNode>>();
    }
    
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
        for (float x = 0; x < dimensionsX; x += scale.x)
        {

            for (float z = 0; z < dimensionsZ; z += scale.z)
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
        makeGrid();
    }

    // Update is called once per frame
    void makeGrid()
    {
        for (int x = 0; x < dimensionsX; x++)
        {

            for (int z = 0; z < dimensionsZ; z++)
            {
                // var point = NearestGridPoint(new Vector3(transform.position.x + x, 0f, transform.position.z + z));
                var point = NearestGridPoint(new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z));

                //Gizmos.DrawSphere(point, scale * 0.1f);
                Instantiate(gridThing, point, gridThing.transform.rotation);

                SetGridNodePosition(x, z, true);
            }
        }

        for (int x = 0; x < dimensionsX; x++)
        {

            for (int z = 0; z < dimensionsZ; z++)
            {
                PopulateNeighbors(ref , ref );
            }
        }
    }

    //Self explanatory
    private void SetGridNodePosition(int x, int z, bool walkableSpace)
    {
        AStarNode node = new AStarNode();
        List<AStarNode> nNeighbors = new List<AStarNode>();

        node.gridX = x;
        node.gridZ = z;

        node.validSpace = walkableSpace;
        node.inCalc = false;

        nodeDict.Add(node, nNeighbors);
    }

    //Create a 2d array per node based on it's neighbors
    private void PopulateNeighbors(ref AStarNode node, ref List<AStarNode> nList)
    {        
        //North
        if (node.gridZ + 1 > -1 ||
            node.gridZ + 1 < dimensionsZ)
        {
            if(nList.Count == 0)
            {
                nList.Add()
            }
            
            if (node.validSpace &&
                !nList[node.gridX, node.gridZ + 1].inCalc)
            {
                nList[0, 1] = nList[node.gridX, node.gridZ + 1];
            }
        }

        //East
        if (node.gridX + 1 > -1 ||
            node.gridX + 1 < dimensionsX)
        {
            if (nList.Count == 0)
            {

            }
            
            if (node.validSpace &&
                !nList[node.gridX + 1, node.gridZ].inCalc)
            {
                nList[0, 1] = nList[node.gridX + 1, node.gridZ];
            }
        }

        //South
        if (node.gridZ - 1 > -1 ||
            node.gridZ - 1 < dimensionsZ)
        {
            if (nList.Count == 0)
            {

            }
            
            if (node.validSpace &&
                !nList[node.gridX, node.gridZ - 1].inCalc)
            {
                nList[0, 1] = nList[node.gridX, node.gridZ - 1];
            }
        }

        //West
        if (node.gridX - 1 > -1 ||
            node.gridX - 1 < dimensionsX)
        {
            if (nList.Count == 0)
            {

            }
            
            if (node.validSpace &&
                !nList[node.gridX - 1, node.gridZ].inCalc)
            {
                nList[0, 1] = nList[node.gridX - 1, node.gridZ];
            }
        }

        Console.WriteLine("{0},{1}",node.gridX, node.gridZ);    
    }
}