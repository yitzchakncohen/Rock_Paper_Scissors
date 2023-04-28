using RockPaperScissors.UI;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.Grids
{
    public class GridObject : MonoBehaviour
    {
        private Vector2Int gridPosition;
        private GridObjectUI gridObjectUI;
        private Unit gridPositionOccupyingUnit = null;
        private Unit gridPositionOccupyingTower = null;

        private void Awake() 
        {
            gridObjectUI = GetComponent<GridObjectUI>();
        }

        public void Setup(Vector2Int gridPosition) 
        {
            this.gridPosition = gridPosition;
            gridObjectUI.SetGridPosition(gridPosition);
        }

        public Vector2Int GetGridPostion()
        {
            return gridPosition;
        }

        public bool IsWalkable(bool isFriendly)
        {
            // Grid position empty
            if(gridPositionOccupyingUnit == null && gridPositionOccupyingTower == null) {return true;}
            
            // Grid position has unit
            // TODO can you walk over your own units?            
            if(gridPositionOccupyingUnit != null) {return false;}

            if(gridPositionOccupyingTower != null && isFriendly == gridPositionOccupyingTower.IsFriendly()) {return true;}

            return false;
        }

        public void SetOccupentUnit(Unit occupent)
        {
            gridPositionOccupyingUnit = occupent;
        }

        public Unit GetOccupentUnit()
        {
            return gridPositionOccupyingUnit;
        }

        public void SetOccupentTower(Unit occupent)
        {
            gridPositionOccupyingTower = occupent;
        }

        public Unit GetOccupentTower()
        {
            return gridPositionOccupyingTower;
        }

        public void ShowHighlight(GridHighlightType highlightType) => gridObjectUI.ShowHighlight(highlightType);

        public void HideHighlight(GridHighlightType highlightType) => gridObjectUI.HideHighlight(highlightType);
        public void HideAllHighlights() => gridObjectUI.HideAllHighlights();
        public void SetActionValue(float actionValue) => gridObjectUI.SetActionValue(actionValue);
    }
}
