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
    interface IAStarPathfinding
    {
        bool GetNeighborsCloserToTarget(ref List<AStarNode> neighbors, AStarNode node, TileGrid grid);

        AStarNode EliminateFarNodes(ref List<AStarNode> nodes, TileGrid grid, ref List<float> distsToTarget);

        //float CalculateDistanceMetric(AStarNode current, AStarNode target, TileGrid grid, ref List<float> distsToTarget);

        void MoveCharacter(AStarNode start, AStarNode goal, TileGrid grid);

        float GetDistanceCoveredPct(AStarNode current, AStarNode next);

        float GetDistance(AStarNode current, AStarNode nxt);
    }
}