using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AStarPathfinding;

public class CharacterPathfinding : MonoBehaviour
{
    public TileGrid grid;

    private AStarNode start;
    private AStarNode target;

    public bool InGrid(int x, int y)
    {
        return x >= 0 && x < grid.dimensionsX && y >= 0 && y < grid.dimensionsZ;
    }

    public bool IsGoal(AStarNode coord)
    {
        return coord.gridX == target.gridX && coord.gridZ == target.gridZ;
    }
    public List<Vector2> TracePath()
    {
        List<Vector2> pathXZ = new List<Vector2>();//points for nodes for character to follow
        int curRow = target.gridZ;
        int curCol = target.gridX;
        Stack<AStarNode> path = new Stack<AStarNode>();
        AStarNode cur = target;
        AStarNode prev = null;
        List<Vector2> l = new List<Vector2>();//list of the paths to take, which will be reversed

        Debug.Log("<color=red>Path found: </color>");

        /*while (!(grid.nodeGrid[curRow, curCol].parent.gridZ == curRow &&
            grid.nodeGrid[curRow, curCol].parent.gridX == curCol))
        {
            path.Push(grid.nodeGrid[curRow, curCol]);
            curCol = grid.nodeGrid[curRow, curCol].parent.gridX;
            curRow = grid.nodeGrid[curRow, curCol].parent.gridZ;
        }*/

        l.Add(new Vector2(cur.gridX, cur.gridZ));

        do
        {
            prev = cur.parent;

            cur = prev.parent;

            l.Add(new Vector2(cur.gridX, cur.gridZ));

        }while(prev != null)

        path.Push(grid.nodeGrid[curRow, curCol]);

        while (!(path.Count == 0))
        {
            AStarNode n = path.Pop();

            Debug.Log("<color=purple> -> " + n.gridX + "," + n.gridZ + "</color>");

            pathXZ.Add(new Vector2(n.gridX, n.gridZ));
        }

        return pathXZ;
    }

