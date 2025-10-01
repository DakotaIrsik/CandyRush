using OctoberStudio.Currency;
using UnityEngine;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Configuration for Currencies system
    /// This ScriptableObject holds references needed by CurrenciesService
    /// Place in Resources folder as "CurrenciesConfiguration"
    /// </summary>
    [CreateAssetMenu(fileName = "CurrenciesConfiguration.asset", menuName = "DI/Currencies Configuration")]
    public class CurrenciesConfiguration : ScriptableObject
    {
        [Header("Currencies")]
        public CurrenciesDatabase currenciesDatabase;
    }
}