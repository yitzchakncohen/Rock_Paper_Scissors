using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public static event EventHandler OnUnitSpawn;
    [SerializeField] private UnitAnimator unitAnimator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isFriendly = true;
    [SerializeField] private UnitData unitData;
    private UnitAction[] unitActions;
    private Health health;

    private void Awake() 
    {
        unitActions = GetComponents<UnitAction>();
        health = GetComponent<Health>();
    }

    private void Start() 
    {
        ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
        OnUnitSpawn?.Invoke(this, EventArgs.Empty);
        spriteRenderer.sprite = unitData.unitThumbnail;
        if(unitAnimator != null)
        {
            unitAnimator.SetSpriteLibraryAsset(unitData.spriteLibrary);
        }
    }

    private void OnDestroy() 
    {
        ActionHandler.OnUnitSelected -= ActionHandler_OnUnitSelected;
    }

    private void ActionHandler_OnUnitSelected(object sender, Unit selectedUnit)
    {
        if(selectedUnit == this)
        {
            SetOutlineOn();
        }
        else
        {
            SetOutlineOff();
        }
    }

    public UnitAction[] GetUnitActions()
    {
        return unitActions;
    }

    public void SetOutlineOn()
    {
        spriteRenderer.material.SetInt("_OutlineOn", 1);
    }

    public void SetOutlineOff()
    {
        spriteRenderer.material.SetInt("_OutlineOn", 0);
    }

    public bool IsFriendly()
    {
        return isFriendly;
    }

    public void Damage(int damageAmount, Unit attacker)
    {
        health.Damage(damageAmount, attacker);
    }

    public float GetNormalizedHealth() => health.GetNormalizedHealth();

    public int GetMaximumHealth()
    {
        return unitData.maximumHealth;
    }

    public int GetCost()
    {
        return unitData.unitCost;
    }

    public Sprite GetUnitThumbnail()
    {
        return unitData.unitThumbnail;
    }

    public int GetUnitAttackDamage()
    {
        return unitData.attackDamage;
    }

    public int GetAttackRange()
    {
        return unitData.attackRange;
    }

    internal int GetMoveDistance()
    {
        return unitData.moveDistance;
    }
}
