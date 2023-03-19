using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start() 
    {
        ActionHandler.OnUnitSelected += ActionHandler_OnUnitSelected;
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
}
