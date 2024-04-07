using RockPaperScissors.Grids;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class GridObjectUI : MonoBehaviour
    {
        [SerializeField] private TextMeshPro actionValueText;
        [SerializeField] private TextMeshPro gridPositionText;
        [SerializeField] private TextMeshPro gridDistanceText;
        [SerializeField] private GameObject movementHighlight;
        [SerializeField] private GameObject attackHighlight;
        [SerializeField] private GameObject placementHighlight;
        [SerializeField] private GameObject actionAvailableHighlight;
        [SerializeField] private bool debugging = false;
        [SerializeField] private OutlineShine outlineShine;
        public OutlineShine OutlineShine => outlineShine;
        [SerializeField] private OutlineShine actionAvailableAnimation;


        private void Awake() 
        {
            if(!debugging)
            {
                actionValueText.enabled = false;
                gridPositionText.enabled = false;
                gridDistanceText.enabled = false;
            }
            actionAvailableHighlight.SetActive(false);
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

        public void SetDistanceFromPosition(Vector2Int centerPosition, Vector2Int position)
        {
            if(debugging)
            {
                int dx = position.x - centerPosition.x;
                int dy = position.y - centerPosition.y;
                int x = Mathf.Abs(dx);
                int y = Mathf.Abs(dy);
                if(centerPosition.y % 2 == 1 ^ dx < 0)
                {
                    int distance = Mathf.Max(0, x - (y + 1) / 2) + y;
                    gridDistanceText.text = distance.ToString();
                }
                else
                {
                    int distance = Mathf.Max(0, x - y / 2) + y;
                    gridDistanceText.text = distance.ToString();
                }
            }
        }

        public void SetActionAvailableHighlight(bool isAvailable)
        {
            // actionAvailableHighlight.SetActive(isAvailable);
            if(isAvailable)
            {
                actionAvailableAnimation.StartShine(0, 1.0f);
            }
            else
            {
                actionAvailableAnimation.StopShine();
            }
        }
    }
}
