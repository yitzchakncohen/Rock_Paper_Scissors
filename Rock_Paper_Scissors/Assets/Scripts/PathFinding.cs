using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 1;
    private GridManager gridManager;
    private PathNode[,] pathNodes;
    private Vector2Int gridSize;

    private void Start() 
    {
        gridManager = FindObjectOfType<GridManager>();
        gridSize = gridManager.GetGridSize();
        // Setup the nodes
        pathNodes = new PathNode[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x,y);
                pathNodes[x,y] = new PathNode(gridManager.GetGridObject(gridPosition));
            }
        } 
    }

    public List<GridObject> FindPath(Vector2Int startGridPosition, Vector2Int endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = pathNodes[startGridPosition.x, startGridPosition.y];
        PathNode endNode = pathNodes[endGridPosition.x, endGridPosition.y];
        openList.Add(startNode);
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                PathNode pathNode = pathNodes[gridPosition.x, gridPosition.y];

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateHeuristicDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if(currentNode == endNode)
            {
                // Reached the final node
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if(closedList.Contains(neighbourNode))
                {
                    // Already searched
                    continue;
                }

                if(!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + MOVE_STRAIGHT_COST;

                if(tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateHeuristicDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if(!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // No path found
        pathLength = 0;
        return null;
    }

    public int CalculateHeuristicDistance(Vector2Int gridPositionA, Vector2Int gridPositionB)
    {
        return Mathf.RoundToInt(MOVE_STRAIGHT_COST * Vector3.Distance(GetNode(gridPositionA.x, gridPositionA.y).GetWorldPosition(), 
                                                                        GetNode(gridPositionB.x, gridPositionB.y).GetWorldPosition()));
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private PathNode GetNode(int x, int y)
    {
        return pathNodes[x,y];
    }
    
    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        Vector2Int gridPosition = currentNode.GetGridPosition();
        bool oddRow = gridPosition.y % 2 == 1;

        if(gridPosition.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(gridPosition.x -1, gridPosition.y +0));

        }

        if(gridPosition.x + 1 < gridSize.x)
        {
            // Right
            neighbourList.Add(GetNode(gridPosition.x +1, gridPosition.y +0));
        }

        if(gridPosition.y -1 >= 0)
        {
            // Down
            neighbourList.Add(GetNode(gridPosition.x +0, gridPosition.y -1));
            if(gridPosition.x - 1 >= 0 && gridPosition.x + 1 < gridSize.x)
            {
                neighbourList.Add(GetNode(gridPosition.x + (oddRow ? +1 : -1), gridPosition.y -1));                
            }
        }

        if(gridPosition.y + 1 < gridSize.y)
        {
            // Up
            neighbourList.Add(GetNode(gridPosition.x + 0, gridPosition.y +1));
            if(gridPosition.x - 1 >= 0 && gridPosition.x + 1 < gridSize.x)
            {
                neighbourList.Add(GetNode(gridPosition.x + (oddRow ? +1 : -1), gridPosition.y +1));
            }
        }

        return neighbourList;
    }

    private List<GridObject> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }
        
        pathNodeList.Reverse();
        
        List<GridObject> gridPositionList  = new List<GridObject>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionList.Add(gridManager.GetGridObject(pathNode.GetGridPosition()));
        }

        return gridPositionList;
    }

    public bool IsWalkableGridPosition(Vector2Int gridPosition)
    {
        return gridManager.GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(Vector2Int startGridPosition, Vector2Int endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }
}
