using OctoberStudio.Upgrades;
using UnityEngine;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Configuration for Upgrades system
    /// This ScriptableObject holds references needed by UpgradesService
    /// Place in Resources folder as "UpgradesConfiguration"
    /// </summary>
    [CreateAssetMenu(fileName = "UpgradesConfiguration.asset", menuName = "DI/Upgrades Configuration")]
    public class UpgradesConfiguration : ScriptableObject
    {
        [Header("Upgrades")]
        public UpgradesDatabase upgradesDatabase;
    }
}