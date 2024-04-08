using System;
using System.Collections;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using UnityEngine;

public abstract class UnitTrap : UnitAction
{
    public bool IsTrapSprung => isTrapSprung;
    [SerializeField] protected GameObject trapBackground;
    [SerializeField] protected GameObject trapForeground;
    [SerializeField] protected GameObject trapObjectSprite;
    [SerializeField] protected int trapEffectTurns = 2;
    [SerializeField] protected float trapAnimationTime = 1f;
    protected GridManager gridManager;
    protected int turnsUntilDestroyed;
    protected bool isTrapSprung = false;

    protected override void Awake() 
    {
        base.Awake();
        // Traps have no available actions.
        actionPointsRemaining = 0;
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

        SetupSprites();
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
        SetupSprites();
    }

    private void SetupSprites()
    {
        if (isTrapSprung)
        {
            trapObjectSprite.SetActive(false);
            if(trapBackground != null)
            {
                trapBackground.SetActive(true);
                trapForeground.SetActive(true);
            }
        }
        else
        {
            trapObjectSprite.SetActive(true);
            if(trapBackground != null)
            {
                trapBackground.SetActive(false);
                trapForeground.SetActive(false);
            }
        }
    }

    public override SaveUnitData SaveAction(SaveUnitData saveData)
    {
        saveData.TrapIsSprung = isTrapSprung;
        return saveData;
    }

    public override bool TryTakeAction(GridObject gridObject, Action onActionComplete)
    {
        // Exit early when actively trying to spring a trap. This could be changed later.
        return false;
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
            SpringTheTrap(movingUnit.Unit);
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
        Instantiate(Unit.HitFX, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(trapAnimationTime);
        AnimateTrap();
        ApplyTrapEffect(trappedUnit);
        // Destroy after turns
        turnsUntilDestroyed = trapEffectTurns;
        ActionComplete();
    }

    internal void SetActionCompletedAction(Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
    }

    abstract protected void AnimateTrap();
    abstract protected void ApplyTrapEffect(Unit trappedUnit);
}
