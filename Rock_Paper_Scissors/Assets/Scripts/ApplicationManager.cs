using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleMobileAds.Api;
using RockPaperScissors.Ads;
using RockPaperScissors.Grids;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.UI;
using RockPaperScissors.UI.Buttons;
using RockPaperScissors.UI.Menus;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace RockPaperScissors
{
    public enum GameMode
    {
        Endless,
        Level
    }


    /// <summary>
    /// Class <c>ApplicationManager</c> is a persistent class that manages the high level state of the application.
    /// </summary>
    public class ApplicationManager : MonoBehaviour
    {
        public const string HIGH_SCORE_STRING = "highscore";
        private const string GAME_SCENE_STRING = "MainScene";
        private const string MENU_SCENE_STRING = "MenuScene";
        private const float REWARD_MULTIPLIER = 10f;
        private const int TARGET_FRAME_RATE = 30;
        public static ApplicationManager Instance;
        private SceneTransitionUI sceneTransitionUI;
        private AdsManager adsManager;
        private DeviceReviewsManager deviceReviewsManager;
        private int rewardAmount = 0;
        public GameMode GameMode => gameMode;
        private GameMode gameMode;
        public int Level => level;
        private int level = -1;
        [SerializeField] private LevelData endlessModeLevelData;
        [SerializeField] private List<LevelData> levelDataList;

        void Awake()
        {
            Application.targetFrameRate = TARGET_FRAME_RATE;
            OnDemandRendering.renderFrameInterval = Application.targetFrameRate/30;
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
            adsManager = GetComponent<AdsManager>();
            deviceReviewsManager = GetComponent<DeviceReviewsManager>();
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Start() 
        {
            MainMenu.OnStartEndlessGameButtonPress += MainMenu_OnStartEndlessGameButtonPress;
            MainMenu.OnStartLevelGameButtonPress += MainMenu_OnStartLevelGameButtonPress;
            MainMenu.OnContinueGameButtonPress += MainMenu_OnContinueGameButtonPress;
            SaveButton.OnSaveButtonPress += SaveButton_OnSaveButtonPress;
            GameMenu.OnStartEndlessGameButtonPress += GameMenu_OnStartEndlessGameButtonPress;
            AdModal.OnWatchButtonClick += AdModal_OnWatchButtonClick;
            AdModal.OnSkipButtonClick += AdModal_OnSkipButtonClick;
            GameplayManager.OnGameOver += GameplayManager_OnGameOver;
            StartCoroutine(StartUpRoutine());
        }

        private void OnDisable() 
        {
            MainMenu.OnStartEndlessGameButtonPress -= MainMenu_OnStartEndlessGameButtonPress;
            MainMenu.OnStartLevelGameButtonPress -= MainMenu_OnStartLevelGameButtonPress;
            MainMenu.OnContinueGameButtonPress -= MainMenu_OnContinueGameButtonPress;
            SaveButton.OnSaveButtonPress -= SaveButton_OnSaveButtonPress;
            GameMenu.OnStartEndlessGameButtonPress -= GameMenu_OnStartEndlessGameButtonPress;
            AdModal.OnWatchButtonClick -= AdModal_OnWatchButtonClick;
            AdModal.OnSkipButtonClick -= AdModal_OnSkipButtonClick;
            GameplayManager.OnGameOver -= GameplayManager_OnGameOver;
        }

        public void StartNewGame(GameMode gameMode)
        {
            this.gameMode = gameMode;
            TimeScaleManager.ResetTimeScale();
            StartCoroutine(StartGameRoutine());
        }

        public void ContinueGame()
        {
            TimeScaleManager.ResetTimeScale();
            StartCoroutine(LoadGameRoutine());
        }

        public void ReturnToMenu()
        {
            TimeScaleManager.ResetTimeScale();
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

            yield return StartCoroutine(sceneTransitionUI.LoadingCompletedRoutine());

            TitlePageAnimation titlePageAnimation = FindObjectOfType<TitlePageAnimation>();
            if(titlePageAnimation != null)
            {
                StartCoroutine(titlePageAnimation.AnimationRoutine());
            }
        }

        private IEnumerator StartGameRoutine()
        {
            if(gameMode == GameMode.Endless)
            {
                yield return StartCoroutine(LoadGameScene(endlessModeLevelData));
            }
            else
            {
                level = 1;
                yield return StartCoroutine(LoadGameScene(levelDataList[0]));
            }

            yield return StartCoroutine(sceneTransitionUI.LoadingCompletedRoutine());

            // Trigger new game.
            WaveManager waveManager = FindObjectOfType<WaveManager>();
            waveManager.StartWaveWhenReady(levelDataList[0].wave, gameMode);

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
            SaveManager saveManager = FindObjectOfType<SaveManager>();
            SaveData saveData;
            bool loadSuccessful = saveManager.LoadSaveData(out saveData);
            if(loadSuccessful)
            {
                gameMode = saveData.GameMode;
                level = saveData.Level;
                if(saveData.GameMode == GameMode.Endless)
                {
                    yield return StartCoroutine(LoadGameScene(endlessModeLevelData));
                }
                else
                {
                    yield return StartCoroutine(LoadGameScene(levelDataList[saveData.Level - 1]));
                }
                Task loadTask = saveManager.LoadGameAsync(saveData);
                yield return new WaitUntil(() => loadTask.IsCompleted);
                yield return StartCoroutine(sceneTransitionUI.LoadingCompletedRoutine());          
            }
            else
            {
                Debug.LogError("Could not load game.");
            }
        }

        private IEnumerator LoadGameScene(LevelData levelData)
        {
            yield return StartCoroutine(sceneTransitionUI.TransitionOut());
            sceneTransitionUI.StartLoading();
            AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(GAME_SCENE_STRING);
            Debug.Log("Loading Scene...");
            yield return new WaitUntil(() => asyncLoadScene.isDone);

            sceneTransitionUI.TransitionIn();

            GridManager gridManager = FindObjectOfType<GridManager>();
            Task GridSetup = gridManager.SetupGrid(levelData.width, levelData.height, levelData.spawnPoints);
            if(gridManager != null)
            {
                Debug.Log("Waiting for grid setup...");
                yield return new WaitUntil(() => GridSetup.IsCompleted);
            }
            else
            {
                Debug.LogError("No grid found in scene");
            }
        }

        private void MainMenu_OnContinueGameButtonPress()
        {
            ContinueGame();
        }

        private void MainMenu_OnStartEndlessGameButtonPress()
        {
            StartNewGame(GameMode.Endless);
        }

        private void MainMenu_OnStartLevelGameButtonPress()
        {
            StartNewGame(GameMode.Level);
        }

        private void SaveButton_OnSaveButtonPress()
        {
            SaveManager saveManager = FindObjectOfType<SaveManager>();
            saveManager.SaveGame();
        }

        private void GameMenu_OnStartEndlessGameButtonPress()
        {
            StartNewGame(GameMode.Endless);
        }

        private void ShowAd()
        {
            if(adsManager == null)
            {
                Debug.LogWarning("No Ads Manager Found");
                return;
            }
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

        private void GameplayManager_OnGameOver(object sender, GameplayManager.OnGameOverEventArgs e)
        {
#if UNITY_ANDROID
            deviceReviewsManager.RequestReviewAsync();
#endif
        }

        private void AdModal_OnWatchButtonClick(object sender, GameplayManager.OnGameOverEventArgs e)
        {
            ShowAd();
            LanuchReview();
        }
        
        private void AdModal_OnSkipButtonClick(object sender, GameplayManager.OnGameOverEventArgs e)
        {
            LanuchReview();
        }

        private void LanuchReview()
        {
#if UNITY_ANDROID
            if(deviceReviewsManager != null)
            {
                deviceReviewsManager.LaunchReview();
            }
            else
            {
                Debug.LogError("No Review Manger Found");
            }
#endif
        }
    }

}
