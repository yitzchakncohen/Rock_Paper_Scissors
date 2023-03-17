using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridObjectUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro gridPositionText;

    public void SetGridPosition(Vector2 gridPosition)
    {   
        gridPositionText.text = $"x: {gridPosition.x}, y: {gridPosition.y}";
        // gridPositionText.text = $"x: {transform.position.x}, \n y: {transform.position.y}";
    }
}
