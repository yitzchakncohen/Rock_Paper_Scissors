using RockPaperScissors.Grids;
using TMPro;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class GridObjectUI : MonoBehaviour
    {
        [SerializeField] private TextMeshPro actionValueText;
        [SerializeField] private TextMeshPro gridPositionText;
        [SerializeField] private GameObject movementHighlight;
        [SerializeField] private GameObject attackHighlight;
        [SerializeField] private GameObject placementHighlight;
        [SerializeField] private bool debugging = false;

        private void Awake() 
        {
            if(!debugging)
            {
                actionValueText.enabled = false;
                gridPositionText.enabled = false;
            }
        }

        public void ShowHighlight(GridHighlightType highlightType)
        {
            switch(highlightType)
            {
                case GridHighlightType.Movement:
                    movementHighlight.SetActive(true);
                    break;
                case GridHighlightType.Attack:
                    attackHighlight.SetActive(true);
                    break;
                case GridHighlightType.PlaceObject:
                    placementHighlight.SetActive(true);
                    break;
            }
        }

        public void HideHighlight(GridHighlightType highlightType)
        {
            switch(highlightType)
            {
                case GridHighlightType.Movement:
                    attackHighlight.SetActive(false);
                    break;
                case GridHighlightType.Attack:
                    movementHighlight.SetActive(false);  
                    break;
                case GridHighlightType.PlaceObject:
                    placementHighlight.SetActive(false);
                    break;
            }
        }

        public void HideAllHighlights()
        {
            attackHighlight.SetActive(false);
            movementHighlight.SetActive(false);
            placementHighlight.SetActive(false);
        }

        public void SetGridPosition(Vector2 gridPosition)
        {   
            if(debugging)
            {
                gridPositionText.text = $"x: {gridPosition.x}, y: {gridPosition.y}";
            }
        }

        public void SetActionValue(float actionValue)
        {   
            if(debugging)
            {
                if(actionValue == 0f)
                {
                    actionValueText.text = $"----";
                }
                else
                {
                    actionValueText.text = $"AV: {actionValue:0.00}";
                }
            }
        }
    }
}
