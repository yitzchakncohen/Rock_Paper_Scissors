using RockPaperScissors.UI;
using RockPaperScissors.Units;
using UnityEngine;

namespace RockPaperScissors.Grids
{
    public class GridObject : MonoBehaviour
    {
        private Vector2Int gridPosition;
        private GridObjectUI gridObjectUI;
        private IGridOccupantInterface gridPositionOccupyingUnit = null;
        private IGridOccupantInterface gridPositionOccupyingBuilding = null;
        private IGridOccupantInterface gridPositionOccupyingTrap = null;

        private void Awake() 
        {
            gridObjectUI = GetComponent<GridObjectUI>();
        }

        public void Setup(Vector2Int gridPosition) 
        {
            this.gridPosition = gridPosition;
            gridObjectUI.SetGridPosition(gridPosition);
            gridObjectUI.SetDistanceFromPosition(Vector2Int.one*18, gridPosition);
        }

        public Vector2Int GetGridPostion()
        {
            return gridPosition;
        }

        public bool IsWalkable(IGridOccupantInterface gridObject)
        {
            // Grid position empty
            if(gridPositionOccupyingUnit == null 
                && gridPositionOccupyingBuilding == null 
                && gridPositionOccupyingTrap == null) 
            {
                return true;
            }
            
            // Grid position has unit
            // TODO can you walk over your own units?            
            if(gridPositionOccupyingUnit != null) 
            {
                return false;
            }
            if(gridPositionOccupyingBuilding != null)
            {
                return gridObject.CanWalkOnGridOccupant(gridPositionOccupyingBuilding);
            }
            if(gridPositionOccupyingTrap != null)
            {
                return gridObject.CanWalkOnGridOccupant(gridPositionOccupyingTrap);
            }

            return true;
        }

        public void SetOccupantUnit(IGridOccupantInterface Occupant)
        {
            gridPositionOccupyingUnit = Occupant;
        }

        public IGridOccupantInterface GetOccupantUnit()
        {
            return gridPositionOccupyingUnit;
        }

        public void SetOccupantBuilding(IGridOccupantInterface Occupant)
        {
            gridPositionOccupyingBuilding = Occupant;
        }

        public IGridOccupantInterface GetOccupantBuilding()
        {
            return gridPositionOccupyingBuilding;
        }

        public void SetOccupantTrap(IGridOccupantInterface Occupant)
        {
            gridPositionOccupyingTrap = Occupant;
        }

        public IGridOccupantInterface GetOccupantTrap()
        {
            return gridPositionOccupyingTrap;
        }

        public IGridOccupantInterface GetCombatTarget()
        {
            if(gridPositionOccupyingUnit != null)
            {
                return gridPositionOccupyingUnit;
            }
            if(gridPositionOccupyingBuilding != null)
            {
                return gridPositionOccupyingBuilding;
            }
            return null;
        }

        public void ShowHighlight(GridHighlightType highlightType) => gridObjectUI.ShowHighlight(highlightType);

        public void HideHighlight(GridHighlightType highlightType) => gridObjectUI.HideHighlight(highlightType);
        public void HideAllHighlights() => gridObjectUI.HideAllHighlights();
        public void SetActionValue(float actionValue) => gridObjectUI.SetActionValue(actionValue);
        public void SetDistanceFromPosition(Vector2Int centerPosition) => gridObjectUI.SetDistanceFromPosition(centerPosition, gridPosition);
        public void SetActionAvailableHighlight(bool isAvailable) => gridObjectUI.SetActionAvailableHighlight(isAvailable);    
        public OutlineShine GetOutlineShine() => gridObjectUI.OutlineShine;
        public void EnableAttackRangeIndicator(Direction direction) => gridObjectUI.EnableAttackRangeIndicator(direction);
        public void DisableAttackRangeIndicator() => gridObjectUI.DisableAttackRangeIndicator();
    }
}
