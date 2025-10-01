using UnityEngine;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Helper class for loading Resources with organized paths and fallback support
    /// Supports gradual migration from flat structure to domain-organized structure
    /// </summary>
    public static class ResourcePathHelper
    {
        // Configuration paths
        public const string CORE_CONFIG_PATH = "Configuration/Core/";
        public const string SYSTEMS_CONFIG_PATH = "Configuration/Systems/";
        public const string SCENES_CONFIG_PATH = "Configuration/Scenes/";

        // Data paths
        public const string AUDIO_DATA_PATH = "Data/Audio/";
        public const string CURRENCIES_DATA_PATH = "Data/Currencies/";
        public const string UPGRADES_DATA_PATH = "Data/Upgrades/";
        public const string ABILITIES_DATA_PATH = "Data/Abilities/";
        public const string ENEMIES_DATA_PATH = "Data/Enemies/";
        public const string STAGES_DATA_PATH = "Data/Stages/";

        // Localization paths
        public const string LOCALIZATION_PATH = "Localization/";

        // Settings paths
        public const string SETTINGS_PATH = "Settings/";

        /// <summary>
        /// Loads a core configuration asset with fallback to legacy path
        /// </summary>
        public static T LoadCoreConfig<T>(string assetName) where T : Object
        {
            return LoadWithFallback<T>(CORE_CONFIG_PATH + assetName, assetName);
        }

        /// <summary>
        /// Loads a system configuration asset with fallback to legacy path
        /// </summary>
        public static T LoadSystemConfig<T>(string assetName) where T : Object
        {
            return LoadWithFallback<T>(SYSTEMS_CONFIG_PATH + assetName, assetName);
        }

        /// <summary>
        /// Loads a data asset from specific domain folder
        /// </summary>
        public static T LoadData<T>(string domainPath, string assetName) where T : Object
        {
            return LoadWithFallback<T>(domainPath + assetName, assetName);
        }

        /// <summary>
        /// Loads an asset with fallback support for gradual migration
        /// </summary>
        public static T LoadWithFallback<T>(string newPath, string legacyPath) where T : Object
        {
            // Try new organized path first
            var asset = Resources.Load<T>(newPath);

            if (asset == null)
            {
                // Fall back to legacy flat path
                asset = Resources.Load<T>(legacyPath);

                if (asset != null)
                {
                    Debug.LogWarning($"[ResourcePathHelper] Using legacy path '{legacyPath}'. " +
                                   $"Please move asset to new path: '{newPath}'");
                }
                else
                {
                    Debug.LogError($"[ResourcePathHelper] Asset not found at '{newPath}' or legacy path '{legacyPath}'");
                }
            }

            return asset;
        }

        /// <summary>
        /// Loads a localization asset for specific language
        /// </summary>
        public static TextAsset LoadLocalization(string languageCode, string fileName)
        {
            string path = $"{LOCALIZATION_PATH}{languageCode}/{fileName}";
            return Resources.Load<TextAsset>(path);
        }

        /// <summary>
        /// Loads a settings preset
        /// </summary>
        public static T LoadSettings<T>(string category, string presetName) where T : Object
        {
            string path = $"{SETTINGS_PATH}{category}/{presetName}";
            return Resources.Load<T>(path);
        }

        /// <summary>
        /// Validates that an asset exists at the expected new path
        /// Useful for migration verification
        /// </summary>
        public static bool ValidateNewPath<T>(string newPath) where T : Object
        {
            var asset = Resources.Load<T>(newPath);
            return asset != null;
        }

        /// <summary>
        /// Gets migration info for an asset
        /// </summary>
        public static string GetMigrationInfo(string assetName, string suggestedNewPath)
        {
            return $"Asset '{assetName}' should be moved to: Resources/{suggestedNewPath}";
        }
    }
}