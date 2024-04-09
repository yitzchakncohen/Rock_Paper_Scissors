using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRangeIndicator : MonoBehaviour
{
    [SerializeField] private GameObject topRight;
    [SerializeField] private GameObject topLeft;

    [SerializeField] private GameObject right;

    [SerializeField] private GameObject left;

    [SerializeField] private GameObject bottomRight;

    [SerializeField] private GameObject bottomLeft;

    public void SetIndicator(Direction direction)
    {
        DisableAll();
        if(direction.HasFlag(Direction.East))
        {
            right.SetActive(true);
        }
        if(direction.HasFlag(Direction.West))
        {
            left.SetActive(true);
        }
        if(direction.HasFlag(Direction.NorthWest))
        {
            topLeft.SetActive(true);
        }
        if(direction.HasFlag(Direction.SouthWest))
        {
            bottomLeft.SetActive(true);
        }
        if(direction.HasFlag(Direction.NorthEast))
        {
            topRight.SetActive(true);
        }
        if(direction.HasFlag(Direction.SouthEast))
        {
            bottomRight.SetActive(true);
        }
    }

    public void DisableAll()
    {
        topRight.SetActive(false);
        topLeft.SetActive(false);
        right.SetActive(false);
        left.SetActive(false);
        bottomRight.SetActive(false);
        bottomLeft.SetActive(false);    
    }
}
