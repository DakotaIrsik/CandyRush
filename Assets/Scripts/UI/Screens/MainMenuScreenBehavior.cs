using OctoberStudio.Audio;
using OctoberStudio.Easing;
using OctoberStudio.Upgrades.UI;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace OctoberStudio.UI
{
    public class MainMenuScreenBehavior : MonoBehaviour
    {
        private Canvas canvas;
        private IAudioManager audioManager;

        [SerializeField] LobbyWindowBehavior lobbyWindow;
        [SerializeField] UpgradesWindowBehavior upgradesWindow;
        [SerializeField] SettingsWindowBehavior settingsWindow;
        [SerializeField] CharactersWindowBehavior charactersWindow;

        [Inject]
        public void Construct(IAudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            lobbyWindow.Init(ShowUpgrades, ShowSettings, ShowCharacters);
            upgradesWindow.Init(HideUpgrades);
            settingsWindow.Init(HideSettings);
            charactersWindow.Init(HideCharacters);
        }

        private void ShowUpgrades()
        {
            audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);

            lobbyWindow.Close();
            upgradesWindow.Open();
        }

        private void HideUpgrades()
        {
            audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);

            upgradesWindow.Close();
            lobbyWindow.Open();
        }

        private void ShowCharacters()
        {
            audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);

            lobbyWindow.Close();
            charactersWindow.Open();
        }

        private void HideCharacters()
        {
            audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);

            charactersWindow.Close();
            lobbyWindow.Open();
        }

        private void ShowSettings()
        {
            audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);

            lobbyWindow.Close();
            settingsWindow.Open();
        }

        private void HideSettings()
        {
            audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);

            settingsWindow.Close();
            lobbyWindow.Open();
        }

        private void OnDestroy()
        {
            charactersWindow.Clear();
            upgradesWindow.Clear();
        }
    }
}