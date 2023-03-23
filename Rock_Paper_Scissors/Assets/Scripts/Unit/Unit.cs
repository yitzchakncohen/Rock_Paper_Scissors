using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isFriendly = true;
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

    public void Damage(int damageAmount)
    {
        health.Damage(damageAmount);
    }

    public float GetNormalizedHealth() => health.GetNormalizedHealth();
}
