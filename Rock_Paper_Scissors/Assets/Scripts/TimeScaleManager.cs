using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    private static float fastForwardMultiplier = 3f;
    private static float currentTimeScale = 1.0f;

    private void Start()
    {
        PauseMenu.OnPauseMenuOpen += PauseMenu_OnPauseMenuOpen;
        PauseMenu.OnPauseMenuClose += PauseMenu_OnPauseMenuClose;
    }

    private void OnDisable() 
    {
        PauseMenu.OnPauseMenuOpen -= PauseMenu_OnPauseMenuOpen;
        PauseMenu.OnPauseMenuClose -= PauseMenu_OnPauseMenuClose;
    }

    private void PauseMenu_OnPauseMenuClose()
    {
        ReturnToPreviousTimeScale();
    }

    private void PauseMenu_OnPauseMenuOpen()
    {
        currentTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
    }
    
    private static void ReturnToPreviousTimeScale()
    {
        Time.timeScale = currentTimeScale;
    }

    public static void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }

    public static void SpeedUpTimeScale()
    {
        Time.timeScale = fastForwardMultiplier;
    }


    public static void ReturnToNormalSpeed()
    {
        currentTimeScale = 1.0f;
        if(Time.timeScale > 0.0f)
        {
            Time.timeScale = currentTimeScale;
        }
    }
}
