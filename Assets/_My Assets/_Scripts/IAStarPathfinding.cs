using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * IAstarPAthfindingInterface - Script for player movement out-ofcombat and within combat
 * 
 * Authors - Ian Rosenberg
 * 
 * Tutorials used: Random Art Attack - Custom Movement on a Grid and Path-Finding: Unity 3D Tutorial
 - https://www.youtube.com/watch?v=fUiNDDcU_I4&start=10s
 */
namespace AStarPathfinding
{
    interface IAStarPathfinding<Character>
    {
        List<AStarNode> GetNeighborsCloserToTarget(AStarNode node, AStarNode target);

        float CalculateDistanceMetric(AStarNode target);

        void ExpandTarversableRange(ref List<AStarNode> nodeList, ref int []distsToTarget);

        void MoveCharacter(AStarNode target);
    }
}