using OctoberStudio.Save;
using UnityEngine;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Configuration for Save system
    /// This ScriptableObject holds settings needed by SaveService
    /// Place in Resources folder as "SaveConfiguration"
    /// </summary>
    [CreateAssetMenu(fileName = "SaveConfiguration.asset", menuName = "DI/Save Configuration")]
    public class SaveConfiguration : ScriptableObject
    {
        [Header("Save Settings")]
        public SaveType saveType = SaveType.SaveFile;
        public bool clearSave = false;
        public bool autoSaveEnabled = true;
        public float autoSaveDelay = 30f;
    }
}