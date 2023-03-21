using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isFriendly = true;
    private Health health;
    private UnitAttack unitAttacking;
    private UnitMovement unitMovement;

    private void Awake() 
    {
        health = GetComponent<Health>();
        unitAttacking = GetComponent<UnitAttack>();
        unitMovement = GetComponent<UnitMovement>();
    }

    private void Start() 
    {
        ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
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

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public UnitAttack GetUnitAttacking()
    {
        return unitAttacking;
    }

    public void Damage(int damageAmount) => health.Damage(damageAmount);
}
