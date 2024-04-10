using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.Units;
using UnityEngine;

public class TrampolineTrap : UnitTrap
{
    [SerializeField] private int launchDistance = 5;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private AnimationCurve animationHeightCurve;
    private float launchAnimationTimeMultiplier = 0.2f;
    List<Vector2Int> launchLocations = new List<Vector2Int>();

    protected override void AnimateTrap()
    {
        // TODO animate the trap.
    }

    protected override IEnumerator ApplyTrapEffect(Unit trappedUnit)
    {
        Direction unitFacingDirection = trappedUnit.UnitAnimator.GetCurrentDirection();
        Vector2Int gridLaunchLocation = gridManager.GetGridPositionFromWorldPosition(transform.position);
        Vector2Int landingGridLocation = GetLandingLocation(unitFacingDirection, gridLaunchLocation, launchDistance);
        int i = 1;
        while(!gridManager.GetGridObject(landingGridLocation).IsWalkable(trappedUnit) && i < launchDistance)
        {
            landingGridLocation = GetLandingLocation(unitFacingDirection, gridLaunchLocation, launchDistance -i);
            i++;
        }
        yield return StartCoroutine(LaunchUnitRoutine(trappedUnit, landingGridLocation));
    }

    private Vector2Int GetLandingLocation(Direction unitFacingDirection, Vector2Int gridLaunchLocation, int launchDistance)
    {
        // Odd Row
        bool oddRow = gridLaunchLocation.y % 2 == 1;
        switch (unitFacingDirection)
        {
            case Direction.NorthEast:
                return new Vector2Int(gridLaunchLocation.x + (oddRow ? launchDistance + 1 : launchDistance - 1), gridLaunchLocation.y -launchDistance);
            case Direction.NorthWest:
                return new Vector2Int(gridLaunchLocation.x + 0, gridLaunchLocation.y - launchDistance);
            case Direction.West:
                return new Vector2Int(gridLaunchLocation.x + launchDistance, gridLaunchLocation.y + 0);
            case Direction.East:
                return new Vector2Int(gridLaunchLocation.x - launchDistance, gridLaunchLocation.y + 0);
            case Direction.SouthEast:
                return new Vector2Int(gridLaunchLocation.x + (oddRow ? launchDistance + 1 : launchDistance - 1), gridLaunchLocation.y + launchDistance);
            case Direction.SouthWest:
                return new Vector2Int(gridLaunchLocation.x + 0, gridLaunchLocation.y + launchDistance);
        }
        return gridLaunchLocation;
    }

    private IEnumerator LaunchUnitRoutine(Unit unit, Vector2Int landingGridLocation)
    {
        Vector2Int gridPosition = gridManager.GetGridPositionFromWorldPosition(transform.position);
        float animationTime = launchAnimationTimeMultiplier * gridManager.GetGridDistanceBetweenPositions(landingGridLocation, gridPosition);
        float timer = 0f;
        float maxLaunchHeight = 2.5f;
        Vector3 launchStartPosition = transform.position;
        Vector3 landingPosition = gridManager.GetGridObject(landingGridLocation).transform.position;
        AudioManager.Instance.PlayTrampolineTrapSound();
        while(timer < animationTime)
        {
            float normalizedAnimationTime = timer/animationTime;
            Vector3 launchHeight = new Vector3(0, animationHeightCurve.Evaluate(normalizedAnimationTime)*maxLaunchHeight, 0);
            unit.transform.position = launchStartPosition + (landingPosition - launchStartPosition)*animationCurve.Evaluate(normalizedAnimationTime) + launchHeight;
            timer += Time.deltaTime;
            yield return null;
        }
        unit.transform.position = landingPosition;
    }

    public override bool TryTakeAction(GridObject gridObject, Action onActionComplete)
    {
        if(launchLocations.Contains(gridObject.Position))
        {
            ActionStart(onActionComplete);
            GridObject launchGridObject = gridManager.GetGridObjectFromWorldPosition(transform.position);
            Unit unitToLaunch = launchGridObject.GetOccupantUnit() as Unit;
            StartCoroutine(LaunchUnit(unitToLaunch, gridObject.Position, launchGridObject.Position));
            return true;
        }
        return false;
    }

    private IEnumerator LaunchUnit(Unit unit, Vector2Int landingGridLocation, Vector2Int gridLaunchLocation)
    {
        yield return StartCoroutine(LaunchUnitRoutine(unit, landingGridLocation));
        ActionComplete();  
    }

    public List<Vector2Int> GetLaunchLocations(Unit unitToLaunch, GridObject gridObject)
    {
        launchLocations.Clear();
        for (int x = 0; x < gridManager.GridSize.x; x++)
        {
            for (int z = 0; z < gridManager.GridSize.y; z++)
            {
                Vector2Int testGridPosition = new Vector2Int(x, z);
                int distance = gridManager.GetGridDistanceBetweenPositions(testGridPosition, gridObject.Position);
                if(distance == launchDistance)
                {
                    GridObject testGridObject = gridManager.GetGridObject(testGridPosition);
                    if(testGridObject.IsWalkable(unitToLaunch))
                    {
                       launchLocations.Add(testGridObject.Position);
                    }
                }
            }
        }
        return launchLocations;
    }
}
