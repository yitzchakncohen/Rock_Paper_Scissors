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
    [SerializeField] private AudioClip unitMovementSound = null;
    [SerializeField] private AudioClip collectCurrencySound = null;
    [SerializeField] private AudioClip glueTrapSound = null;
    [SerializeField] private AudioClip trampolineTrapSound = null;
    [SerializeField] private AudioClip rockAttackSound = null;
    [SerializeField] private AudioClip paperAttackSound = null;
    [SerializeField] private AudioClip scissorsAttackSound = null;
    [SerializeField] private AudioClip enemyWaveSound = null;
    [SerializeField] private AudioClip gameOverSound = null;

    private AudioSource audioSource;
    private bool soundEnabled = true;
    public bool SoundEnabled => soundEnabled;
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
        if(audioClip != null && soundEnabled)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }

    public void EnabledSound()
    {
        soundEnabled = true;
    }

    public void DisbledSound()
    {
        soundEnabled = false;
    }

    public void SetVolume(float value)
    {
        audioSource.volume = value;
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

    public void PlayUnitMovementSound()
    {
        PlaySoundOneShot(unitMovementSound);
    }

    public void PlayCollectCurrencySound()
    {
        PlaySoundOneShot(collectCurrencySound);
    }

    public void PlayGlueTrapSound()
    {
        PlaySoundOneShot(glueTrapSound);
    }
    public void PlayTrampolineTrapSound()
    {
        PlaySoundOneShot(trampolineTrapSound);
    }

    public void PlayRockAttackSound()
    {
        PlaySoundOneShot(rockAttackSound);
    }

    public void PlayPaperAttackSound()
    {
        PlaySoundOneShot(paperAttackSound);
    }

    public void PlayScissorsAttackSound()
    {
        PlaySoundOneShot(scissorsAttackSound);
    }

    public void PlayEnemyWaveSound()
    {
        PlaySoundOneShot(enemyWaveSound);
    }

    public void PlayGameOverSound()
    {
        PlaySoundOneShot(gameOverSound);
    }
}
