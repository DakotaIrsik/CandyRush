using OctoberStudio.Abilities;
using OctoberStudio.Abilities.UI;
using OctoberStudio.Currency;
using OctoberStudio.Easing;
using OctoberStudio.Pool;
using OctoberStudio.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OctoberStudio
{
    /// <summary>
    /// GameLifetimeScope - Specific to Game scene
    /// Registers components that exist in the Game scene for dependency injection
    /// Inherits core services from RootLifetimeScope
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Core Game Components")]
        [SerializeField] private PlayerBehavior playerBehavior;
        [SerializeField] private GameScreenBehavior gameScreen;

        [Header("Manager Components")]
        [SerializeField] private AbilityManager abilityManager;
        [SerializeField] private ExperienceManager experienceManager;
        [SerializeField] private DropManager dropManager;
        [SerializeField] private EnemiesSpawner enemiesSpawner;
        [SerializeField] private CameraManager cameraManager;
        [SerializeField] private StageFieldManager stageFieldManager;
        [SerializeField] private WorldSpaceTextManager worldSpaceTextManager;
        [SerializeField] private PoolsManager poolsManager;
        [SerializeField] private SafeAreaManager safeAreaManager;
        [SerializeField] private EasingManager easingManager;
        [SerializeField] private StageController stageController;

        [Header("UI Components")]
        [SerializeField] private CurrencyScreenIncicatorBehavior currencyIndicator;
        [SerializeField] private BackgroundTintUI backgroundTint;
        [SerializeField] private UITimer uiTimer;
        [SerializeField] private ExperienceUI experienceUI;
        [SerializeField] private StageFailedScreen stageFailedScreen;
        [SerializeField] private StageCompleteScreen stageCompleteScreen;
        [SerializeField] private JoystickBehavior joystick;

        [Header("UI Windows")]
        [SerializeField] private AbilitiesWindowBehavior abilitiesWindow;
        [SerializeField] private PauseWindowBehavior pauseWindow;
        [SerializeField] private ChestWindowBehavior chestWindow;

        [Header("Gameplay Components")]
        [SerializeField] private PlayerEnemyCollisionHelper playerEnemyCollisionHelper;

        protected override void Awake()
        {
            base.Awake();
            Debug.Log("[GameLifetimeScope] Awake called - GameLifetimeScope is initializing");
        }

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("[GameLifetimeScope] Configure called - Registering Game scene components");
            // Register core game components
            if (playerBehavior != null)
                builder.RegisterComponent(playerBehavior);

            if (gameScreen != null)
                builder.RegisterComponent(gameScreen);

            // Register manager components
            if (abilityManager != null)
                builder.RegisterComponent(abilityManager).AsImplementedInterfaces();

            if (experienceManager != null)
                builder.RegisterComponent(experienceManager).AsImplementedInterfaces();

            if (dropManager != null)
                builder.RegisterComponent(dropManager).AsImplementedInterfaces();

            if (enemiesSpawner != null)
                builder.RegisterComponent(enemiesSpawner).AsImplementedInterfaces();

            if (cameraManager != null)
                builder.RegisterComponent(cameraManager).AsImplementedInterfaces();

            if (stageFieldManager != null)
                builder.RegisterComponent(stageFieldManager).AsImplementedInterfaces();

            if (worldSpaceTextManager != null)
                builder.RegisterComponent(worldSpaceTextManager).AsImplementedInterfaces();

            if (poolsManager != null)
                builder.RegisterComponent(poolsManager).AsImplementedInterfaces();

            if (safeAreaManager != null)
                builder.RegisterComponent(safeAreaManager);

            if (easingManager != null)
                builder.RegisterComponent(easingManager);

            if (stageController != null)
                builder.RegisterComponent(stageController);

            // Register UI components
            if (currencyIndicator != null)
                builder.RegisterComponent(currencyIndicator);

            if (backgroundTint != null)
                builder.RegisterComponent(backgroundTint);

            if (uiTimer != null)
                builder.RegisterComponent(uiTimer);

            if (experienceUI != null)
                builder.RegisterComponent(experienceUI);

            if (stageFailedScreen != null)
                builder.RegisterComponent(stageFailedScreen);

            if (stageCompleteScreen != null)
                builder.RegisterComponent(stageCompleteScreen);

            if (joystick != null)
                builder.RegisterComponent(joystick);

            if (abilitiesWindow != null)
                builder.RegisterComponent(abilitiesWindow);

            if (pauseWindow != null)
                builder.RegisterComponent(pauseWindow);

            if (chestWindow != null)
                builder.RegisterComponent(chestWindow);

            // Register gameplay components
            if (playerEnemyCollisionHelper != null)
                builder.RegisterComponent(playerEnemyCollisionHelper);
        }
    }
}
