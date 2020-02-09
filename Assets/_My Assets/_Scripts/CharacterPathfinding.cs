using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStarPathfinding;

public class CharacterPathfinding<Character> : IAStarPathfinding<Character>
{

    private Character c;

    private void SetCharacter(Character cr)
    {
        c = cr;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Returns a list of adjacent nodes(N,S,E,W)
    public List<AStarNode> GetNeighborsCloserToTarget(AStarNode node, AStarNode target)
    {
        
        
        return null;
    }

    //Determine the distance to travel to a specified target
    public float CalculateDistanceMetric(AStarNode target)
    {


            return 0;
    }

    //Discover
    public void ExpandTarversableRange(ref List<AStarNode> nodeList, ref int []distsToTarget)
    {

    }

    //
    public void MoveCharacter(AStarNode target)
    {

    }
}
