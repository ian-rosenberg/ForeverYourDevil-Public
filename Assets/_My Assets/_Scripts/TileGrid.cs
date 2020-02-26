using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Tutorial Used: https://www.youtube.com/watch?v=VBZFYGWvm4A
public class TileGrid : MonoBehaviour
{
    [Header("Debug")]
    public bool showPoints;
    public bool showWireframe;

    public const int homeFromNeighborValue = 1;

    public AStarNode[,] nodeGrid;

    [Header("Properties")]
    public uint dimensionsX = 8;
    public uint dimensionsZ = 8;

    public Vector3 defaultSpawn;

    public Vector3 scale = Vector2.one;
    public GameObject gridThing;
    public GameObject highlighter;
    public Bounds bounds;

    private int idCounter;

    //Get nearest grid point to whichever position is specified (ideally mouse position)
    public AStarNode NearestGridNode(Vector3 position)
    {
        float closest = Mathf.Infinity;
        int x = 0, y = 0;

        for (int i = 0; i < dimensionsZ; i++)
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
        Vector3 s = gridThing.transform.localScale;
        Vector3 extents = gridThing.GetComponentInChildren<MeshRenderer>().bounds.extents;

        Gizmos.color = Color.red;
        for (float z = 0; z < dimensionsZ; z++)
        {
            for (float x = 0; x < dimensionsX; x++)
            {
                var point = new Vector3(transform.position.x + x * s.x + x, transform.position.y, transform.position.z + z * s.z);//multiply by local scale

                //var point = NearestGridPoint(new Vector3(transform.position.x + x * extents.x*s.x, transform.position.y, transform.position.z + z * extents.z*s.z));
                //var point = new Vector3(transform.position.x * x + extents.x * scale.x, transform.position.y, transform.position.z * z + extents.z * scale.z);
                //Gizmos.DrawCube(point, scale);
                if (showPoints) Gizmos.DrawSphere(point, .5f);
                if (showWireframe) Gizmos.DrawWireCube(point, new Vector3(scale.x, scale.y, scale.z));
            }
        }
    }

    void Awake()
    {
        nodeGrid = new AStarNode[dimensionsZ, dimensionsX];

        defaultSpawn = transform.position;

        bounds = gridThing.GetComponentInChildren<MeshRenderer>().bounds;

        makeGrid();
    }

    //Omar's new Highlight path
    public void HighlightPath(List<AStarNode> path)
    {
        foreach (AStarNode p in path)
        {
            p.Highlight(true);
        }
    }

    public void RemoveHighlights()
    {
        for (int z = 0; z < dimensionsZ; z++)
        {
            for (int x = 0; x < dimensionsX; x++)
            {
                nodeGrid[z, x].Highlight(false);
            }
        }
    }

    // Update is called once per frame
    public void makeGrid()
    {
        Vector3 s = gridThing.transform.localScale;
        Gizmos.color = Color.red;
        Vector3 extents = bounds.extents;

        for (int z = 0; z < dimensionsZ; z++)
        {
            for (int x = 0; x < dimensionsX; x++)
            {
                var point = new Vector3(transform.position.x + x * s.x + x, transform.position.y, transform.position.z + z * s.z + z);//multiply by local scale
                var clone = Instantiate(gridThing, point, gridThing.transform.rotation);

                SetGridNodePosition(x, z, true, point + bounds.center, clone, bounds.center);
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
    public void SetGridNodePosition(int x, int z, bool walkableSpace, Vector3 p, GameObject clone, Vector3 centerOffset)
    {
        AStarNode node = new AStarNode();

        GameObject hClone = Instantiate(highlighter, p + new Vector3(3f, 300, 3f), highlighter.transform.rotation);
        node.SetCoords(x, z, hClone);

        node.validSpace = walkableSpace;
        node.worldPosition = p - new Vector3(centerOffset.x, centerOffset.y, centerOffset.z);

        node.id = idCounter++;

        nodeGrid[z, x] = node;
    }

}