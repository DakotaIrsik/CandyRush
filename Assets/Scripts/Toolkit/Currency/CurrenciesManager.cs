using OctoberStudio.DI;
using OctoberStudio.Save;
using UnityEngine;

namespace OctoberStudio.Currency
{
    /// <summary>
    /// DEPRECATED: Use CurrenciesService via dependency injection instead
    /// This MonoBehaviour wrapper is kept for backward compatibility only
    /// </summary>
    [System.Obsolete("Use CurrenciesService via dependency injection instead")]
    public class CurrenciesManager : MonoBehaviour, ICurrenciesManager
    {
        [SerializeField] CurrenciesDatabase database;

        public void Init()
        {
            // Legacy initialization - service pattern doesn't need this
            DontDestroyOnLoad(gameObject);
        }

        public Sprite GetIcon(string currencyId)
        {
            var data = database.GetCurrency(currencyId);

            if(data == null) return null;

            return data.Icon;
        }

        public string GetName(string currencyId)
        {
            var data = database.GetCurrency(currencyId);

            if (data == null) return null;

            return data.Name;
        }

        public CurrencySave GetCurrency(string currencyId)
        {
            // This is a legacy implementation - proper service would access through SaveManager
            // For now, return null since this deprecated class shouldn't be used
            Debug.LogWarning($"CurrenciesManager.GetCurrency() called for {currencyId} - use CurrenciesService instead");
            return null;
        }
    }
}