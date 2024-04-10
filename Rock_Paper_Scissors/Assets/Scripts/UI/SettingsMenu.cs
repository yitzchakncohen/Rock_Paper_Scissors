using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private const string VOLUME_PREF_STRING = "volume";
    private const string SOUND_ENABLED_PREF_STRING = "soundEnabled";

    private ModalWindow modalWindow;

    [SerializeField] private Button closeButton;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Slider volumeSlider;

    private void Awake()
    {
        modalWindow = GetComponent<ModalWindow>();

        soundToggle.onValueChanged.AddListener(OnSoundToggleValueChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChanged);
        closeButton.onClick.AddListener(modalWindow.CloseModal);

        LoadSettings();
    }

    private void OnDestroy() 
    {
        soundToggle.onValueChanged.RemoveAllListeners();
        volumeSlider.onValueChanged.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
    }

    private void LoadSettings()
    {
        float volume = PlayerPrefs.GetFloat(VOLUME_PREF_STRING, 0.5f);
        volumeSlider.value = volume;
        float normalizedVolume = volume / volumeSlider.maxValue;
        PlayerPrefs.SetFloat(VOLUME_PREF_STRING, normalizedVolume);
        bool soundEnabled = PlayerPrefs.GetInt(SOUND_ENABLED_PREF_STRING, 1) == 1;
        soundToggle.isOn = soundEnabled;
        if(soundEnabled)
        {
            AudioManager.Instance.EnabledSound();            
        }
        else
        {
            AudioManager.Instance.DisbledSound();         
        }
        PlayerPrefs.SetInt(SOUND_ENABLED_PREF_STRING, soundEnabled ? 1 : 0);
    }

    private void OnVolumeSliderValueChanged(float value)
    {
        float normalizedVolume = value / volumeSlider.maxValue;
        AudioManager.Instance.SetVolume(normalizedVolume);
        PlayerPrefs.SetFloat(VOLUME_PREF_STRING, value);
    }

    private void OnSoundToggleValueChanged(bool value)
    {
        AudioManager.Instance.PlayMenuNavigationSound();
        if(value)
        {
            AudioManager.Instance.EnabledSound();            
        }
        else
        {
            AudioManager.Instance.DisbledSound();         
        }
        PlayerPrefs.SetInt(SOUND_ENABLED_PREF_STRING, value ? 1 : 0);
    }
}
