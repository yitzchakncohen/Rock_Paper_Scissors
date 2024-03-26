using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RockPaperScissors
{
    /// <summary>
    /// Class <c>ApplicationManager</c> is a persistent class that manages the high level state of the application. 
    /// </summary>
    public class ApplicationManager : MonoBehaviour
    {
        private const string GAME_SCENE_STRING = "MainScene";
        private const string Menu_SCENE_STRING = "MenuScene";

        public static ApplicationManager Instance;
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
        }

        private void Start() 
        {
            MainMenu.OnStartGameButtonPress += MainMenu_OnStartGameButtonPress;
            MainMenu.OnContinueGameButtonPress += MainMenu_OnContinueGameButtonPress;
            SaveButton.OnSaveButtonPress += SaveButton_OnSaveButtonPress;
        }

        private void OnDisable() 
        {
            MainMenu.OnStartGameButtonPress -= MainMenu_OnStartGameButtonPress;
            MainMenu.OnContinueGameButtonPress -= MainMenu_OnContinueGameButtonPress;
            SaveButton.OnSaveButtonPress -= SaveButton_OnSaveButtonPress;
        }

        public void StartNewGame()
        {
            StartCoroutine(StartGameRoutineAsync());
        }

        public void ContinueGame()
        {
            StartCoroutine(LoadGameRoutineAsync());
        }

        public void ReturnToMenu()
        {
            SceneManager.LoadScene(Menu_SCENE_STRING);
        }

        private IEnumerator StartGameRoutineAsync()
        {
            AsyncOperation asyncLoadScene =  SceneManager.LoadSceneAsync(GAME_SCENE_STRING);

            while(!asyncLoadScene.isDone)
            {
                yield return null;
            }

            // Trigger new game.
            WaveManager waveManager = FindObjectOfType<WaveManager>();
            waveManager.StartWave(0);
        }

        private IEnumerator LoadGameRoutineAsync()
        {
            AsyncOperation asyncLoadScene =  SceneManager.LoadSceneAsync(GAME_SCENE_STRING);

            while(!asyncLoadScene.isDone)
            {
                yield return null;
            }

            // Clear existing game

            SaveManager saveManager = FindObjectOfType<SaveManager>();
            saveManager.LoadGame();
        }

        private void MainMenu_OnContinueGameButtonPress()
        {
            ContinueGame();
        }

        private void MainMenu_OnStartGameButtonPress()
        {
            StartNewGame();
        }

        private void SaveButton_OnSaveButtonPress()
        {
            SaveManager saveManager = FindObjectOfType<SaveManager>();
            saveManager.SaveGame();
        }
    }

}
