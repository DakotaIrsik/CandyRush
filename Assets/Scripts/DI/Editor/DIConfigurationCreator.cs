using UnityEngine;
using UnityEditor;
using System.IO;
using OctoberStudio.Audio;

namespace OctoberStudio.DI.Editor
{
    public static class DIConfigurationCreator
    {
        private const string RESOURCES_PATH = "Assets/Resources";

        [MenuItem("DI/Create All Resource Configurations", false, 0)]
        public static void CreateAllConfigurations()
        {
            // Ensure Resources folder exists
            if (!AssetDatabase.IsValidFolder(RESOURCES_PATH))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            // Create sub-configurations
            CreateConfiguration<AudioConfiguration>("AudioConfiguration");
            CreateConfiguration<SaveConfiguration>("SaveConfiguration");
            CreateConfiguration<CurrenciesConfiguration>("CurrenciesConfiguration");
            CreateConfiguration<UpgradesConfiguration>("UpgradesConfiguration");
            CreateConfiguration<InputConfiguration>("InputConfiguration");

            // Create main ServiceConfiguration last
            CreateConfiguration<ServiceConfiguration>("ServiceConfiguration");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[DI] Created all configuration assets in Resources folder!");

            // Select the main ServiceConfiguration
            var mainConfig = AssetDatabase.LoadAssetAtPath<ServiceConfiguration>($"{RESOURCES_PATH}/ServiceConfiguration.asset");
            if (mainConfig != null)
            {
                Selection.activeObject = mainConfig;
                EditorGUIUtility.PingObject(mainConfig);
            }
        }


        private static T CreateConfiguration<T>(string fileName) where T : ScriptableObject
        {
            string assetPath = $"{RESOURCES_PATH}/{fileName}.asset";

            // Check if asset already exists
            T existingAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (existingAsset != null)
            {
                Debug.Log($"[DI] {fileName}.asset already exists, skipping creation.");
                return existingAsset;
            }

            // Create new asset
            T newAsset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(newAsset, assetPath);

            Debug.Log($"[DI] Created {fileName}.asset in Resources folder.");
            return newAsset;
        }
    }
}