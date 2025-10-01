using OctoberStudio.Abilities.UI;
using OctoberStudio.Audio;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Input;
using OctoberStudio.Save;
using OctoberStudio.Vibration;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;

namespace OctoberStudio.UI
{
    public class PauseWindowBehavior : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] Button continueButton;
        [SerializeField] Button exitButton;
        [SerializeField] List<AbilitiesIndicatorsListBehavior> pauseAbilitiesLists;

        [Header("Settings")]
        [SerializeField] ToggleBehavior soundToggle;
        [SerializeField] ToggleBehavior musicToggle;
        [SerializeField] ToggleBehavior vibrationToggle;

        public event UnityAction OnStartedClosing;
        public event UnityAction OnClosed;

        private StageSave stageSave;

        // Injected dependencies
        private IAudioManager audioManager;
        private IVibrationManager vibrationManager;
        private IInputManager inputManager;
        private ISaveManager saveManager;
        private IEasingManager easingManager;

        [Inject]
        public void Construct(IAudioManager audioManager, IVibrationManager vibrationManager,
                             IInputManager inputManager, ISaveManager saveManager, IEasingManager easingManager)
        {
            this.audioManager = audioManager;
            this.vibrationManager = vibrationManager;
            this.inputManager = inputManager;
            this.saveManager = saveManager;
            this.easingManager = easingManager;
        }

        private void Awake()
        {
            continueButton.onClick.AddListener(ContinueButtonClick);
            exitButton.onClick.AddListener(ExitButtonClick);
        }

        private void Start()
        {
            soundToggle.SetToggle(audioManager.SoundVolume != 0);
            musicToggle.SetToggle(audioManager.MusicVolume != 0);
            vibrationToggle.SetToggle(vibrationManager.IsVibrationEnabled);

            soundToggle.onChanged += (soundEnabled) => audioManager.SoundVolume = soundEnabled ? 1 : 0;
            musicToggle.onChanged += (musicEnabled) => audioManager.MusicVolume = musicEnabled ? 1 : 0;
            vibrationToggle.onChanged += (vibrationEnabled) => vibrationManager.IsVibrationEnabled = vibrationEnabled;

            stageSave = saveManager.GetSave<StageSave>("Stage");
        }

        public void Open()
        {
            gameObject.SetActive(true);

            canvasGroup.alpha = 0f;
            canvasGroup.DoAlpha(1, 0.3f).SetUnscaledTime(true);

            Time.timeScale = 0f;

            for (int i = 0; i < pauseAbilitiesLists.Count; i++)
            {
                var abilityList = pauseAbilitiesLists[i];

                abilityList.Show();
                abilityList.Refresh();
            }

            easingManager.DoNextFrame(() => {
                EventSystem.current.SetSelectedGameObject(null);
                soundToggle.Select();
                inputManager.InputAsset.UI.Back.performed += OnBackInputClicked;
            });

            inputManager.onInputChanged += OnInputChanged;
        }

        public void Close()
        {
            canvasGroup.DoAlpha(0, 0.3f).SetUnscaledTime(true).SetOnFinish(() => {
                gameObject.SetActive(false);
                Time.timeScale = 1f;

                inputManager.onInputChanged -= OnInputChanged;
                inputManager.InputAsset.UI.Back.performed -= OnBackInputClicked;

                OnClosed?.Invoke();
            });

            OnStartedClosing?.Invoke();
        }

        private void OnInputChanged(InputType prevInput, InputType inputType)
        {
            if (prevInput == InputType.UIJoystick)
            {
                easingManager.DoNextFrame(soundToggle.Select);
            }
        }

        private void ContinueButtonClick()
        {
            audioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);
            Close();
        }

        private void ExitButtonClick()
        {
            audioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);
            Time.timeScale = 1f;

            stageSave.IsPlaying = false;

            StageController.ReturnToMainMenu();
        }

        private void OnBackInputClicked(InputAction.CallbackContext context)
        {
            continueButton.onClick?.Invoke();
        }

        private void OnDestroy()
        {
            if (inputManager != null)
            {
                inputManager.onInputChanged -= OnInputChanged;
                inputManager.InputAsset.UI.Back.performed -= OnBackInputClicked;
            }
        }
    }
}