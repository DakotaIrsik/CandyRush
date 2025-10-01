using OctoberStudio.Abilities;
using OctoberStudio.Audio;
using OctoberStudio.Currency;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Input;
using OctoberStudio.Pool;
using OctoberStudio.Save;
using OctoberStudio.Upgrades;
using OctoberStudio.Vibration;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OctoberStudio.DI
{
    /// <summary>
    /// RootLifetimeScope - Persists across ALL scenes
    /// Creates and manages core services via pure dependency injection
    /// No reliance on scene GameObjects - everything is created by the container
    /// </summary>
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Load configuration from Resources with organized paths and fallback
            var config = ResourcePathHelper.LoadCoreConfig<ServiceConfiguration>("ServiceConfiguration");
            if (config == null)
            {
                Debug.LogWarning("[RootLifetimeScope] ServiceConfiguration not found in Resources. Using defaults.");
                config = ScriptableObject.CreateInstance<ServiceConfiguration>();
            }

            // Register configuration
            builder.RegisterInstance(config);

            // SaveManager as pure service
            builder.Register<ISaveManager>(container =>
            {
                var saveConfig = ResourcePathHelper.LoadCoreConfig<SaveConfiguration>("SaveConfiguration");
                if (saveConfig == null)
                {
                    Debug.LogWarning("[RootLifetimeScope] SaveConfiguration not found in Resources. Using defaults.");
                    return new SaveService(SaveType.SaveFile, false, true, 30f);
                }
                return new SaveService(saveConfig.saveType, saveConfig.clearSave, saveConfig.autoSaveEnabled, saveConfig.autoSaveDelay);
            }, Lifetime.Singleton);

            // AudioManager as pure service
            builder.Register<IAudioManager>(container =>
            {
                var saveManager = container.Resolve<ISaveManager>();
                var audioConfig = ResourcePathHelper.LoadSystemConfig<AudioConfiguration>("AudioConfiguration");
                if (audioConfig == null)
                {
                    Debug.LogWarning("[RootLifetimeScope] AudioConfiguration not found in Resources. Using defaults.");
                    return new AudioService(saveManager, LoadAudioDatabase(), CreateDefaultAudioSourcePrefab());
                }
                var prefab = audioConfig.audioSourcePrefab ?? CreateDefaultAudioSourcePrefab();
                var database = audioConfig.audioDatabase ?? LoadAudioDatabase();
                return new AudioService(saveManager, database, prefab);
            }, Lifetime.Singleton);

            // Create updater for AudioService
            builder.RegisterBuildCallback(container =>
            {
                var updaterGO = new GameObject("[AudioSystemUpdater]");
                DontDestroyOnLoad(updaterGO);
                var updater = updaterGO.AddComponent<AudioSystemUpdater>();
                container.Inject(updater);
            });

            // Input and Vibration services
            builder.Register<IInputManager>(container =>
            {
                var saveManager = container.Resolve<ISaveManager>();
                return new InputService(saveManager, null); // No highlights for now
            }, Lifetime.Singleton);

            // Create updater for InputService
            builder.RegisterBuildCallback(container =>
            {
                var updaterGO = new GameObject("[InputServiceUpdater]");
                DontDestroyOnLoad(updaterGO);
                var updater = updaterGO.AddComponent<InputServiceUpdater>();
                container.Inject(updater);
            });

            builder.Register<IVibrationManager, VibrationService>(Lifetime.Singleton);

            // EasingManager as pure service
            builder.Register<IEasingManager, EasingService>(Lifetime.Singleton);

            // Currencies service as pure service
            builder.Register<ICurrenciesManager>(container =>
            {
                var saveManager = container.Resolve<ISaveManager>();
                var currenciesConfig = ResourcePathHelper.LoadSystemConfig<CurrenciesConfiguration>("CurrenciesConfiguration");
                if (currenciesConfig?.currenciesDatabase != null)
                {
                    return new CurrenciesService(currenciesConfig.currenciesDatabase, saveManager);
                }
                Debug.LogWarning("[RootLifetimeScope] CurrenciesConfiguration not found in Resources or database not assigned.");
                // Return a valid fallback instead of null
                return new CurrenciesService(null, saveManager);
            }, Lifetime.Singleton);

            builder.Register<IUpgradesManager>(container =>
            {
                var saveManager = container.Resolve<ISaveManager>();
                var upgradesConfig = ResourcePathHelper.LoadSystemConfig<UpgradesConfiguration>("UpgradesConfiguration");
                if (upgradesConfig?.upgradesDatabase != null)
                {
                    return new UpgradesService(saveManager, upgradesConfig.upgradesDatabase);
                }
                Debug.LogWarning("[RootLifetimeScope] UpgradesConfiguration not found in Resources or database not assigned.");
                return null;
            }, Lifetime.Singleton);

            // Pools service as pure service
            builder.Register<IPoolsManager>(container =>
            {
                var poolConfig = ResourcePathHelper.LoadCoreConfig<PoolConfiguration>("PoolConfiguration");
                return new PoolsService(poolConfig);
            }, Lifetime.Singleton);

            // Global game state
            builder.Register<IGlobalGameState, GlobalGameState>(Lifetime.Singleton);

            // EventSystem management service
            builder.Register<EventSystemService>(Lifetime.Singleton);

            // Scene loading service - depends on core services so register here
            builder.Register<ISceneLoader>(container =>
            {
                var saveManager = container.Resolve<ISaveManager>();
                var currenciesManager = container.Resolve<ICurrenciesManager>();
                // Try to resolve ProjectSettings, fallback to null if not available
                ProjectSettings projectSettings = null;
                try
                {
                    projectSettings = container.Resolve<ProjectSettings>();
                }
                catch (VContainer.VContainerException)
                {
                    // ProjectSettings not available yet, will be resolved later
                }
                var eventSystemService = container.Resolve<EventSystemService>();
                return new SceneLoaderService(saveManager, currenciesManager, projectSettings, eventSystemService);
            }, Lifetime.Singleton);
        }

        private GameObject CreateDefaultAudioSourcePrefab()
        {
            var prefab = new GameObject("AudioSource");
            prefab.AddComponent<AudioSource>();
            return prefab;
        }

        private AudioDatabase LoadAudioDatabase()
        {
            // Try to load from organized path with fallback to legacy
            return ResourcePathHelper.LoadData<AudioDatabase>(ResourcePathHelper.AUDIO_DATA_PATH, "AudioDatabase");
        }
    }
}