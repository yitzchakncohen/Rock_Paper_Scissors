using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CancelButton : MonoBehaviour
{
    public static event Action OnCancelButtonPress;
    private Button button;

    private void Awake() 
    {  
        button = GetComponent<Button>();
    }

    private void Start() 
    {
        UnitAction.OnAnyActionStarted += UnitAction_OnAnyActionStarted;
        UnitAction.OnAnyActionCompleted += UnitAction_OnAnyActionCompleted;
        button.interactable = false;
    }

    private void UnitAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        if(((UnitAction)sender).IsCancellableAction)
        {
            button.interactable = false;
        }
    }

    private void UnitAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        if(((UnitAction)sender).IsCancellableAction)
        {
            button.interactable = true;
        }
    }

    public void CancelButtonPress()
    {
        OnCancelButtonPress?.Invoke();
    }
}
