using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEditor.VersionControl;
using UnityEngine;

public class UnitTrap : UnitAction
{
    [SerializeField] private GameObject trapBackground;
    [SerializeField] private GameObject trapForeground;
    [SerializeField] private GameObject trapObjectSprite;
    [SerializeField] private int trapEffectTurns = 2;
    [SerializeField] private float trapAnimationTime = 1f;
    private GridManager gridManager;
    private Unit unit;
    private int turnsUntilDestroyed;
    private bool isTrapSprung = false;

    private void Awake() 
    {
        unit = GetComponent<Unit>();
    }

    protected override void Start()
    {
        base.Start();
        gridManager = FindObjectOfType<GridManager>();
    }

    private void OnEnable() 
    {
        UnitMovement.OnAnyActionCompleted += UnitMovement_OnAnyActionCompleted;
        TurnManager.OnNextTurn += TurnManager_OnNextTurn;

        trapBackground.SetActive(false);
        trapForeground.SetActive(false);
        trapObjectSprite.SetActive(true);
    }

    private void OnDestroy() 
    {
        UnitMovement.OnAnyActionCompleted -= UnitMovement_OnAnyActionCompleted;
        TurnManager.OnNextTurn -= TurnManager_OnNextTurn;
    }

    private void TurnManager_OnNextTurn(object sender, TurnManager.OnNextTurnEventArgs e)
    {
        if(isTrapSprung)
        {
            turnsUntilDestroyed--;
            if(turnsUntilDestroyed <= 0)
            {
                unit.DestroyUnit();
            }
        }
    }

    public override EnemyAIAction GetBestEnemyAIAction()
    {
        return null;
    }

    public override int GetValidActionsRemaining()
    {
        return 0; 
    }

    public override void LoadAction(SaveUnitData loadData)
    {
        isTrapSprung = loadData.TrapIsSprung;
    }

    public override void SaveAction(SaveUnitData saveData)
    {
        saveData.TrapIsSprung = isTrapSprung;
    }

    public override bool TryTakeAction(GridObject gridObject, Action onActionComplete)
    {
        // Exit early when actively trying to spring a trap. This could be changed later.
        return false;
    }

    public Unit GetUnit()
    {
        return unit;
    }
    
    public bool GetIsTrapSprung()
    {
        return isTrapSprung;
    }

    private void UnitMovement_OnAnyActionCompleted(object sender, EventArgs e)
    {
        // Check if the unit that is moving is on the grid location of this trap.
        UnitMovement movingUnit = sender as UnitMovement;
        if(movingUnit == null)
        {
            return;
        }

        GridObject movingUnitGridObject = gridManager.GetGridObjectFromWorldPosition(movingUnit.transform.position);
        GridObject trapGridObject = gridManager.GetGridObjectFromWorldPosition(transform.position);
        if(movingUnitGridObject == trapGridObject)
        {
            SpringTheTrap(movingUnit.GetUnit());
        }
    }

    private void SpringTheTrap(Unit trappedUnit)
    {
        ActionStart(onActionComplete);
        Debug.Log("It's a trap!");
        isTrapSprung = true;
        trappedUnit.SetTrappedTurnsRemaining(trapEffectTurns);
        StartCoroutine(TrapRoutine(trappedUnit));
    }

    private IEnumerator TrapRoutine(Unit trappedUnit)
    {
        trapObjectSprite.SetActive(false);
        Instantiate(unit.GetHitFX(), transform.position, Quaternion.identity);
        yield return new WaitForSeconds(trapAnimationTime);
        // Trap animation
        trapBackground.SetActive(true);
        trapForeground.SetActive(true);
        // Damage unit
        int damageAmount = CombatModifiers.GetDamage(unit, trappedUnit, false);
        trappedUnit.Damage(damageAmount, unit);
        // Destroy after turns
        turnsUntilDestroyed = trapEffectTurns;
        ActionComplete();
    }

    internal void SetActionCompletedAction(Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
    }
}
