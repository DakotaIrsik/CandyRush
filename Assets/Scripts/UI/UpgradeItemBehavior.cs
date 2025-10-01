using OctoberStudio.Audio;
using OctoberStudio.DI;
using OctoberStudio.Save;
using OctoberStudio.UI;
using OctoberStudio.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace OctoberStudio.Upgrades.UI
{
    public class UpgradeItemBehavior : MonoBehaviour
    {
        [SerializeField] RectTransform rect;
        public RectTransform Rect => rect;

        [Header("Info")]
        [SerializeField] Image iconImage;
        [SerializeField] TMP_Text titleLabel;
        [SerializeField] TMP_Text levelLabel;

        [Header("Button")]
        [SerializeField] Button upgradeButton;
        [SerializeField] Sprite enabledButtonSprite;
        [SerializeField] Sprite disabledButtonSprite;

        [Space]
        [SerializeField] ScalingLabelBehavior costLabel;
        [SerializeField] GameObject upgradedLabel;

        public CurrencySave GoldCurrency { get; private set; }

        public UpgradeData Data { get; private set; }
        public int UpgradeLevelId { get; private set; }

        public Selectable Selectable => upgradeButton;
        public bool IsSelected { get; private set; }

        public UnityAction<UpgradeItemBehavior> onNavigationSelected;

        // Injected dependencies
        private ISaveManager saveManager;
        private IUpgradesManager upgradesManager;
        private IAudioManager audioManager;

        [Inject]
        public void Construct(ISaveManager saveManager, IUpgradesManager upgradesManager, IAudioManager audioManager)
        {
            this.saveManager = saveManager;
            this.upgradesManager = upgradesManager;
            this.audioManager = audioManager;
        }

        private void Start()
        {
            upgradeButton.onClick.AddListener(UpgradeButtonClick);
        }

        public void Init(UpgradeData data, int levelId)
        {
            if(GoldCurrency == null)
            {
                if (saveManager != null)
                {
                    GoldCurrency = saveManager.GetSave<CurrencySave>("gold");
                    GoldCurrency.onGoldAmountChanged += OnGoldAmountChanged;
                }
                else
                {
                    // Debug.LogWarning("[UpgradeItemBehavior] SaveManager not injected - will be properly configured in Game scene");
                }
            }

            Data = data;
            UpgradeLevelId = levelId;

            RedrawVisuals();
        }

        private void RedrawVisuals()
        {
            if(UpgradeLevelId >= Data.LevelsCount - 1)
            {
                levelLabel.text = "Max Level";
            }
            else
            {
                levelLabel.text = $"LEVEL {UpgradeLevelId + 1}";
            }
            
            titleLabel.text = Data.Title;
            iconImage.sprite = Data.Icon;

            RedrawButton();
        }

        private void RedrawButton()
        {
            if (UpgradeLevelId >= Data.LevelsCount)
            {
                costLabel.gameObject.SetActive(false);
                upgradedLabel.gameObject.SetActive(true);

                upgradeButton.interactable = false;
                upgradeButton.image.sprite = disabledButtonSprite;
            }
            else
            {
                costLabel.gameObject.SetActive(true);
                upgradedLabel.gameObject.SetActive(false);

                var level = Data.GetLevel(UpgradeLevelId);
                costLabel.SetAmount(level.Cost);

                if (GoldCurrency?.CanAfford(level.Cost) == true)
                {
                    upgradeButton.interactable = true;
                    upgradeButton.image.sprite = enabledButtonSprite;
                } else
                {
                    upgradeButton.interactable = false;
                    upgradeButton.image.sprite = disabledButtonSprite;
                }
            }
        }

        private void UpgradeButtonClick()
        {
            var level = Data.GetLevel(UpgradeLevelId);

            upgradesManager.IncrementUpgradeLevel(Data.UpgradeType);
            UpgradeLevelId++;
            GoldCurrency.Withdraw(level.Cost);

            RedrawVisuals();

            audioManager.PlaySound(AudioService.BUTTON_CLICK_HASH);

            EventSystem.current.SetSelectedGameObject(upgradeButton.gameObject);
        }

        private void OnGoldAmountChanged(int amount)
        {
            RedrawButton();
        }

        public void Select()
        {
            EventSystem.current.SetSelectedGameObject(upgradeButton.gameObject);
        }

        public void Unselect()
        {
            IsSelected = false;
        }

        private void Update()
        {
            if (!IsSelected && EventSystem.current.currentSelectedGameObject == upgradeButton.gameObject)
            {
                IsSelected = true;

                onNavigationSelected?.Invoke(this);
            }
            else if (IsSelected && EventSystem.current.currentSelectedGameObject != upgradeButton.gameObject)
            {
                IsSelected = false;
            }
        }

        public void Clear()
        {
            if (GoldCurrency != null)
            {
                GoldCurrency.onGoldAmountChanged -= OnGoldAmountChanged;
            }
        }
    }
}