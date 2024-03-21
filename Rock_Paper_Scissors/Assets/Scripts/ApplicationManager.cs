using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
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

        public void StartNewGame()
        {
            StartCoroutine(StartGameRoutineAsync());
        }

        public void ContinueGame()
        {
            StartCoroutine(LoadGameRoutineAsync());
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
    }

}
