using OctoberStudio.Audio;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace OctoberStudio.UI
{
    public class StageCompleteScreen : MonoBehaviour
    {
        private Canvas canvas;
        private IAudioManager audioManager;
        private IInputManager inputManager;
        private ISceneLoader sceneLoader;

        private static readonly int STAGE_COMPLETE_HASH = "Stage Complete".GetHashCode();

        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] Button button;

        [Inject]
        public void Construct(IAudioManager audioManager, IInputManager inputManager, ISceneLoader sceneLoader)
        {
            this.audioManager = audioManager;
            this.inputManager = inputManager;
            this.sceneLoader = sceneLoader;
        }

        private void Awake()
        {
            canvas = GetComponent<Canvas>();

            button.onClick.AddListener(OnButtonClicked);
        }

        public void Show(UnityAction onFinish = null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DoAlpha(1f, 0.3f).SetUnscaledTime(true).SetOnFinish(onFinish);

            gameObject.SetActive(true);

            audioManager.PlaySound(STAGE_COMPLETE_HASH);

            EventSystem.current.SetSelectedGameObject(button.gameObject);

            inputManager.onInputChanged += OnInputChanged;
        }

        public void Hide(UnityAction onFinish = null)
        {
            canvasGroup.DoAlpha(0f, 0.3f).SetUnscaledTime(true).SetOnFinish(() => {
                gameObject.SetActive(false);
                onFinish?.Invoke();
            });

            inputManager.onInputChanged -= OnInputChanged;
        }

        private void OnButtonClicked()
        {
            audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);
            Time.timeScale = 1;
            sceneLoader.LoadMainMenu();

            inputManager.onInputChanged -= OnInputChanged;
        }

        private void OnInputChanged(InputType prevInput, InputType inputType)
        {
            if (prevInput == InputType.UIJoystick)
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
        }
    }
}