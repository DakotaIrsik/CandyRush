using OctoberStudio.Currency;
using OctoberStudio.Save;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Service implementation for handling scene loading operations.
    /// Replaces static GameController scene loading methods with dependency injection.
    /// </summary>
    public class SceneLoaderService : ISceneLoader
    {
        private readonly ISaveManager saveManager;
        private readonly ICurrenciesManager currenciesManager;
        private readonly ProjectSettings projectSettings;
        private readonly EventSystemService eventSystemService;
        private MonoBehaviour coroutineRunner;

        private StageSave stageSave;
        private CurrencySave tempGold;
        private CurrencySave gold;

        public SceneLoaderService(ISaveManager saveManager, ICurrenciesManager currenciesManager, ProjectSettings projectSettings, EventSystemService eventSystemService)
        {
            this.saveManager = saveManager;
            this.currenciesManager = currenciesManager;
            this.projectSettings = projectSettings;
            this.eventSystemService = eventSystemService;


            var sceneLoaderGO = new GameObject("[SceneLoaderSystem]");
            Object.DontDestroyOnLoad(sceneLoaderGO);
            coroutineRunner = sceneLoaderGO.AddComponent<SceneLoaderSystemUpdater>();
        }

        public void LoadStage()
        {
            InitializeSaveData();

            if (stageSave != null && stageSave.ResetStageData && tempGold != null)
            {
                tempGold.Withdraw(tempGold.Amount);
            }

            coroutineRunner.StartCoroutine(StageLoadingCoroutine());
            saveManager?.Save(false);
        }

        public void LoadMainMenu()
        {
            InitializeSaveData();

            if (gold != null && tempGold != null)
            {
                gold.Deposit(tempGold.Amount);
                tempGold.Withdraw(tempGold.Amount);
            }

            coroutineRunner.StartCoroutine(MainMenuLoadingCoroutine());
            saveManager?.Save(false);

            // EventSystem is now managed by DI container - no cleanup needed
        }

        private void InitializeSaveData()
        {
            if (saveManager != null)
            {
                if (stageSave == null)
                    stageSave = saveManager.GetSave<StageSave>("Stage");
                if (tempGold == null)
                    tempGold = currenciesManager.GetCurrency("TempGold");
                if (gold == null)
                    gold = currenciesManager.GetCurrency("Gold");
            }
        }

        private IEnumerator StageLoadingCoroutine()
        {
            var loadingSceneName = GetLoadingScreenSceneName();
            var mainMenuSceneName = GetMainMenuSceneName();
            var gameSceneName = GetGameSceneName();

            if (loadingSceneName != null)
            {
                yield return LoadAsyncScene(loadingSceneName, LoadSceneMode.Additive);
            }

            // Unload Main Menu scene
            if (mainMenuSceneName != null)
            {
                var unloadMainMenuOp = UnloadAsyncScene(mainMenuSceneName);
                if (unloadMainMenuOp != null)
                {
                    yield return unloadMainMenuOp;
                }
            }

            // Load Game scene as Single (replaces active scene)
            if (gameSceneName != null)
            {
                yield return LoadAsyncScene(gameSceneName, LoadSceneMode.Single);
            }

            if (loadingSceneName != null)
            {
                var unloadOp = UnloadAsyncScene(loadingSceneName);
                if (unloadOp != null)
                {
                    yield return unloadOp;
                }
            }
        }

        private IEnumerator MainMenuLoadingCoroutine()
        {
            var loadingSceneName = GetLoadingScreenSceneName();
            var mainMenuSceneName = GetMainMenuSceneName();
            var gameSceneName = GetGameSceneName();

            if (loadingSceneName != null)
            {
                yield return LoadAsyncScene(loadingSceneName, LoadSceneMode.Additive);
            }

            if (gameSceneName != null)
            {
                var unloadGameOp = UnloadAsyncScene(gameSceneName);
                if (unloadGameOp != null)
                {
                    yield return unloadGameOp;
                }
            }

            if (mainMenuSceneName != null)
            {
                yield return LoadAsyncScene(mainMenuSceneName, LoadSceneMode.Single);
            }

            if (loadingSceneName != null)
            {
                var unloadLoadingOp = UnloadAsyncScene(loadingSceneName);
                if (unloadLoadingOp != null)
                {
                    yield return unloadLoadingOp;
                }
            }
        }

        private string GetLoadingScreenSceneName()
        {
            if (projectSettings != null && SceneExists(projectSettings.LoadingSceneName))
            {
                return projectSettings.LoadingSceneName;
            }
            else if (SceneExists("Loading Screen"))
            {
                return "Loading Screen";
            }
            else
            {
                Debug.LogWarning("Loading screen scene not found. Please add a loading screen scene to the project settings or ensure it exists in the build settings.");
                return null;
            }
        }

        private string GetMainMenuSceneName()
        {
            if (projectSettings != null && SceneExists(projectSettings.MainMenuSceneName))
            {
                return projectSettings.MainMenuSceneName;
            }
            else if (SceneExists("Main Menu"))
            {
                return "Main Menu";
            }
            else
            {
                Debug.LogWarning("Main menu scene not found. Please add a main menu scene to the project settings or ensure it exists in the build settings.");
                return null;
            }
        }

        private string GetGameSceneName()
        {
            if (projectSettings != null && SceneExists(projectSettings.GameSceneName))
            {
                return projectSettings.GameSceneName;
            }
            else if (SceneExists("Game"))
            {
                return "Game";
            }
            else
            {
                Debug.LogWarning("Game scene not found. Please add a game scene to the project settings or ensure it exists in the build settings.");
                return null;
            }
        }

        private static bool SceneExists(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return false;

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (sceneNameFromPath == sceneName)
                {
                    return true;
                }
            }
            return false;
        }

        private static IEnumerator LoadAsyncScene(string sceneName, LoadSceneMode mode)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
            if (asyncLoad == null)
            {
                Debug.LogError($"[SceneLoaderService] Failed to start loading scene '{sceneName}'");
                yield break;
            }

            // Wait for scene to load completely
            yield return asyncLoad;

            // Wait multiple frames to ensure all GameObject initialization and UI Toolkit jobs complete
            // This prevents threading issues with font asset initialization
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();
        }

        private static AsyncOperation UnloadAsyncScene(string sceneName)
        {
            // Check if scene is actually loaded before attempting to unload
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.LogWarning($"[SceneLoaderService] Cannot unload scene '{sceneName}' - scene is not currently loaded");
                return null;
            }

            return SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}