using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static event Action OnPauseMenuOpen;
    public static event Action OnPauseMenuClose;

    private void OnEnable() 
    {
        OnPauseMenuOpen?.Invoke();
    }

    private void OnDisable() 
    {
        OnPauseMenuClose?.Invoke();
    }
}
