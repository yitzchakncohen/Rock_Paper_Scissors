using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using UnityEngine;

namespace RockPaperScissors.Grids
{
    public class GridUI : MonoBehaviour
    {
        [SerializeField] private bool gridWaveAnimationEnabled = false;
        [SerializeField] private float gridWaveAnimationTimePerHex = 0.5f;
        private GridManager gridManager;
        
        private void Awake() 
        {
            gridManager = GetComponent<GridManager>();
            gridManager.OnGridSetupComplete += GridManager_OnGridSetupComplete;
        }

        private void GridManager_OnGridSetupComplete()
        {
            StartCoroutine(AnimateGrid());
        }

        public void HideAllGridPosition()
        {
            for (int x = 0; x < gridManager.GetGridSize().x; x++)
            {
                for (int y = 0; y < gridManager.GetGridSize().y; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x,y);
                    gridManager.GetGridObject(gridPosition).HideAllHighlights();
                }
            }
        }

        public void ShowGridPositionList(List<Vector2Int> gridPositionList, GridHighlightType highlightType)
        {
            foreach (Vector2Int gridPosition in gridPositionList)
            {
                gridManager.GetGridObject(gridPosition).ShowHighlight(highlightType);
            }
        }

        private IEnumerator AnimateGrid()
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(gridWaveAnimationTimePerHex/2);
            while(gridWaveAnimationEnabled)
            {
                StartCoroutine(AnimateGridWave(true));
                yield return waitForSeconds;
                yield return StartCoroutine(AnimateGridWave(false));
            }
        }

        private IEnumerator AnimateGridWave(bool even)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(gridWaveAnimationTimePerHex);
            for (int x = 0; x < gridManager.GetGridSize().x; x++)
            {
                for (int y = 0; y < gridManager.GetGridSize().y; y++)
                {
                    if((even && y % 2 == 0) || (!even && y % 2 == 1))
                    {
                        Vector2Int gridPosition = new Vector2Int(x,y);
                        OutlineShine outlineShine = gridManager.GetGridObject(gridPosition).GetComponentInChildren<OutlineShine>();
                        StartCoroutine(outlineShine.StartShine(gridWaveAnimationTimePerHex));
                    }
                }
                yield return waitForSeconds;
            }
        }
    }
}
