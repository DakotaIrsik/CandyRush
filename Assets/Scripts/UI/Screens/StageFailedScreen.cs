using OctoberStudio.Audio;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Input;
using OctoberStudio.Upgrades;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace OctoberStudio.UI
{
    public class StageFailedScreen : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] Button reviveButton;
        [SerializeField] Button exitButton;

        private Canvas canvas;

        private bool revivedAlready = false;

        // Injected dependencies
        private IUpgradesManager upgradesManager;
        private IAudioManager audioManager;
        private IInputManager inputManager;

        [Inject]
        public void Construct(IUpgradesManager upgradesManager, IAudioManager audioManager, IInputManager inputManager)
        {
            this.upgradesManager = upgradesManager;
            this.audioManager = audioManager;
            this.inputManager = inputManager;
        }

        private void Awake()
        {
            canvas = GetComponent<Canvas>();

            reviveButton.onClick.AddListener(ReviveButtonClick);
            exitButton.onClick.AddListener(ExitButtonClick);

            revivedAlready = false;
        }

        public void Show()
        {
            gameObject.SetActive(true);

            canvasGroup.alpha = 0;
            canvasGroup.DoAlpha(1, 0.3f).SetUnscaledTime(true);

            if (upgradesManager.IsUpgradeAquired(UpgradeType.Revive) && !revivedAlready)
            {
                reviveButton.gameObject.SetActive(true);

                EventSystem.current.SetSelectedGameObject(reviveButton.gameObject);
            } else
            {
                reviveButton.gameObject.SetActive(false);
                EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
            }

            inputManager.onInputChanged += OnInputChanged;
        }

        public void Hide(UnityAction onFinish)
        {
            canvasGroup.DoAlpha(0, 0.3f).SetUnscaledTime(true).SetOnFinish(() => {
                gameObject.SetActive(false);
                onFinish?.Invoke();
            });

            inputManager.onInputChanged -= OnInputChanged;
        }

        private void ReviveButtonClick()
        {
            audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);
            Hide(StageController.ResurrectPlayer);
            revivedAlready = true;
        }

        private void ExitButtonClick()
        {
            audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);
            Time.timeScale = 1;
            StageController.ReturnToMainMenu();

            inputManager.onInputChanged -= OnInputChanged;
        }

        private void OnInputChanged(InputType prevInput, InputType inputType)
        {
            if (prevInput == InputType.UIJoystick)
            {
                if (upgradesManager.IsUpgradeAquired(UpgradeType.Revive) && !revivedAlready)
                {
                    EventSystem.current.SetSelectedGameObject(reviveButton.gameObject);
                }
                else
                {
                    EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
                }
            }
        }
    }
}