using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleMobileAds.Api;
using RockPaperScissors.Ads;
using RockPaperScissors.Grids;
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
        public const string HIGH_SCORE_STRING = "highscore";
        private const string GAME_SCENE_STRING = "MainScene";
        private const string MENU_SCENE_STRING = "MenuScene";
        private const float REWARD_MULTIPLIER = 10f;
        public static ApplicationManager Instance;
        private SceneTransitionUI sceneTransitionUI;
        private GridManager gridManager;
        private AdsManager adsManager;
        private int rewardAmount = 0;

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

            sceneTransitionUI = GetComponentInChildren<SceneTransitionUI>();
            GameplayManager.OnGameOver += GameplayManager_OnGameOver;
            adsManager = GetComponent<AdsManager>();
        }

        private void OnEnable() 
        {
            gridManager = FindObjectOfType<GridManager>();
        }

        private void Start() 
        {
            MainMenu.OnStartGameButtonPress += MainMenu_OnStartGameButtonPress;
            MainMenu.OnContinueGameButtonPress += MainMenu_OnContinueGameButtonPress;
            SaveButton.OnSaveButtonPress += SaveButton_OnSaveButtonPress;
            GameMenu.OnStartGameButtonPress += GameMenu_OnStartGameButtonPress;
            StartCoroutine(StartUpRoutine());
        }

        private void OnDisable() 
        {
            MainMenu.OnStartGameButtonPress -= MainMenu_OnStartGameButtonPress;
            MainMenu.OnContinueGameButtonPress -= MainMenu_OnContinueGameButtonPress;
            SaveButton.OnSaveButtonPress -= SaveButton_OnSaveButtonPress;
            GameMenu.OnStartGameButtonPress -= GameMenu_OnStartGameButtonPress;
        }

        public void StartNewGame()
        {
            StartCoroutine(StartGameRoutine());
        }

        public void ContinueGame()
        {
            StartCoroutine(LoadGameRoutine());
        }

        public void ReturnToMenu()
        {
            StartCoroutine(ReturnToMenuRoutine());
        }

        private IEnumerator ReturnToMenuRoutine()
        {
            yield return StartCoroutine(sceneTransitionUI.TransitionOut());
            sceneTransitionUI.StartLoading();
            AsyncOperation asyncLoadScene =  SceneManager.LoadSceneAsync(MENU_SCENE_STRING);
            Debug.Log("Loading Scene...");
            yield return new WaitUntil(() => asyncLoadScene.isDone);
            StartCoroutine(StartUpRoutine());
        }


        private IEnumerator StartUpRoutine()
        {
            sceneTransitionUI.TransitionIn();
            gridManager = FindObjectOfType<GridManager>();
            Debug.Log("Waiting for grid setup...");
            if(gridManager.SetupGridTask != null)
            {
                yield return new WaitUntil(() => gridManager.SetupGridTask.IsCompleted);
            }
            sceneTransitionUI.LoadingCompleted();
        }
        private IEnumerator StartGameRoutine()
        {
            yield return StartCoroutine(LoadGameScene());

            sceneTransitionUI.LoadingCompleted();

            // Trigger new game.
            WaveManager waveManager = FindObjectOfType<WaveManager>();
            waveManager.StartWaveWhenReady();

            // Apply Ad Reward
            if(rewardAmount > 0)
            {
                CurrencyBank currencyBank = FindObjectOfType<CurrencyBank>();
                currencyBank.AddCurrencyToBank(rewardAmount, null);
                // Reset Flag
                rewardAmount = 0;
            }
        }

        private IEnumerator LoadGameRoutine()
        {
            yield return StartCoroutine(LoadGameScene());

            SaveManager saveManager = FindObjectOfType<SaveManager>();
            Task loadTask = saveManager.LoadGameAsync();
            yield return new WaitUntil(() => loadTask.IsCompleted);
            sceneTransitionUI.LoadingCompleted();
        }

        private IEnumerator LoadGameScene()
        {
            yield return StartCoroutine(sceneTransitionUI.TransitionOut());
            sceneTransitionUI.StartLoading();
            AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(GAME_SCENE_STRING);
            Debug.Log("Loading Scene...");
            yield return new WaitUntil(() => asyncLoadScene.isDone);

            sceneTransitionUI.TransitionIn();

            if(gridManager.SetupGridTask != null)
            {
                Debug.Log("Waiting for grid setup...");
                yield return new WaitUntil(() => gridManager.SetupGridTask.IsCompleted);
            }
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

        private void GameMenu_OnStartGameButtonPress()
        {
            StartNewGame();
        }

        private void GameplayManager_OnGameOver(object sender, GameplayManager.OnGameOverEventArgs e)
        {
            if(adsManager == null)
            {
                Debug.LogWarning("No Ads Manager Found");
                return;
            }

            ShowAd();
        }

        private void ShowAd()
        {
            if(!adsManager.adsInitialized)
            {
                return;
            }
            adsManager.ShowRewardedInterstitialAd(OnRewardReceived);
        }

        private void OnRewardReceived(Reward reward)
        {
            rewardAmount = (int)(REWARD_MULTIPLIER * reward.Amount);
            RewardBonusUI rewardBonusUI = FindObjectOfType<RewardBonusUI>(true);
            if(rewardBonusUI != null)
            {
                rewardBonusUI.gameObject.SetActive(true);
                rewardBonusUI.SetRewardAmount(rewardAmount);
            }
            Debug.Log("Reward Received");
        }
    }

}
