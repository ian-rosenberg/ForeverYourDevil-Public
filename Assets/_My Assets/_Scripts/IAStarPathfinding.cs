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

interface IAStarPathfinding<Character>
{
    List<AStarNode> GetNeighbors(Character c, AStarNode node);

    uint CalculateDistanceMetric(Character c, AStarNode target);

    void ExpandTarversableRange(Character c, List<AStarNode> nodeList);

    void MoveCharacter(Character c);
}
