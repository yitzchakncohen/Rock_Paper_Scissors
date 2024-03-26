using RockPaperScissors.Grids;
using UnityEngine;

namespace RockPaperScissors.PathFindings
{
    public class PathNode
    {
        private GridObject gridObject;
        private int gCost;
        private int hCost;
        private int fCost;
        private PathNode cameFromPathNode;

        public PathNode(GridObject gridObject)
        {
            this.gridObject = gridObject;
        }

        public override string ToString()
        {
            return gridObject.ToString();
        }

        public int GetGCost()
        {
            return gCost;
        }

        public int GetHCost()
        {
            return hCost;
        }

        public int GetFCost()
        {
            return fCost;
        }

        public void SetGCost(int gCost)
        {
            this.gCost = gCost;
        }

        public void SetHCost(int hCost)
        {
            this.hCost = hCost;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        public void ResetCameFromPathNode()
        {
            cameFromPathNode = null;
        }

        public void SetCameFromPathNode(PathNode pathNode)
        {
            cameFromPathNode = pathNode;
        }

        public PathNode GetCameFromPathNode()
        {
            return cameFromPathNode;
        }

        public Vector2Int GetGridPosition()
        {
            return gridObject.GetGridPostion();
        }

        public Vector3 GetWorldPosition()
        {
            return gridObject.transform.position;
        }

        public bool IsWalkable(IGridOccupantInterface gridOccupantInterface)
        {
            return gridObject.IsWalkable(gridOccupantInterface);
        }
    }
}
