﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AStarPathfinding;

public class CharacterPathfinding : MonoBehaviour
{

    [Header("Debug")]
    public bool printPath;

    [Header("Pathfinding")]
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

    //Ian's Original Tracepath (not in use in AStarPolish)
    /*    public List<Vector2> TracePath()
        {
            List<Vector2> pathXZ = new List<Vector2>();//points for nodes for character to follow
            int curRow = target.gridZ;
            int curCol = target.gridX;
            Stack<AStarNode> path = new Stack<AStarNode>();
            AStarNode cur = target;



            path.Push(cur);

            while (cur != start)
            {
                path.Push(cur.parent);
                cur = cur.parent;
            }

            Debug.Log("<color=red>Path found: </color>");

            while (!(path.Count == 0))
            {
                AStarNode n = path.Pop();

                if (path.Count > 0)
                {
                    Debug.Log("<color=purple>" + n.gridX + "," + n.gridZ + " -> </color>");
                }
                else
                {
                    Debug.Log("<color=purple>" + n.gridX + "," + n.gridZ + "</color>");
                }
                pathXZ.Add(new Vector2(n.gridX, n.gridZ));
            }

            return pathXZ;
        }*/

    //Omar's Edited TracePath
    public List<AStarNode> TracePath()
    {
        List<AStarNode> pathXZ = new List<AStarNode>();//points for nodes for character to follow
        int curRow = target.gridZ;
        int curCol = target.gridX;
        Stack<AStarNode> path = new Stack<AStarNode>();
        AStarNode cur = target;

        path.Push(cur);

        while (cur != start)
        {
            path.Push(cur.parent);
            cur = cur.parent;
        }

        if (printPath) Debug.Log("<color=red>Path found: </color>");

        while (!(path.Count == 0))
        {
            AStarNode n = path.Pop();

            if (path.Count > 0)
            {
                if (printPath) Debug.Log("<color=purple>" + n.gridX + "," + n.gridZ + " -> </color>");
            }
            else
            {
                if (printPath) Debug.Log("<color=purple>" + n.gridX + "," + n.gridZ + "</color>");
            }
            pathXZ.Add(n);
        }

        return pathXZ;
    }

