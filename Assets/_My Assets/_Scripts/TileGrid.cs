using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tutorial Used: https://www.youtube.com/watch?v=VBZFYGWvm4A
public class TileGrid : MonoBehaviour
{
    public const int homeFromNeighborValue = 1;

    public AStarNode [,]nodeGrid;

    public uint dimensionsX = 8;
    public uint dimensionsZ = 8;

    public Vector3 defaultSpawn; 

    public Vector3 scale = Vector2.one;
    public GameObject gridThing;
    public GameObject highlighter;


    private int idCounter;


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

    public AStarNode NearestGridNode(Vector3 position)
    {      
        float closest = 10000000;
        int x = 0, y = 0;

        for(int i = 0; i < dimensionsZ; i++)
        {
            for (int j = 0; j < dimensionsX; j++)
            {
                float newVal = Vector3.Distance(position, nodeGrid[i, j].worldPosition);

                if (newVal < closest) 
                {
                    closest = newVal;
                    x = j;
                    y = i;
                }
            }
        }

        return nodeGrid[y, x];
    }

    //Draw the grid in Editor. 
    private void OnDrawGizmos()
    {
        Vector3 s = gridThing.gameObject.transform.localScale;

        Gizmos.color = Color.red;
        for (float z = 0; z < dimensionsZ; z++ )
        {
            for (float x = 0; x < dimensionsX; x++)
            {
                // var point = NearestGridPoint(new Vector3(transform.position.x + x, 0f, transform.position.z + z));
                var point = new Vector3(transform.position.x + x * s.x, transform.position.y, transform.position.z + z * s.z);
                Gizmos.DrawSphere(point, 0.5f);
                //Gizmos.DrawWireCube(point, new Vector3(scale.x, scale.y, scale.z));
            }
        }
    }

    void Awake()
    {
        nodeGrid = new AStarNode[dimensionsZ, dimensionsX];

        defaultSpawn = this.transform.position;

        makeGrid();
    }

    public void HighlightPath(List<Vector2> path)
    {
        foreach(Vector2 p in path)
        {
            NearestGridNode(p).Highlight(true);
        }
    }
    public void RemoveHighlightedPath(List<Vector2> prevPath)
    {
        foreach (Vector2 p in prevPath)
        {
            NearestGridNode(p).Highlight(false);
        }
    }

    // Update is called once per frame
    public void makeGrid()
    {
        Vector3 s = gridThing.gameObject.transform.localScale;
        Gizmos.color = Color.red;
        Vector3 spot = new Vector3(-100, -100, -100);

        for (int z = 0; z < dimensionsZ; z++)
        {
            for (int x = 0; x < dimensionsX; x++)
            {
                // var point = NearestGridPoint(new Vector3(transform.position.x + x, 0f, transform.position.z + z));
                // var point = NearestGridPoint(new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z));
                var point = new Vector3(transform.position.x + x*s.x, transform.position.y, transform.position.z + z*s.z);

                //Gizmos.DrawSphere(point, scale * 0.1f);
                var clone = Instantiate(gridThing, spot, gridThing.transform.rotation);

                SetGridNodePosition(x, z, true, point, clone);
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
    public void SetGridNodePosition(int x, int z, bool walkableSpace, Vector3 p, GameObject clone)
    {
        AStarNode node = new AStarNode();

        node.SetCoords(x, z, Instantiate(highlighter, p, highlighter.transform.rotation));

        node.validSpace = walkableSpace;
        node.worldPosition = p;

        node.id = idCounter++;

        node.SetGlowSpot(clone);

        nodeGrid[z, x] = node;
    }
 
}