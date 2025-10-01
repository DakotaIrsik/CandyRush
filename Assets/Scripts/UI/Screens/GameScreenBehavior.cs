using OctoberStudio.Abilities;
using OctoberStudio.Abilities.UI;
using OctoberStudio.Audio;
using OctoberStudio.Bossfight;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Input;
using OctoberStudio.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;

namespace OctoberStudio
{
    public class GameScreenBehavior : MonoBehaviour
    {
        private Canvas canvas;

        [SerializeField] BackgroundTintUI blackgroundTint;
        [SerializeField] JoystickBehavior joystick;

        [Header("Abilities")]
        [FormerlySerializedAs("abilitiesPanel")]
        [SerializeField] AbilitiesWindowBehavior abilitiesWindow;
        [SerializeField] ChestWindowBehavior chestWindow;
        [SerializeField] List<AbilitiesIndicatorsListBehavior> abilitiesLists;

        public AbilitiesWindowBehavior AbilitiesWindow => abilitiesWindow;
        public ChestWindowBehavior ChestWindow => chestWindow;

        [Header("Top UI")]
        [SerializeField] CanvasGroup topUI;

        [Header("Pause")]
        [SerializeField] Button pauseButton;
        [SerializeField] PauseWindowBehavior pauseWindow;

        [Header("Bossfight")]
        [SerializeField] CanvasGroup bossfightWarning;
        [SerializeField] BossfightHealthbarBehavior bossHealthbar;

        // Injected dependencies
        private IAudioManager audioManager;
        private IInputManager inputManager;
        private IEasingManager easingManager;

        [Inject]
        public void Construct(IAudioManager audioManager, IInputManager inputManager, IEasingManager easingManager)
        {
            this.audioManager = audioManager;
            this.inputManager = inputManager;
            this.easingManager = easingManager;
        }

        private void Awake()
        {
            canvas = GetComponent<Canvas>();

            abilitiesWindow.onPanelClosed += OnAbilitiesPanelClosed;
            abilitiesWindow.onPanelStartedClosing += OnAbilitiesPanelStartedClosing;

            pauseButton.onClick.AddListener(PauseButtonClick);

            pauseWindow.OnStartedClosing += OnPauseWindowStartedClosing;
            pauseWindow.OnClosed += OnPauseWindowClosed;

            chestWindow.OnClosed += OnChestWindowClosed;
        }

        private void Start()
        {
            abilitiesWindow.Init();

            inputManager.InputAsset.UI.Settings.performed += OnSettingsInputClicked;
        }

        private void OnSettingsInputClicked(InputAction.CallbackContext context)
        {
            pauseButton.onClick?.Invoke();
        }

        public void Show(Action onFinish = null)
        {
            canvas.enabled = true;
            onFinish?.Invoke();
        }

        public void Hide(Action onFinish = null)
        {
            canvas.enabled = false;
            onFinish?.Invoke();
        }

        public void ShowBossfightWarning()
        {
            bossfightWarning.gameObject.SetActive(true);
            bossfightWarning.alpha = 0;
            bossfightWarning.DoAlpha(1f, 0.3f);
        }

        public void HideBossFightWarning()
        {
            bossfightWarning.DoAlpha(0f, 0.3f).SetOnFinish(() => bossfightWarning.gameObject.SetActive(false));
            topUI.DoAlpha(0, 0.3f);
        }

        public void ShowBossHealthBar(BossfightData data)
        {
            bossHealthbar.Init(data);
            bossHealthbar.Show();
        }

        public void HideBossHealthbar()
        {
            bossHealthbar.Hide();
            topUI.DoAlpha(1, 0.3f);
        }

        public void LinkBossToHealthbar(EnemyBehavior enemy)
        {
            bossHealthbar.SetBoss(enemy);
        }

        public void ShowAbilitiesPanel(List<AbilityData> abilities, bool isLevelUp)
        {
            abilitiesWindow.SetData(abilities);

            easingManager.DoAfter(0.2f, () =>
            {
                for (int i = 0; i < abilitiesLists.Count; i++)
                {
                    var abilityList = abilitiesLists[i];

                    abilityList.Show();
                    abilityList.Refresh();
                }
            }, true);

            blackgroundTint.Show();

            abilitiesWindow.Show(isLevelUp);

            inputManager.InputAsset.UI.Settings.performed -= OnSettingsInputClicked;
        }

        private void OnAbilitiesPanelStartedClosing()
        {
            for (int i = 0; i < abilitiesLists.Count; i++)
            {
                var abilityList = abilitiesLists[i];

                abilityList.Hide();
            }

            blackgroundTint.Hide();
        }

        private void OnAbilitiesPanelClosed()
        {
            inputManager.InputAsset.UI.Settings.performed += OnSettingsInputClicked;
        }

        public void ShowChestWindow(int tierId, List<AbilityData> abilities, List<AbilityData> selectedAbilities)
        {
            chestWindow.OpenWindow(tierId, abilities, selectedAbilities);

            inputManager.InputAsset.UI.Settings.performed -= OnSettingsInputClicked;
        }

        private void OnChestWindowClosed()
        {
            inputManager.InputAsset.UI.Settings.performed += OnSettingsInputClicked;
        }

        private void PauseButtonClick()
        {
            audioManager.PlaySound(AudioManager.BUTTON_CLICK_HASH);

            joystick.Disable();

            blackgroundTint.Show();
            pauseWindow.Open();

            inputManager.InputAsset.UI.Settings.performed -= OnSettingsInputClicked;
        }
        
        private void OnPauseWindowClosed()
        {
            if(inputManager.ActiveInput == InputType.UIJoystick)
            {
                joystick.Enable();
            }

            inputManager.InputAsset.UI.Settings.performed += OnSettingsInputClicked;
        }

        private void OnPauseWindowStartedClosing()
        {
            blackgroundTint.Hide();
        }
    }
}