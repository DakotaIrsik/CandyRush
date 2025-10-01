using OctoberStudio.UI;
using UnityEngine;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Configuration for Input system
    /// This ScriptableObject holds references needed by InputService
    /// Place in Resources folder as "InputConfiguration"
    /// </summary>
    [CreateAssetMenu(fileName = "InputConfiguration.asset", menuName = "DI/Input Configuration")]
    public class InputConfiguration : ScriptableObject
    {
        [Header("Input Settings")]
        public GameObject highlightsParentPrefab;
    }
}