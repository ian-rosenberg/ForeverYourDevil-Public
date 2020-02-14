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

    private float pctToNextTile;
    private bool startedTravel;

    private void Start()
    {
        path = new List<AStarNode>();
    }

    public void SetObj(GameObject o)
    {
        obj = o;
    }

    //Returns a list of adjacent nodes(N,S,E,W)
    public bool GetNeighborsCloserToTarget(ref List<AStarNode> neighbors, ref AStarNode node, TileGrid grid)
    {

        Debug.Log(node.gridX + "," + node.gridZ);
        
        //Check for northern neighbor
        if(node.neighbors[1,2] != null)
        {
            if (node.neighbors[1, 2].validSpace)
            {
                neighbors.Add(node.neighbors[1, 2]);
            }
        }

        //Check for eastern neighbor
        if (node.neighbors[2, 1] != null)
        {
            if (node.neighbors[2, 1].validSpace)
            {
                neighbors.Add(node.neighbors[2, 1]);
            }
        }

        //Check for southern neighbor
        if (node.neighbors[1, 0] != null)
        {
            if (node.neighbors[1, 0].validSpace)
            {
                neighbors.Add(node.neighbors[1, 0]);
            }
        }

        //Check for western neighbor
        if (node.neighbors[0, 1] != null)
        {
            if (node.neighbors[0, 1].validSpace)
            {
                neighbors.Add(node.neighbors[0, 1]);
            }
        }

        if(neighbors.Contains(target))
        {
            List<AStarNode> goal = new List<AStarNode>();
            goal.Add(target);

            neighbors.Clear();
            neighbors = goal;

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
    public List<AStarNode> MoveCharacter(AStarNode s, AStarNode goal, TileGrid grid)
    {
        List<AStarNode> neighbors = new List<AStarNode>();
        start = s;
        current = start;
        pctToNextTile = 0f;
        target = goal;
        startedTravel = false;

        Debug.Log("Beginning character movement, character is @ (" + start.gridX + "," + start.gridZ + ")");
       
        path.Add(start);

        do
        {

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
            
            if(GetNeighborsCloserToTarget(ref neighbors, ref current, grid))
            {

                obj.GetComponent<PlayerController>().ToggleAutoMove();

                foreach (AStarNode n in path)
                {
                    obj.GetComponent<PlayerController>().xzPath.Add(new Vector2Int(n.gridX, n.gridZ));
                }
                break;
            }
            next = EliminateFarNodes(ref neighbors, grid, ref dists);
            path.Add(next);

        } while (current != target);

        Debug.Log("<color=cyan>"+path[(path.Count - 1)]+"</color>" );

        return path;
    }

    public AStarNode EliminateFarNodes(ref List<AStarNode> nodes, TileGrid grid, ref Dictionary<uint, float> distsToTarget)
    {
        List<AStarNode> potentialKeepers = new List<AStarNode>();
        AStarNode closest;
        float cur = 0f;
        int index = 0;

        cur = GetDistance(start, nodes[0]);

        for (int i = 0; i < nodes.Count; i++)
        {
            if(GetDistance(start, nodes[i]) <= cur)
            {
                potentialKeepers.Add(nodes[i]);

                distsToTarget.Add((uint)i,GetDistance(start, nodes[i]));
            }
        }

        closest = potentialKeepers[0];

        foreach(KeyValuePair<uint, float> nodeDist in distsToTarget)
        {
            if(nodeDist.Value <= cur)
            {
                cur = nodeDist.Value;
                index = (int)nodeDist.Key;
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
