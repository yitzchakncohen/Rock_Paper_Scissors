using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip menuNavigationSound = null;
    [SerializeField] private AudioClip unitSelectionSound = null;
    [SerializeField] private AudioClip unitDeselectionSound = null;
    [SerializeField] private AudioClip unitLevelUpSound = null;
    [SerializeField] private AudioClip unitSpawnSound = null;
    [SerializeField] private AudioClip collectCurrencySound = null;
    private AudioSource audioSource;
    public static AudioManager Instance;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void PlaySoundOneShot(AudioClip audioClip)
    {
        if(audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void PlayMenuNavigationSound()
    {
        PlaySoundOneShot(menuNavigationSound);
    }

    public void PlayUnitSelectionSound()
    {
        PlaySoundOneShot(unitSelectionSound);
    }

    public void PlayUnitDeselectionSound()
    {
        PlaySoundOneShot(unitDeselectionSound);
    }

    public void PlayUnitLevelUpSound()
    {
        PlaySoundOneShot(unitLevelUpSound);
    }

    public void PlayUnitSpawnSound()
    {
        PlaySoundOneShot(unitSpawnSound);
    }

    public void PlayCollectCurrencySound()
    {
        PlaySoundOneShot(collectCurrencySound);
    }
}
