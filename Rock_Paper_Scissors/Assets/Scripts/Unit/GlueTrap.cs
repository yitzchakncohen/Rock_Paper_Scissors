using System.Collections;
using RockPaperScissors.Grids;
using RockPaperScissors.Units;
using UnityEngine;

public class GlueTrap : UnitTrap
{    
    protected override void Start()
    {
        base.Start();
        UnitHealth.OnDeath += UnitHealth_OnDeath;
    }

    protected override void OnDestroy() 
    {
        base.OnDestroy();
        UnitHealth.OnDeath -= UnitHealth_OnDeath;
    }

    protected override void AnimateTrap()
    {
        // Trap animation
        trapBackground.SetActive(true);
        trapForeground.SetActive(true);
    }

    protected override IEnumerator ApplyTrapEffect(Unit trappedUnit)
    {
        // Damage unit
        int damageAmount = CombatModifiers.GetDamage(unit, trappedUnit, false);
        trappedUnit.Damage(damageAmount, unit);
        yield return null;
    }

    private void UnitHealth_OnDeath( object sender, Unit attacker)
    {
        Unit deadUnit = (sender as UnitHealth).Unit;
        GridObject gridObject =  gridManager.GetGridObjectFromWorldPosition(deadUnit.transform.position);
        Unit glueTrap = gridObject.OccupantTrap as Unit;
        if(glueTrap && glueTrap == Unit)
        {
            Unit.DestroyUnit();
        }
    }
}