    public List<Vector2> AStarSearch(AStarNode s, AStarNode goal)
    {
        List<AStarNode> openPath = new List<AStarNode>(); // all possible tiles that lead to the target
        bool[,] closedList = new bool[grid.dimensionsZ, grid.dimensionsX];
        bool done = false;
        int i = s.gridZ, j = s.gridX;

        start = s;
        target = goal;

        Debug.Log("<color=blue>Start:(" + s.gridX + "," + s.gridZ + ")</color>");
        Debug.Log("<color=green>Goal:(" + goal.gridX + "," + goal.gridZ + ")</color>");

        if (IsGoal(start))
        {
            Debug.Log("We der alreadyyy");

            return null;
        }

        if (!InGrid(start.gridX, start.gridZ) || !InGrid(target.gridX,target.gridZ))
        {
            Debug.Log("Node not on grid");

            return null;
        }

        if (!start.validSpace || !target.validSpace)
        {
            Debug.Log("Invalid space, it blocked u dummy");

            return null;
        }

        for(int inZ = 0; inZ < grid.dimensionsZ; inZ++)
        {
            for (int inX = 0; inX < grid.dimensionsX; inX++)
            {
                closedList[inZ, inX] = false;
            }
        }


        openPath.Add(start);

        while (openPath.Count != 0)
        {
            //Reset node costs
            for (int ii = 0; ii < grid.dimensionsZ; ii++)
            {
                for (int jj = 0; jj < grid.dimensionsX; jj++)
                {
                    grid.nodeGrid[ii, jj].fCost = float.MaxValue;
                    grid.nodeGrid[ii, jj].gCost = float.MaxValue;
                    grid.nodeGrid[ii, jj].hCost = float.MaxValue;
                }
            }

            AStarNode node = openPath[0];
            i = node.gridZ;
            j = node.gridX;

            openPath.RemoveAt(0);
            closedList[node.gridZ, node.gridX] = true;

            float gNew = 0f, hNew = 0f, fNew = 0f;

            ///////////////////////////////////////////////////////////
            ///North
            ///////////////////////////////////////////////////////////
            if (InGrid(i - 1, j))
            {
                if (grid.nodeGrid[i - 1, j].validSpace)
                {
                    if (IsGoal(grid.nodeGrid[i - 1, j]))
                    {
                        grid.nodeGrid[i - 1, j].parent = grid.nodeGrid[i, j];
                        Debug.Log("<color=green>Destination found! </color>" + j + "," + (i - 1));
                        return TracePath();
                    }
                }
                else if (InGrid(i - 1, j) && closedList[i - 1, j] == false &&
                    grid.nodeGrid[i - 1, j].validSpace)
                {
                    gNew = grid.nodeGrid[i, j].gCost + 1.0f;
                    hNew = Mathf.Max(Mathf.Abs(grid.nodeGrid[i, j].gridX - target.gridX), Mathf.Abs(grid.nodeGrid[i, j].gridZ - target.gridZ));
                    fNew = gNew + hNew;

                    if (grid.nodeGrid[i - 1, j].fCost == float.MaxValue ||
                        grid.nodeGrid[i - 1, j].fCost > fNew)
                    {
                        openPath.Add(grid.nodeGrid[i - 1, j]);

                        grid.nodeGrid[i - 1, j].fCost = fNew;
                        grid.nodeGrid[i - 1, j].gCost = gNew;
                        grid.nodeGrid[i - 1, j].hCost = hNew;

                        grid.nodeGrid[i - 1, j].parent = grid.nodeGrid[i, j];
                    }
                }
            }

            ///////////////////////////////////////////////////////////
            //South
            ///////////////////////////////////////////////////////////
            if (InGrid(i + 1, j))
            {
                if (grid.nodeGrid[i + 1, j].validSpace)
                {
                    if (IsGoal(grid.nodeGrid[i + 1, j]))
                    {
                        grid.nodeGrid[i + 1, j].parent = grid.nodeGrid[i, j];
                        Debug.Log("<color=green>Destination found! </color>" + j + "," + (i + 1));
                        return TracePath();
                    }
                }
                else if (closedList[i + 1, j] == false &&
                    grid.nodeGrid[i + 1, j].validSpace)
                {
                    gNew = grid.nodeGrid[i, j].gCost + 1.0f;
                    hNew = Mathf.Max(Mathf.Abs(grid.nodeGrid[i, j].gridX - target.gridX), Mathf.Abs(grid.nodeGrid[i, j].gridZ - target.gridZ));
                    fNew = gNew + hNew;

                    if (grid.nodeGrid[i + 1, j].fCost == float.MaxValue ||
                        grid.nodeGrid[i + 1, j].fCost > fNew)
                    {
                        openPath.Add(grid.nodeGrid[i + 1, j]);

                        grid.nodeGrid[i + 1, j].fCost = fNew;
                        grid.nodeGrid[i + 1, j].gCost = gNew;
                        grid.nodeGrid[i + 1, j].hCost = hNew;

                        grid.nodeGrid[i + 1, j].parent = grid.nodeGrid[i, j];
                    }
                }
            }

            ///////////////////////////////////////////////////////////
            ///East
            ///////////////////////////////////////////////////////////
            if (InGrid(i, j + 1))
            {
                if (grid.nodeGrid[i, j + 1].validSpace)
                {
                    if (IsGoal(grid.nodeGrid[i, j + 1]))
                    {
                        grid.nodeGrid[i, j + 1].parent = grid.nodeGrid[i, j];
                        Debug.Log("<color=green>Destination found! </color>" + (j + 1) + "," + i);
                        return TracePath();
                    }
                }
                else if (closedList[i, j + 1] == false &&
                    grid.nodeGrid[i, j + 1].validSpace)
                {
                    gNew = grid.nodeGrid[i, j].gCost + 1.0f;
                    hNew = Mathf.Max(Mathf.Abs(grid.nodeGrid[i, j].gridX - target.gridX), Mathf.Abs(grid.nodeGrid[i, j].gridZ - target.gridZ));
                    fNew = gNew + hNew;

                    if (grid.nodeGrid[i, j + 1].fCost == float.MaxValue ||
                        grid.nodeGrid[i, j + 1].fCost > fNew)
                    {
                        openPath.Add(grid.nodeGrid[i, j + 1]);

                        grid.nodeGrid[i, j + 1].fCost = fNew;
                        grid.nodeGrid[i, j + 1].gCost = gNew;
                        grid.nodeGrid[i, j + 1].hCost = hNew;

                        grid.nodeGrid[i, j + 1].parent = grid.nodeGrid[i, j];
                    }
                }
            }

            ///////////////////////////////////////////////////////////
            ///West
            ///////////////////////////////////////////////////////////
            if (InGrid(i, j - 1))
            {
                if (grid.nodeGrid[i, j - 1].validSpace)
                {
                    if (IsGoal(grid.nodeGrid[i, j - 1]))
                    {
                        grid.nodeGrid[i, j - 1].parent = grid.nodeGrid[i, j];
                        Debug.Log("<color=green>Destination found! </color>" + (j - 1) + "," + i);
                        return TracePath();
                    }
                }
                else if (closedList[i, j - 1] == false &&
                    grid.nodeGrid[i, j - 1].validSpace)
                {
                    gNew = grid.nodeGrid[i, j].gCost + 1.0f;
                    hNew = Mathf.Max(Mathf.Abs(grid.nodeGrid[i, j].gridX - target.gridX), Mathf.Abs(grid.nodeGrid[i, j].gridZ - target.gridZ));
                    fNew = gNew + hNew;

                    if (grid.nodeGrid[i, j - 1].fCost == float.MaxValue ||
                        grid.nodeGrid[i, j - 1].fCost > fNew)
                    {
                        openPath.Add(grid.nodeGrid[i, j - 1]);

                        grid.nodeGrid[i, j - 1].fCost = fNew;
                        grid.nodeGrid[i, j - 1].gCost = gNew;
                        grid.nodeGrid[i, j - 1].hCost = hNew;

                        grid.nodeGrid[i, j - 1].parent = grid.nodeGrid[i, j];
                    }
                }
            }

            ///////////////////////////////////////////////////////////
            ///North-East
            ///////////////////////////////////////////////////////////
            if (InGrid(i - 1, j + 1))
            {
                if (grid.nodeGrid[i - 1, j + 1].validSpace)
                {
                    if (IsGoal(grid.nodeGrid[i - 1, j + 1]))
                    {
                        grid.nodeGrid[i - 1, j + 1].parent = grid.nodeGrid[i, j];
                        Debug.Log("<color=green>Destination found! </color>" + (j - 1) + "," + (i + 1));
                        return TracePath();
                    }
                }
                else if (closedList[i - 1, j + 1] == false &&
                    grid.nodeGrid[i - 1, j + 1].validSpace)
                {
                    gNew = grid.nodeGrid[i, j].gCost + 1.0f;
                    hNew = Mathf.Max(Mathf.Abs(grid.nodeGrid[i, j].gridX - target.gridX), Mathf.Abs(grid.nodeGrid[i, j].gridZ - target.gridZ));
                    fNew = gNew + hNew;

                    if (grid.nodeGrid[i - 1, j + 1].fCost == float.MaxValue ||
                        grid.nodeGrid[i - 1, j + 1].fCost > fNew)
                    {
                        openPath.Add(grid.nodeGrid[i - 1, j + 1]);

                        grid.nodeGrid[i - 1, j + 1].fCost = fNew;
                        grid.nodeGrid[i - 1, j + 1].gCost = gNew;
                        grid.nodeGrid[i - 1, j + 1].hCost = hNew;

                        grid.nodeGrid[i - 1, j + 1].parent = grid.nodeGrid[i, j];
                    }
                }
            }

            ///////////////////////////////////////////////////////////
            ///North-West
            ///////////////////////////////////////////////////////////
            if (InGrid(i - 1, j - 1))
            {
                if (grid.nodeGrid[i - 1, j - 1].validSpace)
                {
                    if (IsGoal(grid.nodeGrid[i - 1, j - 1]))
                    {
                        grid.nodeGrid[i - 1, j - 1].parent = grid.nodeGrid[i, j];
                        Debug.Log("<color=green>Destination found! </color>" + j + "," + i);
                        return TracePath();
                    }
                }
                else if (closedList[i - 1, j - 1] == false &&
                    grid.nodeGrid[i - 1, j - 1].validSpace)
                {
                    gNew = grid.nodeGrid[i, j].gCost + 1.0f;
                    hNew = Mathf.Max(Mathf.Abs(grid.nodeGrid[i, j].gridX - target.gridX), Mathf.Abs(grid.nodeGrid[i, j].gridZ - target.gridZ));
                    fNew = gNew + hNew;

                    if (grid.nodeGrid[i - 1, j - 1].fCost == float.MaxValue ||
                        grid.nodeGrid[i - 1, j - 1].fCost > fNew)
                    {
                        openPath.Add(grid.nodeGrid[i - 1, j - 1]);

                        grid.nodeGrid[i - 1, j - 1].fCost = fNew;
                        grid.nodeGrid[i - 1, j - 1].gCost = gNew;
                        grid.nodeGrid[i - 1, j - 1].hCost = hNew;

                        grid.nodeGrid[i - 1, j - 1].parent = grid.nodeGrid[i, j];
                    }
                }
            }

            ///////////////////////////////////////////////////////////
            ///South-East
            ///////////////////////////////////////////////////////////
            if (InGrid(i + 1, j + 1))
            {
                if (grid.nodeGrid[i + 1, j + 1].validSpace)
                {
                    if (IsGoal(grid.nodeGrid[i + 1, j + 1]))
                    {
                        grid.nodeGrid[i + 1, j + 1].parent = grid.nodeGrid[i, j];
                        Debug.Log("<color=green>Destination found! </color>" + (j + 1) + "," + (i + 1));
                        return TracePath();
                    }
                }
                else if (closedList[i + 1, j + 1] == false &&
                    grid.nodeGrid[i + 1, j + 1].validSpace)
                {
                    gNew = grid.nodeGrid[i, j].gCost + 1.0f;
                    hNew = Mathf.Max(Mathf.Abs(grid.nodeGrid[i, j].gridX - target.gridX), Mathf.Abs(grid.nodeGrid[i, j].gridZ - target.gridZ));
                    fNew = gNew + hNew;

                    if (grid.nodeGrid[i + 1, j + 1].fCost == float.MaxValue ||
                        grid.nodeGrid[i + 1, j + 1].fCost > fNew)
                    {
                        openPath.Add(grid.nodeGrid[i + 1, j + 1]);

                        grid.nodeGrid[i + 1, j + 1].fCost = fNew;
                        grid.nodeGrid[i + 1, j + 1].gCost = gNew;
                        grid.nodeGrid[i + 1, j + 1].hCost = hNew;

                        grid.nodeGrid[i + 1, j + 1].parent = grid.nodeGrid[i, j];
                    }
                }
            }

            ///////////////////////////////////////////////////////////
            ///South-West
            ///////////////////////////////////////////////////////////
            if (InGrid(i + 1, j - 1))
            {
                if (grid.nodeGrid[i + 1, j - 1].validSpace)
                {
                    if (IsGoal(grid.nodeGrid[i + 1, j - 1]))
                    {
                        grid.nodeGrid[i + 1, j - 1].parent = grid.nodeGrid[i, j];
                        Debug.Log("<color=green>Destination found! </color>" + j + "," + i);
                        return TracePath();
                    }
                }
                else if (closedList[i + 1, j - 1] == false &&
                    grid.nodeGrid[i + 1, j - 1].validSpace)
                {
                    gNew = grid.nodeGrid[i, j].gCost + 1.0f;
                    hNew = Mathf.Max(Mathf.Abs(grid.nodeGrid[i, j].gridX - target.gridX), Mathf.Abs(grid.nodeGrid[i, j].gridZ - target.gridZ));
                    fNew = gNew + hNew;

                    if (grid.nodeGrid[i + 1, j - 1].fCost == float.MaxValue ||
                        grid.nodeGrid[i + 1, j - 1].fCost > fNew)
                    {
                        openPath.Add(grid.nodeGrid[i + 1, j - 1]);

                        grid.nodeGrid[i + 1, j - 1].fCost = fNew;
                        grid.nodeGrid[i + 1, j - 1].gCost = gNew;
                        grid.nodeGrid[i + 1, j - 1].hCost = hNew;

                        grid.nodeGrid[i + 1, j - 1].parent = grid.nodeGrid[i, j];
                    }
                }
            }
        }

        if(!done)
        {
            Debug.Log("<color=red>FAILURE TO FIND PATH</color>");
        }

        return null;
    }
}
