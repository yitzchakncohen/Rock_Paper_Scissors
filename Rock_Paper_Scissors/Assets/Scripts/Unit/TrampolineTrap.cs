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

    protected override void AnimateTrap()
    {
        // TODO animate the trap.
    }

    protected override void ApplyTrapEffect(Unit trappedUnit)
    {
        Direction unitFacingDirection = trappedUnit.UnitAnimator.GetCurrentDirection();
        Vector2Int gridLaunchLocation = gridManager.GetGridPositionFromWorldPosition(transform.position);
        Vector2Int landingGridLocation = GetLandingLocation(unitFacingDirection, gridLaunchLocation);
        StartCoroutine(LaunchUnitRoutine(trappedUnit, landingGridLocation, gridLaunchLocation));
    }

    private Vector2Int GetLandingLocation(Direction unitFacingDirection, Vector2Int gridLaunchLocation)
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

    private IEnumerator LaunchUnitRoutine(Unit unit, Vector2Int landingGridLocation, Vector2Int gridLaunchLocation)
    {
        float animationTime = launchAnimationTimeMultiplier * Vector2Int.Distance(landingGridLocation, gridLaunchLocation);
        float timer = 0f;
        float maxLaunchHeight = 2.5f;
        Vector3 launchStartPosition = transform.position;
        Vector3 landingPosition = gridManager.GetGridObject(landingGridLocation).transform.position;
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
}
