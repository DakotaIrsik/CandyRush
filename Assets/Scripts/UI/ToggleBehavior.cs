using OctoberStudio.Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace OctoberStudio.UI
{
    public class ToggleBehavior : MonoBehaviour
    {
        [SerializeField] Image toggleImage;
        [SerializeField] Sprite onSprite;
        [SerializeField] Sprite offSprite;

        [Space]
        [SerializeField] Button toggleButton;

        private IAudioManager audioManager;

        public event UnityAction<bool> onChanged;

        public bool IsOn { get; private set; }

        [Inject]
        public void Construct(IAudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        private void Awake()
        {
            toggleButton.onClick.AddListener(OnToggleClicked);
        }

        public void SetToggle(bool value)
        {
            IsOn = value;
            toggleImage.sprite = IsOn ? onSprite : offSprite;

            onChanged?.Invoke(value);
        }

        private void OnToggleClicked()
        {
            SetToggle(!IsOn);

            audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);
        }

        public void Select()
        {
            EventSystem.current.SetSelectedGameObject(toggleButton.gameObject);
        }
    }
}