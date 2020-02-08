using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode : MonoBehaviour
{
    private List<AStarNode> neighbors;

    public Vector2Int gridPosition;

    public uint distToTarget;
    public uint distToInitial;

    public bool validSpace;
    
    // Start is called before the first frame update
    void Awake()
    {
        gridPosition = new Vector2Int(-1, -1);

        distToTarget = 0;
        distToInitial = 0;
    }

    
}
