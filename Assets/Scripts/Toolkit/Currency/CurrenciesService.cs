using OctoberStudio.DI;
using OctoberStudio.Save;
using UnityEngine;

namespace OctoberStudio.Currency
{
    public class CurrenciesService : ICurrenciesManager
    {
        private readonly CurrenciesDatabase database;
        private readonly ISaveManager saveManager;

        public CurrenciesService(CurrenciesDatabase database, ISaveManager saveManager)
        {
            this.database = database;
            this.saveManager = saveManager;
        }

        public void Init()
        {
            // Pure service - no MonoBehaviour lifecycle management needed
        }

        public Sprite GetIcon(string currencyId)
        {
            if (database == null) return null;

            var data = database.GetCurrency(currencyId);
            return data?.Icon;
        }

        public string GetName(string currencyId)
        {
            if (database == null) return null;

            var data = database.GetCurrency(currencyId);
            return data?.Name;
        }

        public CurrencySave GetCurrency(string currencyId)
        {
            if (saveManager == null) return null;
            return saveManager.GetSave<CurrencySave>(currencyId);
        }
    }
}