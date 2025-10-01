using OctoberStudio.Abilities;
using OctoberStudio.Audio;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Extensions;
using OctoberStudio.Pool;
using OctoberStudio.Save;
using OctoberStudio.Timeline.Bossfight;
using OctoberStudio.UI;
using UnityEngine;
using UnityEngine.Playables;
using VContainer;

namespace OctoberStudio
{
    /// <summary>
    /// StageController - Updated to use dependency injection
    /// Manages stage progression and game components
    /// </summary>
    public class StageController : MonoBehaviour
    {
        private static StageController instance;

        [SerializeField] StagesDatabase database;
        [SerializeField] PlayableDirector director;
        [SerializeField] EnemiesSpawner spawner;
        [SerializeField] StageFieldManager fieldManager;
        [SerializeField] ExperienceManager experienceManager;
        [SerializeField] DropManager dropManager;
        [SerializeField] AbilityManager abilityManager;
        [SerializeField] PoolsManager poolsManager;
        [SerializeField] WorldSpaceTextManager worldSpaceTextManager;
        [SerializeField] CameraManager cameraManager;

        // Static accessors for backward compatibility (legacy code can still use these)
        public static EnemiesSpawner EnemiesSpawner => instance?.spawner;
        public static ExperienceManager ExperienceManager => instance?.experienceManager;
        public static AbilityManager AbilityManager => instance?.abilityManager;
        public static StageFieldManager FieldManager => instance?.fieldManager;
        public static PlayableDirector Director => instance?.director;
        public static PoolsManager PoolsManager => instance?.poolsManager;
        public static WorldSpaceTextManager WorldSpaceTextManager => instance?.worldSpaceTextManager;
        public static CameraManager CameraController => instance?.cameraManager;
        public static DropManager DropManager => instance?.dropManager;

        // Instance properties for dependency injection
        public EnemiesSpawner EnemiesSpawnerInstance => spawner;
        public ExperienceManager ExperienceManagerInstance => experienceManager;
        public AbilityManager AbilityManagerInstance => abilityManager;
        public StageFieldManager FieldManagerInstance => fieldManager;
        public PlayableDirector DirectorInstance => director;
        public PoolsManager PoolsManagerInstance => poolsManager;
        public WorldSpaceTextManager WorldSpaceTextManagerInstance => worldSpaceTextManager;
        public CameraManager CameraControllerInstance => cameraManager;
        public DropManager DropManagerInstance => dropManager;

        [Header("UI")]
        [SerializeField] GameScreenBehavior gameScreen;
        [SerializeField] StageFailedScreen stageFailedScreen;
        [SerializeField] StageCompleteScreen stageCompletedScreen;

        [Header("Testing")]
        [SerializeField] PresetData testingPreset;

        // Static accessors for backward compatibility
        public static GameScreenBehavior GameScreen => instance?.gameScreen;
        public static StageData Stage { get; private set; }

        // Instance properties for dependency injection
        public GameScreenBehavior GameScreenInstance => gameScreen;
        public StageFailedScreen StageFailedScreenInstance => stageFailedScreen;
        public StageCompleteScreen StageCompletedScreenInstance => stageCompletedScreen;

        private StageSave stageSave;

        // Injected dependencies
        private ISaveManager saveManager;
        private IAudioManager audioManager;
        private ISceneLoader sceneLoader;
        private IEasingManager easingManager;

        [Inject]
        public void Construct(ISaveManager saveManager, IAudioManager audioManager, ISceneLoader sceneLoader, IEasingManager easingManager)
        {
            this.saveManager = saveManager;
            this.audioManager = audioManager;
            this.sceneLoader = sceneLoader;
            this.easingManager = easingManager;
        }

        private void Awake()
        {
            instance = this;

            stageSave = saveManager.GetSave<StageSave>("Stage");
        }

        private void Start()
        {
            Stage = database.GetStage(stageSave.SelectedStageId);

            director.playableAsset = Stage.Timeline;

            spawner.Init(director);
            experienceManager.Init(testingPreset);
            dropManager.Init();
            fieldManager.Init(Stage, director);
            abilityManager.Init(testingPreset, PlayerBehavior.Player.Data);
            cameraManager.Init(Stage);

            PlayerBehavior.Player.onPlayerDied += OnGameFailed;

            director.stopped += TimelineStopped;
            if (testingPreset != null) {
                director.time = testingPreset.StartTime; 
            } else
            {
                var time = stageSave.Time;

                var bossClips = director.GetClips<BossTrack, Boss>();

                for(int i = 0; i < bossClips.Count; i++)
                {
                    var bossClip = bossClips[i];

                    if(time >= bossClip.start && time <= bossClip.end)
                    {
                        time = (float) bossClip.start;
                        break;
                    }
                }

                director.time = time;
            }

            director.Play();

            if (Stage.UseCustomMusic)
            {
                ChangeMusic(Stage.MusicName);
            }
        }

        private void TimelineStopped(PlayableDirector director)
        {
            if (gameObject.activeSelf)
            {
                if (stageSave.MaxReachedStageId < stageSave.SelectedStageId + 1 && stageSave.SelectedStageId + 1 < database.StagesCount)
                {
                    stageSave.SetMaxReachedStageId(stageSave.SelectedStageId + 1);
                }

                stageSave.IsPlaying = false;
                saveManager.Save(true);

                gameScreen.Hide();
                stageCompletedScreen.Show();
                Time.timeScale = 0;
            }
        }

        private void OnGameFailed()
        {
            Time.timeScale = 0;

            stageSave.IsPlaying = false;
            saveManager.Save(true);

            gameScreen.Hide();
            stageFailedScreen.Show();
        }

        public static void ResurrectPlayer()
        {
            if (instance != null)
            {
                instance.EnemiesSpawnerInstance.DealDamageToAllEnemies(PlayerBehavior.Player.Damage * 1000);
                instance.GameScreenInstance.Show();
                PlayerBehavior.Player.Revive();
                Time.timeScale = 1;
            }
        }

        // Instance method for dependency injection
        public void ResurrectPlayerInstance()
        {
            EnemiesSpawnerInstance.DealDamageToAllEnemies(PlayerBehavior.Player.Damage * 1000);
            GameScreenInstance.Show();
            PlayerBehavior.Player.Revive();
            Time.timeScale = 1;
        }

        public static void ReturnToMainMenu()
        {
            if (instance != null)
            {
                instance.sceneLoader.LoadMainMenu();
            }
        }

        private void ChangeMusic(string musicName)
        {
            var currentMusic = audioManager.CurrentMusic;
            if (currentMusic != null)
            {
                var oldMusic = currentMusic;
                easingManager.DoFloat(oldMusic.volume, 0, 0.3f, (volume) => oldMusic.volume = volume)
                    .SetOnFinish(() => oldMusic.Stop());
            }

            var newMusic = audioManager.PlayMusic(musicName.GetHashCode());
            if (newMusic != null)
            {
                var volume = newMusic.volume;
                newMusic.volume = 0;
                easingManager.DoFloat(0, volume, 0.3f, (vol) => newMusic.volume = vol);
            }
        }

        private void OnDisable()
        {
            director.stopped -= TimelineStopped;
        }
    }
}