using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AStarPathfinding;

public class CharacterPathfinding : MonoBehaviour
{
    private GameObject obj;

    private AStarNode start;
    private AStarNode current;
    private AStarNode target;
    private AStarNode next;

    private List<AStarNode> path; // The most efficient path
    private List<AStarNode> openPath; // all possible tiles that lead to the target
    private List<AStarNode> closedPath;

    //private float pctToNextTile;
    //private bool startedTravel;

    private void Start()
    {
        path = new List<AStarNode>();
    }

    public void SetObj(GameObject o)
    {
        obj = o;
    }

    //Returns a list of adjacent nodes(N,S,E,W)
    public bool GetNeighborsCloserToTarget(List<AStarNode> neighbors, ref AStarNode node, TileGrid grid)
    {

        //Check for northern neighbor
        if(node.neighbors[1,2] != null)
        {
            if (node.neighbors[1, 2].validSpace && !node.neighbors[1, 2].inCalc)
            {
                neighbors.Add(node.neighbors[1, 2]);
                node.neighbors[1, 2].inCalc = true;
            }
        }

        //Check for eastern neighbor
        if (node.neighbors[2, 1] != null)
        {
            if (node.neighbors[2, 1].validSpace && !node.neighbors[2, 1].inCalc)
            {
                neighbors.Add(node.neighbors[2, 1]);
                node.neighbors[2, 1].inCalc = true;
            }
        }

        //Check for southern neighbor
        if (node.neighbors[1, 0] != null)
        {
            if (node.neighbors[1, 0].validSpace && !node.neighbors[1, 0].inCalc)
            {
                neighbors.Add(node.neighbors[1, 0]);
                node.neighbors[1, 0].inCalc = true;
            }
        }

        //Check for western neighbor
        if (node.neighbors[0, 1] != null)
        {
            if (node.neighbors[0, 1].validSpace && !node.neighbors[0, 1].inCalc)
            {
                neighbors.Add(node.neighbors[0, 1]);
                node.neighbors[1, 0].inCalc = true;
            }
        }

        if(neighbors.Contains(target))
        {
            path.Add(target);

            return true;
        }

        return false;
    }

    //Determine the distance to travel to a specified target
    /*public float CalculateDistanceMetric(AStarNode current, AStarNode target, TileGrid grid, ref Dictionary<uint, float> distsToTarget)
    {
        return 0;
    }*/

    //Actual implementation of algorithim
    public List<AStarNode> GetCharacterPath(AStarNode s, AStarNode goal, TileGrid grid)
    {
        start = s;
        current = start;
        //pctToNextTile = 0f;
        target = goal;
        //startedTravel = false;

        Debug.Log("Beginning character movement, character is @ (" + start.gridX + "," + start.gridZ + ")");

        path.Add(start);

        do
        {
            List<AStarNode> neighbors = new List<AStarNode>();
            //this if hates me as well as Unity
            // Basically we don't move and the loop never progresses
            //must be an issue with the navmeshagent?
            /*if(loopStarted || pctToNextTile > 10f)
            {
                if (!startedTravel)
                {
                    obj.GetComponent<NavMeshAgent>().SetDestination(next.worldPosition);

                    startedTravel = true;
                }


                pctToNextTile = GetDistanceCoveredPct(current, next);
                Debug.Log("Percentage of distance to next waypoint covered: (" + pctToNextTile + ")");
                Debug.Log("Current tile position: (" + current.worldPosition + ")");

                if(pctToNextTile < 10f)
                {
                    loopStarted = false;
                }
                continue;
            }*/

            Debug.Log("Looping");

            Dictionary<uint, float> dists = new Dictionary<uint, float>();

            if (GetNeighborsCloserToTarget(neighbors, ref current, grid))
            {
                obj.GetComponent<PlayerController>().ToggleAutoMove();

                foreach (AStarNode n in path)
                {
                    obj.GetComponent<PlayerController>().xzPath.Add(new Vector2Int(n.gridX, n.gridZ));
                }
                break;
            }
            next = EliminateFarNodes(neighbors, grid, ref dists);

            if (next == null)
            {
                return path;
            }

            path.Add(next);

        } while (current != target);

        foreach (AStarNode n in path)
        {
            Debug.Log("<color=cyan>" + n.gridX+","+n.gridZ + "</color>");
        }

        for(int z = 0; z < grid.dimensionsZ; z++)
        { 
            for (int x = 0; x < grid.dimensionsX; x++)
            {
                grid.nodeGrid[z, x].inCalc = false;
            }
        }

        return path;
    }

    public AStarNode EliminateFarNodes(List<AStarNode> nodes, TileGrid grid, ref Dictionary<uint, float> distsToTarget)
    {
        List<AStarNode> potentialKeepers = new List<AStarNode>();
        AStarNode closest;
        float curVal = 0f;
        int index = 0;

        for (int i = 0; i < nodes.Count; i++)
        {
            float dist = GetDistance(start, nodes[i]);


            if (GetDistance(start, nodes[i]) <= curVal)
            { 
                curVal = nodes[i].distToTarget;

                potentialKeepers.Add(nodes[i]);

                distsToTarget.Add((uint)i,dist);
            }
        }

        if(potentialKeepers.Count == 0)
        {
            return null;
        }

        closest = potentialKeepers[0];

        foreach(KeyValuePair<uint, float> nodeDist in distsToTarget)
        {
            if(nodeDist.Value <= closest.distToTarget)
            {
                index = (int)nodeDist.Key;
            }
        }

        foreach(AStarNode node in nodes)
        {
            if(node != closest)
            {
                node.inCalc = false;
            }
        }

        return nodes[index];
    }

    public float GetDistanceCoveredPct(AStarNode current, AStarNode nxt)
    {
        float distBetween = Vector3.Distance(new Vector3(current.gridX, 0, current.gridZ), new Vector3(nxt.gridX, 0, nxt.gridZ));

        float charDist = Vector3.Distance(new Vector3(obj.GetComponent<Transform>().position.x, 0, obj.GetComponent<Transform>().position.z),
            new Vector3(nxt.gridX, 0, nxt.gridZ));

        return charDist / distBetween;
    }

    public float GetDistance(AStarNode current, AStarNode nxt)
    {
        current.distToInitial = Vector3.Distance(new Vector3(start.gridX, 0, start.gridZ), new Vector3(target.gridX, 0, target.gridZ));
        current.distToTarget = Vector3.Distance(new Vector3(current.gridX, 0, current.gridZ), new Vector3(target.gridX, 0, target.gridZ));

        return Mathf.Abs(Vector3.Distance(new Vector3(current.gridX, 0, current.gridZ), new Vector3(nxt.gridX, 0, nxt.gridZ)));
    }
}
