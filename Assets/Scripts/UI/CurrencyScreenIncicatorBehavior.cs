using OctoberStudio.DI;
using OctoberStudio.Save;
using OctoberStudio.UI;
using UnityEngine;
using VContainer;

namespace OctoberStudio.Currency
{
    public class CurrencyScreenIncicatorBehavior : ScalingLabelBehavior
    {
        [Tooltip("The unique identificator of the currency that is attached to this ui. There must be an entry with the same id in the Currencies Database")]
        [SerializeField] string currencyID;

        public CurrencySave Currency { get; private set; }

        // Injected dependencies
        private ISaveManager saveManager;
        private ICurrenciesManager currenciesManager;

        [Inject]
        public void Construct(ISaveManager saveManager, ICurrenciesManager currenciesManager)
        {
            this.saveManager = saveManager;
            this.currenciesManager = currenciesManager;
        }

        private void Start()
        {
            if (saveManager == null)
            {
                Debug.LogError($"[CurrencyScreenIncicatorBehavior] SaveManager is null - dependency injection failed for {gameObject.name}");
                return;
            }

            if (currenciesManager == null)
            {
                Debug.LogError($"[CurrencyScreenIncicatorBehavior] CurrenciesManager is null - dependency injection failed for {gameObject.name}");
                return;
            }

            Currency = saveManager.GetSave<CurrencySave>(currencyID);

            SetAmount(Currency.Amount);

            icon.sprite = currenciesManager.GetIcon(currencyID);

            Currency.onGoldAmountChanged += SetAmount;
        }

        private void OnDestroy()
        {
            Currency.onGoldAmountChanged -= SetAmount;
        }
    }
}