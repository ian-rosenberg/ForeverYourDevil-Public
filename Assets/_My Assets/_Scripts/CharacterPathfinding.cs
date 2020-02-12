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

            return true;
        }

        return true;
    }

    //Determine the distance to travel to a specified target
    /*public float CalculateDistanceMetric(AStarNode current, AStarNode target, TileGrid grid, ref List<float> distsToTarget)
    {
        return 0;
    }*/

    //Actual implementation of algorithim
    public void MoveCharacter(AStarNode s, AStarNode goal, TileGrid grid)
    {
        List<AStarNode> neighbors = new List<AStarNode>();
        bool loopStarted = false;
        start = s;
        current = start;
        pctToNextTile = 0f;
        target = goal;
        

        do
        {
            if(loopStarted || pctToNextTile > 0.10f)
            {
                pctToNextTile = GetDistanceCoveredPct(current, next);
                continue;
            }

            loopStarted = true;
            
            List<float> dists = new List<float>();
            
            if(GetNeighborsCloserToTarget(ref neighbors, ref current, grid))
            {   
               break;
            }
            next = EliminateFarNodes(ref neighbors, grid, ref dists);

            obj.GetComponent<NavMeshAgent>().SetDestination(new Vector3(next.gridX + grid.gridThing.GetComponent<Renderer>().bounds.extents.x/2, obj.transform.position.y, next.gridZ + grid.gridThing.GetComponent<Renderer>().bounds.extents.z / 2));
        } while (current != target);

    }

    public AStarNode EliminateFarNodes(ref List<AStarNode> nodes, TileGrid grid, ref List<float> distsToTarget)
    {
        float cur = 1000000000;
        List<AStarNode> potentialKeepers = new List<AStarNode>();
        List<AStarNode> toReturn = new List<AStarNode>();
        AStarNode closest;

        for(int i = 0; i < nodes.Count; i++)
        {
            if(GetDistance(start, nodes[i]) <= cur)
            {
                potentialKeepers.Add(nodes[i]);

                cur = GetDistance(start, nodes[i]);
            }
        }

        closest = potentialKeepers[0];

        for (int i = 0; i < potentialKeepers.Count; i++)
        {
            if(i+1 < potentialKeepers.Count)
            {
                if(closest.distToTarget < potentialKeepers[i].distToTarget)
                {
                    closest = potentialKeepers[i];
                }
            }
        }

        return closest;
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
