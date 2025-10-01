using OctoberStudio.Audio;
using OctoberStudio.Currency;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Input;
using OctoberStudio.Save;
using OctoberStudio.Upgrades;
using OctoberStudio.Vibration;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OctoberStudio
{

    public class GameController : MonoBehaviour
    {
        protected static readonly string MAIN_MENU_MUSIC_NAME = "Main Menu Music";

        private static GameController instance;

        [SerializeField] protected UpgradesManager upgradesManager;
        [SerializeField] protected ProjectSettings projectSettings;

        // Public properties for ProjectLifetimeScope registration
        public UpgradesManager UpgradesManager => upgradesManager;

        // Static accessor for ProjectSettings (needed by static scene loading methods)
        public static ProjectSettings ProjectSettings => instance?.projectSettings;

        // LEGACY: Static accessors for backward compatibility during DI migration
        // TODO: Remove these once all code is converted to use dependency injection
        public static ISaveManager SaveManager { get; private set; }
        public static IAudioManager AudioManager { get; private set; }
        public static IVibrationManager VibrationManager { get; private set; }
        public static IInputManager InputManager { get; private set; }

        // Injected dependencies
        private ISaveManager saveManager;
        private IAudioManager audioManager;
        private IInputManager inputManager;
        private IVibrationManager vibrationManager;
        private IEasingManager easingManager;

        [Inject]
        public void Construct(ISaveManager saveManager, IAudioManager audioManager, IInputManager inputManager, IVibrationManager vibrationManager, IEasingManager easingManager)
        {
            this.saveManager = saveManager;
            this.audioManager = audioManager;
            this.inputManager = inputManager;
            this.vibrationManager = vibrationManager;
            this.easingManager = easingManager;

            // Update static references for transitional compatibility
            SaveManager = saveManager;
            AudioManager = audioManager;
            InputManager = inputManager;
            VibrationManager = vibrationManager;
        }

        public static CurrencySave Gold { get; private set; }
        public static CurrencySave TempGold { get; private set; }

        public static AudioSource Music { get; private set; }

        private static StageSave stageSave;

        // Indicates that the main menu is just loaded, and not exited from the game scene
        public static bool FirstTimeLoaded { get; private set; }

        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(this);

                FirstTimeLoaded = false;

                return;
            }

            instance = this;

            FirstTimeLoaded = true;


            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 120;
        }

        protected virtual void Start()
        {
            // Initialize save data
            if (saveManager != null)
            {
                stageSave = saveManager.GetSave<StageSave>("Stage");
                Gold = saveManager.GetSave<CurrencySave>("Gold");
                TempGold = saveManager.GetSave<CurrencySave>("TempGold");
            }

            // Start the main menu music when the game first loads
            if (FirstTimeLoaded && audioManager != null)
            {
                Music = audioManager.PlayMusic(MAIN_MENU_MUSIC_NAME.GetHashCode());
            }
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        protected virtual void MusicStartWebGL(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            InputManager.InputAsset.UI.Click.performed -= MusicStartWebGL;

            Music = instance.audioManager.PlayMusic(MAIN_MENU_MUSIC_NAME.GetHashCode());
        }
#endif
        public static void ChangeMusic(string musicName)
        {
            if (Music != null)
            {
                var oldMusic = Music;
                oldMusic.DoVolume(0, 0.3f).SetOnFinish(() => oldMusic.Stop());
            }

            Music = instance.audioManager.PlayMusic(musicName.GetHashCode());

            if(Music != null)
            {
                var volume = Music.volume;
                Music.volume = 0;
                Music.DoVolume(volume, 0.3f);
            }
        }

        public static void ChangeMusic(SoundContainer music)
        {
            if (Music != null)
            {
                var oldMusic = Music;
                oldMusic.DoVolume(0, 0.3f).SetOnFinish(() => oldMusic.Stop());
            }

            Music = music.Play(true);

            if (Music != null)
            {
                var volume = Music.volume;
                Music.volume = 0;
                Music.DoVolume(volume, 0.3f);
            }
        }

        public static void RegisterInputManager(IInputManager inputManager)
        {
            InputManager = inputManager;
        }

        public static void RegisterSaveManager(ISaveManager saveManager)
        {
            SaveManager = saveManager;
        }

        public static void RegisterVibrationManager(IVibrationManager vibrationManager)
        {
            VibrationManager = vibrationManager;
        }

        public static void RegisterAudioManager(IAudioManager audioManager)
        {
            AudioManager = audioManager;
        }

        public static void LoadStage()
        {
            // Initialize save data if not already initialized
            if (instance.saveManager != null)
            {
                if (stageSave == null)
                    stageSave = instance.saveManager.GetSave<StageSave>("Stage");
                if (TempGold == null)
                    TempGold = instance.saveManager.GetSave<CurrencySave>("TempGold");
            }

            if(stageSave != null && stageSave.ResetStageData && TempGold != null)
            {
                TempGold.Withdraw(TempGold.Amount);
            }

            instance.StartCoroutine(StageLoadingCoroutine());

            instance.saveManager?.Save(false);
        }

        public static void LoadMainMenu()
        {
            // Initialize save data if not already initialized
            if (instance.saveManager != null)
            {
                if (Gold == null)
                    Gold = instance.saveManager.GetSave<CurrencySave>("Gold");
                if (TempGold == null)
                    TempGold = instance.saveManager.GetSave<CurrencySave>("TempGold");
            }

            if (Gold != null && TempGold != null)
            {
                Gold.Deposit(TempGold.Amount);
                TempGold.Withdraw(TempGold.Amount);
            }

            if (instance != null) instance.StartCoroutine(MainMenuLoadingCoroutine());

            instance.saveManager?.Save(false);
        }

        protected static string GetLoadingScreenSceneName()
        {
            if (ProjectSettings != null && SceneExists(ProjectSettings.LoadingSceneName))
            {
                return ProjectSettings.LoadingSceneName;
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

        protected static string GetMainMenuSceneName()
        {
            if (ProjectSettings != null && SceneExists(ProjectSettings.MainMenuSceneName))
            {
                return ProjectSettings.MainMenuSceneName;
            }
            else if (SceneExists("Main Menu"))
            {
                return "Main Menu";
            }
            else
            {
                Debug.LogError("Main menu scene not found. Please add a main menu scene to the project settings or ensure it exists in the build settings.");
                return null;
            }
        }

        protected static string GetGameSceneName()
        {
            if (ProjectSettings != null && SceneExists(ProjectSettings.GameSceneName))
            {
                return ProjectSettings.GameSceneName;
            }
            else if (SceneExists("Game"))
            {
                return "Game";
            }
            else
            {
                Debug.LogError("Game scene not found. Please add a game scene to the project settings or ensure it exists in the build settings.");
                return null;
            }
        }

        protected static IEnumerator StageLoadingCoroutine()
        {
            var loadingSceneName = GetLoadingScreenSceneName();
            var mainMenuSceneName = GetMainMenuSceneName();
            var gameSceneName = GetGameSceneName();

            if (loadingSceneName != null)
            {
                yield return LoadAsyncScene(loadingSceneName, LoadSceneMode.Additive);
            }

            yield return UnloadAsyncScene(mainMenuSceneName);
            yield return LoadAsyncScene(gameSceneName, LoadSceneMode.Single);
        }

        protected static IEnumerator MainMenuLoadingCoroutine()
        {
            var loadingSceneName = GetLoadingScreenSceneName();
            var mainMenuSceneName = GetMainMenuSceneName();
            var gameSceneName = GetGameSceneName();

            if (loadingSceneName != null)
            {
                yield return LoadAsyncScene(loadingSceneName, LoadSceneMode.Additive);
            }

            yield return UnloadAsyncScene(gameSceneName);
            yield return LoadAsyncScene(mainMenuSceneName, LoadSceneMode.Single);

            if (StageController.Stage.UseCustomMusic)
            {
                ChangeMusic(MAIN_MENU_MUSIC_NAME);
            }
        }

        protected static bool SceneExists(string sceneName)
        {
#if UNITY_EDITOR
            return SceneExistsInAssets(sceneName);
#else
            return SceneExistsInBuildSettings(sceneName);
#endif
        }

        protected static bool SceneExistsInBuildSettings(string sceneName)
        {
            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < sceneCount; i++)
            {
                string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (name == sceneName)
                    return true;
            }

            return false;
        }

#if UNITY_EDITOR

        public static bool SceneExistsInAssets(string sceneName)
        {
            string[] guids = AssetDatabase.FindAssets($"t:Scene {sceneName}");
            return guids.Any(guid =>
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                return System.IO.Path.GetFileNameWithoutExtension(path) == sceneName;
            });
        }

#endif

        protected static IEnumerator UnloadAsyncScene(string sceneName)
        {
            var asyncLoad = SceneManager.UnloadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            //wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        protected static IEnumerator LoadAsyncScene(string sceneName, LoadSceneMode loadSceneMode)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            asyncLoad.allowSceneActivation = false;
            //wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                //scene has loaded as much as possible,
                // the last 10% can't be multi-threaded
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        protected virtual void OnApplicationFocus(bool focus)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (focus) { 
                easingManager.DoAfter(0.1f, () => { 
                    if (!Music.isPlaying)
                    {
                        Music = AudioManager.AudioDatabase.Music.Play(true);
                    }
                });
            } 
#endif
        }
    }
}