    public List<AStarNode> AStarSearch(AStarNode s, AStarNode goal)
    {
        List<AStarNode> openPath = new List<AStarNode>(); // all possible tiles that lead to the target
        bool[,] closedList = new bool[grid.dimensionsZ, grid.dimensionsX];
        bool done = false;
        int i = s.gridZ;
        int j = s.gridX;

        start = s;
        target = goal;

        if (printPath) Debug.Log("<color=blue>Start:(" + s.gridX + "," + s.gridZ + ")</color>");
        if (printPath) Debug.Log("<color=green>Goal:(" + goal.gridX + "," + goal.gridZ + ")</color>");

        if (IsGoal(start))
        {
            Debug.Log("We der alreadyyy");

            return null;
        }

        if (!InGrid(start.gridX, start.gridZ) || !InGrid(target.gridX, target.gridZ))
        {
            Debug.Log("Node not on grid");

            return null;
        }

        if (!start.validSpace || !target.validSpace)
        {
            Debug.Log("Invalid space, it blocked u dummy");

            return null;
        }

        for (int inZ = 0; inZ < grid.dimensionsZ; inZ++)
        {
            for (int inX = 0; inX < grid.dimensionsX; inX++)
            {
                closedList[inZ, inX] = false;
            }
        }

        //Reset node costs
        for (int ii = 0; ii < grid.dimensionsZ; ii++)
        {
            for (int jj = 0; jj < grid.dimensionsX; jj++)
            {
                grid.nodeGrid[ii, jj].fCost = double.MaxValue;
                grid.nodeGrid[ii, jj].gCost = double.MaxValue;
                grid.nodeGrid[ii, jj].hCost = double.MaxValue;
            }
        }

        start.fCost = 0;
        start.gCost = 0;
        start.hCost = 0;
        //start.parent = start;

        openPath.Add(start);

        while (openPath.Count > 0)
        {
            AStarNode node = openPath[0];
            i = node.gridZ;
            j = node.gridX;

            openPath.RemoveAt(0);
            closedList[i, j] = true;

            double gNew = 0f, hNew = 0f, fNew = 0f;

            ///////////////////////////////////////////////////////////
            ///North
            ///////////////////////////////////////////////////////////
            if (InGrid(i - 1, j))
            {
                if (grid.nodeGrid[i - 1, j].validSpace)
                {
                    if (IsGoal(grid.nodeGrid[i - 1, j]))
                    {
                        grid.nodeGrid[i - 1, j].parent = node;
                        if (printPath) Debug.Log("<color=green>Destination found! </color>" + (i - 1) + "," + j);
                        return TracePath();
                    }
                    else if (closedList[i - 1, j] == false)
                    {
                        gNew = node.gCost + 1.0f;
                        hNew = Mathf.Sqrt((node.gridX - target.gridX - 1) * 2 + (node.gridZ - target.gridZ) * 2);
                        fNew = gNew + hNew;

                        if (grid.nodeGrid[i - 1, j].fCost == double.MaxValue ||
                            grid.nodeGrid[i - 1, j].fCost > fNew)
                        {
                            openPath.Add(grid.nodeGrid[i - 1, j]);

                            grid.nodeGrid[i - 1, j].fCost = fNew;
                            grid.nodeGrid[i - 1, j].gCost = gNew;
                            grid.nodeGrid[i - 1, j].hCost = hNew;

                            grid.nodeGrid[i - 1, j].parent = node;
                        }
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
                        grid.nodeGrid[i + 1, j].parent = node;
                        if (printPath) Debug.Log("<color=green>Destination found! </color>" + i + 1 + "," + j);
                        return TracePath();
                    }
                    else if (closedList[i + 1, j] == false)
                    {
                        gNew = node.gCost + 1.0f;
                        hNew = Mathf.Sqrt((node.gridX - target.gridX + 1) * 2 + (node.gridZ - target.gridZ) * 2);
                        fNew = gNew + hNew;

                        if (grid.nodeGrid[i + 1, j].fCost == double.MaxValue ||
                            grid.nodeGrid[i + 1, j].fCost > fNew)
                        {
                            openPath.Add(grid.nodeGrid[i + 1, j]);

                            grid.nodeGrid[i + 1, j].fCost = fNew;
                            grid.nodeGrid[i + 1, j].gCost = gNew;
                            grid.nodeGrid[i + 1, j].hCost = hNew;

                            grid.nodeGrid[i + 1, j].parent = node;
                        }
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
                        grid.nodeGrid[i, j + 1].parent = node;
                        if (printPath) Debug.Log("<color=green>Destination found! </color>" + (j + 1) + "," + i);
                        return TracePath();
                    }
                    else if (closedList[i, j + 1] == false)
                    {
                        gNew = node.gCost + 1.0f;
                        hNew = Mathf.Sqrt((node.gridX - target.gridX) * 2 + (node.gridZ - target.gridZ + 1) * 2);
                        fNew = gNew + hNew;

                        if (grid.nodeGrid[i, j + 1].fCost == double.MaxValue ||
                            grid.nodeGrid[i, j + 1].fCost > fNew)
                        {
                            openPath.Add(grid.nodeGrid[i, j + 1]);

                            grid.nodeGrid[i, j + 1].fCost = fNew;
                            grid.nodeGrid[i, j + 1].gCost = gNew;
                            grid.nodeGrid[i, j + 1].hCost = hNew;

                            grid.nodeGrid[i, j + 1].parent = node;
                        }
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
                        grid.nodeGrid[i, j - 1].parent = node;
                        if (printPath) Debug.Log("<color=green>Destination found! </color>" + i + "," + (j - 1));
                        return TracePath();
                    }
                    else if (closedList[i, j - 1] == false)
                    {
                        gNew = node.gCost + 1.0f;
                        hNew = Mathf.Sqrt((node.gridX - target.gridX) * 2 + (node.gridZ - target.gridZ - 1) * 2);
                        fNew = gNew + hNew;

                        if (grid.nodeGrid[i, j - 1].fCost == double.MaxValue ||
                            grid.nodeGrid[i, j - 1].fCost > fNew)
                        {
                            openPath.Add(grid.nodeGrid[i, j - 1]);

                            grid.nodeGrid[i, j - 1].fCost = fNew;
                            grid.nodeGrid[i, j - 1].gCost = gNew;
                            grid.nodeGrid[i, j - 1].hCost = hNew;

                            grid.nodeGrid[i, j - 1].parent = node;
                        }
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
                        grid.nodeGrid[i - 1, j + 1].parent = node;
                        if (printPath) Debug.Log("<color=green>Destination found! </color>" + (i - 1) + "," + (j + 1));
                        return TracePath();

                    }
                    else if (closedList[i - 1, j + 1] == false)
                    {
                        gNew = node.gCost + 1.0f;
                        hNew = Mathf.Sqrt((node.gridX - target.gridX - 1) * 2 + (node.gridZ - target.gridZ + 1) * 2);
                        fNew = gNew + hNew;

                        if (grid.nodeGrid[i - 1, j + 1].fCost == double.MaxValue ||
                            grid.nodeGrid[i - 1, j + 1].fCost > fNew)
                        {
                            openPath.Add(grid.nodeGrid[i - 1, j + 1]);

                            grid.nodeGrid[i - 1, j + 1].fCost = fNew;
                            grid.nodeGrid[i - 1, j + 1].gCost = gNew;
                            grid.nodeGrid[i - 1, j + 1].hCost = hNew;

                            grid.nodeGrid[i - 1, j + 1].parent = node;
                        }
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
                        grid.nodeGrid[i - 1, j - 1].parent = node;
                        if (printPath) Debug.Log("<color=green>Destination found! </color>" + (j - 1) + "," + (i - 1));
                        return TracePath();
                    }
                    else if (closedList[i - 1, j - 1] == false)
                    {
                        gNew = node.gCost + 1.0f;
                        hNew = Mathf.Sqrt((node.gridX - target.gridX - 1) * 2 + (node.gridZ - target.gridZ - 1) * 2);
                        fNew = gNew + hNew;

                        if (grid.nodeGrid[i - 1, j - 1].fCost == double.MaxValue ||
                            grid.nodeGrid[i - 1, j - 1].fCost > fNew)
                        {
                            openPath.Add(grid.nodeGrid[i - 1, j - 1]);

                            grid.nodeGrid[i - 1, j - 1].fCost = fNew;
                            grid.nodeGrid[i - 1, j - 1].gCost = gNew;
                            grid.nodeGrid[i - 1, j - 1].hCost = hNew;

                            grid.nodeGrid[i - 1, j - 1].parent = node;
                        }
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
                        grid.nodeGrid[i + 1, j + 1].parent = node;
                        if (printPath) Debug.Log("<color=green>Destination found! </color>" + (i + 1) + "," + (j + 1));
                        return TracePath();
                    }
                    else if (closedList[i + 1, j + 1] == false)
                    {
                        gNew = node.gCost + 1.0f;
                        hNew = Mathf.Sqrt((node.gridX - target.gridX + 1) * 2 + (node.gridZ - target.gridZ + 1) * 2);
                        fNew = gNew + hNew;

                        if (grid.nodeGrid[i + 1, j + 1].fCost == double.MaxValue ||
                            grid.nodeGrid[i + 1, j + 1].fCost > fNew)
                        {
                            openPath.Add(grid.nodeGrid[i + 1, j + 1]);

                            grid.nodeGrid[i + 1, j + 1].fCost = fNew;
                            grid.nodeGrid[i + 1, j + 1].gCost = gNew;
                            grid.nodeGrid[i + 1, j + 1].hCost = hNew;

                            grid.nodeGrid[i + 1, j + 1].parent = node;
                        }
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
                        grid.nodeGrid[i + 1, j - 1].parent = node;
                        if (printPath) Debug.Log("<color=green>Destination found! </color>" + (i + 1) + "," + (j - 1));
                        return TracePath();
                    }
                    else if (closedList[i + 1, j - 1] == false)
                    {
                        gNew = node.gCost + 1.0f;
                        hNew = Mathf.Sqrt((node.gridX - target.gridX - 1) * 2 + (node.gridZ - target.gridZ + 1) * 2);
                        fNew = gNew + hNew;

                        if (grid.nodeGrid[i + 1, j - 1].fCost == double.MaxValue ||
                            grid.nodeGrid[i + 1, j - 1].fCost > fNew)
                        {
                            openPath.Add(grid.nodeGrid[i + 1, j - 1]);

                            grid.nodeGrid[i + 1, j - 1].fCost = fNew;
                            grid.nodeGrid[i + 1, j - 1].gCost = gNew;
                            grid.nodeGrid[i + 1, j - 1].hCost = hNew;

                            grid.nodeGrid[i + 1, j - 1].parent = node;
                        }
                    }
                }
            }
        }

        if (!done)
        {
            Debug.Log("<color=red>FAILURE TO FIND PATH</color>");
        }

        return null;
    }
}
