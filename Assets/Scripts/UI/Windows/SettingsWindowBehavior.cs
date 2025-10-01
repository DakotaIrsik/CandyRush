using OctoberStudio.Audio;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Input;
using OctoberStudio.Vibration;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;

namespace OctoberStudio.UI
{
    public class SettingsWindowBehavior : MonoBehaviour
    {
        [SerializeField] ToggleBehavior soundToggle;
        [SerializeField] ToggleBehavior musicToggle;
        [SerializeField] ToggleBehavior vibrationToggle;

        [Space]
        [SerializeField] Button backButton;
        [SerializeField] Button exitButton;

        // Injected dependencies
        private IAudioManager audioManager;
        private IVibrationManager vibrationManager;
        private IInputManager inputManager;
        private IEasingManager easingManager;

        [Inject]
        public void Construct(IAudioManager audioManager, IVibrationManager vibrationManager, IInputManager inputManager, IEasingManager easingManager)
        {
            this.audioManager = audioManager;
            this.vibrationManager = vibrationManager;
            this.inputManager = inputManager;
            this.easingManager = easingManager;
        }

        private void Start()
        {
            easingManager.DoNextFrame().SetOnFinish(InitToggles);
        }

        private void InitToggles()
        {
            soundToggle.SetToggle(audioManager.SoundVolume != 0);
            musicToggle.SetToggle(audioManager.MusicVolume != 0);
            vibrationToggle.SetToggle(vibrationManager.IsVibrationEnabled);

            soundToggle.onChanged += (soundEnabled) => audioManager.SoundVolume = soundEnabled ? 1 : 0;
            musicToggle.onChanged += (musicEnabled) => audioManager.MusicVolume = musicEnabled ? 1 : 0;
            vibrationToggle.onChanged += (vibrationEnabled) => vibrationManager.IsVibrationEnabled = vibrationEnabled;
        }

        public void Init(UnityAction onBackButtonClicked)
        {
            backButton.onClick.AddListener(onBackButtonClicked);

#if (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL) && !UNITY_EDITOR
            exitButton.gameObject.SetActive(false);
#else
            exitButton.gameObject.SetActive(true);
            exitButton.onClick.AddListener(OnExitButtonClicked);
#endif
        }

        public void Open()
        {
            gameObject.SetActive(true);
            easingManager.DoNextFrame(() => {
                soundToggle.Select();
                inputManager.InputAsset.UI.Back.performed += OnBackInputClicked;
            });
            inputManager.onInputChanged += OnInputChanged;
        }

        public void Close()
        {
            gameObject.SetActive(false);

            inputManager.InputAsset.UI.Back.performed -= OnBackInputClicked;
            inputManager.onInputChanged -= OnInputChanged;
        }

        private void OnBackInputClicked(InputAction.CallbackContext context)
        {
            backButton.onClick?.Invoke();
        }

        private void OnInputChanged(InputType prevInput, InputType inputType)
        {
            if (prevInput == InputType.UIJoystick)
            {
                easingManager.DoNextFrame(soundToggle.Select);
            }
        }

        private void OnExitButtonClicked()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}