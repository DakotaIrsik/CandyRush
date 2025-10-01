using OctoberStudio.Audio;
using UnityEngine;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Configuration for Audio system
    /// This ScriptableObject holds references needed by AudioService
    /// Place in Resources folder as "AudioConfiguration"
    /// </summary>
    [CreateAssetMenu(fileName = "AudioConfiguration.asset", menuName = "DI/Audio Configuration")]
    public class AudioConfiguration : ScriptableObject
    {
        [Header("Audio System")]
        public AudioDatabase audioDatabase;
        public GameObject audioSourcePrefab;
    }
}