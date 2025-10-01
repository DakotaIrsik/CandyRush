using OctoberStudio.Audio;
using OctoberStudio.Currency;
using OctoberStudio.DI;
using OctoberStudio.Input;
using OctoberStudio.Save;
using OctoberStudio.UI;
using OctoberStudio.Upgrades;
using OctoberStudio.Upgrades.UI;
using OctoberStudio.Vibration;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OctoberStudio.DI
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameController gameController;

        protected override void Configure(IContainerBuilder builder)
        {
            // Register global services that persist across scenes
            builder.RegisterInstance(GameController.ProjectSettings);

            // Legacy MonoBehaviour managers - these will be converted to pure services
            // TODO: Remove these once converted to pure services in RootLifetimeScope
            // CurrenciesManager now handled by RootLifetimeScope as pure service
            builder.RegisterComponent(gameController.UpgradesManager).As<IUpgradesManager>();

            // Core services are now handled by RootLifetimeScope as pure services
            // No need to register AudioManager, SaveManager, VibrationManager, InputManager here


            // Register all UI components that need injection in this scene
            builder.RegisterComponentInHierarchy<LobbyWindowBehavior>();
            builder.RegisterComponentInHierarchy<MainMenuScreenBehavior>();
            builder.RegisterComponentInHierarchy<UpgradesWindowBehavior>();

            // Scene loading service is now registered in RootLifetimeScope

            // Global state holder is handled by RootLifetimeScope
        }
    }
}