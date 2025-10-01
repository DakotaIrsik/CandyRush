using OctoberStudio.Audio;
using OctoberStudio.Currency;
using OctoberStudio.DI;
using OctoberStudio.Input;
using OctoberStudio.UI;
using OctoberStudio.Upgrades;
using OctoberStudio.Upgrades.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OctoberStudio
{
    /// <summary>
    /// MainMenuLifetimeScope - Specific to Main Menu scene
    /// Only registers components that exist in the Main Menu scene
    /// Inherits core services from RootLifetimeScope
    /// </summary>
    public class MainMenuLifetimeScope : LifetimeScope
    {
        [Header("UI Components for Injection")]
        [SerializeField] private LobbyWindowBehavior lobbyWindow;
        [SerializeField] private CurrencyScreenIncicatorBehavior currencyIndicator;
        [SerializeField] private UpgradesWindowBehavior upgradesWindow;
        [SerializeField] private SettingsWindowBehavior settingsWindow;
        [SerializeField] private MainMenuScreenBehavior mainMenuScreen;

        [Header("Game Components for Injection")]
        [SerializeField] private GameController gameController;

        protected override void Configure(IContainerBuilder builder)
        {
            // Register UI components for dependency injection
            if (lobbyWindow != null)
                builder.RegisterComponent(lobbyWindow);

            if (currencyIndicator != null)
                builder.RegisterComponent(currencyIndicator);

            if (upgradesWindow != null)
                builder.RegisterComponent(upgradesWindow);

            if (settingsWindow != null)
                builder.RegisterComponent(settingsWindow);

            if (mainMenuScreen != null)
                builder.RegisterComponent(mainMenuScreen);

            // Register GameController for dependency injection
            if (gameController != null)
                builder.RegisterComponent(gameController);
        }
    }
}
