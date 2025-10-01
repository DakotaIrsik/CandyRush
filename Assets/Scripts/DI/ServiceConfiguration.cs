using OctoberStudio.Audio;
using OctoberStudio.Save;
using UnityEngine;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Configuration for core services
    /// This ScriptableObject holds references needed by pure DI services
    /// Place in Resources folder as "ServiceConfiguration"
    /// </summary>
    [CreateAssetMenu(fileName = "ServiceConfiguration.asset", menuName = "DI/Service Configuration")]
    public class ServiceConfiguration : ScriptableObject
    {
        [Header("Sub-Configurations")]
        public AudioConfiguration audioConfig;
        public SaveConfiguration saveConfig;
        public CurrenciesConfiguration currenciesConfig;
        public UpgradesConfiguration upgradesConfig;
        public InputConfiguration inputConfig;

        [Header("Pool Configuration")]
        public GameObject[] poolPrefabs;
        public int defaultPoolSize = 10;

        [Header("Debug Settings")]
        public bool enableDebugLogs = false;
        public bool enablePerformanceMetrics = false;
    }
}