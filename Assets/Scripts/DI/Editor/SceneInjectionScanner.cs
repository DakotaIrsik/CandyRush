using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using VContainer;
using VContainer.Unity;
using OctoberStudio;

namespace OctoberStudio.DI.Editor
{
    public static class SceneInjectionScanner
    {
        [MenuItem("DI/Scan Current Scene for Injection Needs", false, 300)]
        public static void ScanSceneForInjection()
        {
            var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            Debug.Log($"[DI Scanner] === Scanning {sceneName} Scene ===");

            // Find all MonoBehaviours in the scene
            var allComponents = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            var componentsWithInject = new Dictionary<System.Type, List<MonoBehaviour>>();
            var managerTypes = new List<MonoBehaviour>();
            var uiComponents = new List<MonoBehaviour>();
            var gameplayComponents = new List<MonoBehaviour>();

            foreach (var component in allComponents)
            {
                if (component == null) continue;

                var type = component.GetType();

                // Skip Unity internal components
                if (type.Namespace != null && type.Namespace.StartsWith("UnityEngine")) continue;
                if (type.Namespace != null && type.Namespace.StartsWith("Unity.")) continue;
                if (type.Namespace != null && type.Namespace.StartsWith("TMPro")) continue;

                // Check for [Inject] methods
                var injectMethods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(m => m.GetCustomAttribute<InjectAttribute>() != null)
                    .ToList();

                if (injectMethods.Any())
                {
                    if (!componentsWithInject.ContainsKey(type))
                        componentsWithInject[type] = new List<MonoBehaviour>();
                    componentsWithInject[type].Add(component);
                }

                // Categorize components
                var typeName = type.Name;
                if (typeName.Contains("Manager") || typeName.Contains("Controller") || typeName.Contains("Spawner"))
                {
                    managerTypes.Add(component);
                }
                else if (typeName.Contains("Window") || typeName.Contains("Screen") || typeName.Contains("UI") ||
                         typeName.Contains("Button") || typeName.Contains("Panel"))
                {
                    uiComponents.Add(component);
                }
                else if (typeName.Contains("Player") || typeName.Contains("Enemy") || typeName.Contains("Ability") ||
                         typeName.Contains("Stage") || typeName.Contains("Camera"))
                {
                    gameplayComponents.Add(component);
                }
            }

            // Report findings
            Debug.Log($"[DI Scanner] Found {componentsWithInject.Count} component types with [Inject] methods:");
            foreach (var kvp in componentsWithInject)
            {
                Debug.Log($"  • {kvp.Key.Name} ({kvp.Value.Count} instances) - Path: {GetGameObjectPath(kvp.Value.First())}");
            }

            Debug.Log($"\n[DI Scanner] === Potential Components to Register ===");

            if (managerTypes.Any())
            {
                Debug.Log($"\n[Managers/Controllers] ({managerTypes.Count}):");
                foreach (var comp in managerTypes.Distinct())
                {
                    Debug.Log($"  • {comp.GetType().Name} - {GetGameObjectPath(comp)}");
                }
            }

            if (gameplayComponents.Any())
            {
                Debug.Log($"\n[Gameplay Components] ({gameplayComponents.Count}):");
                foreach (var comp in gameplayComponents.Distinct())
                {
                    Debug.Log($"  • {comp.GetType().Name} - {GetGameObjectPath(comp)}");
                }
            }

            if (uiComponents.Any())
            {
                Debug.Log($"\n[UI Components] ({uiComponents.Count}):");
                foreach (var comp in uiComponents.Distinct().Take(10)) // Limit UI components shown
                {
                    Debug.Log($"  • {comp.GetType().Name} - {GetGameObjectPath(comp)}");
                }
                if (uiComponents.Count > 10)
                    Debug.Log($"  ... and {uiComponents.Count - 10} more UI components");
            }

            // Check for existing LifetimeScope
            var lifetimeScope = GameObject.FindFirstObjectByType<LifetimeScope>();
            if (lifetimeScope != null)
            {
                Debug.Log($"\n[DI Scanner] ✓ Found LifetimeScope: {lifetimeScope.GetType().Name}");
                Selection.activeObject = lifetimeScope;
            }
            else
            {
                Debug.LogWarning($"\n[DI Scanner] ⚠️ No LifetimeScope found in scene!");
                Debug.Log("[DI Scanner] To fix: Add GameLifetimeScope component to a GameObject in the scene");
            }

            Debug.Log($"\n[DI Scanner] === Scan Complete ===");
        }

        private static string GetGameObjectPath(Component component)
        {
            if (component == null) return "null";

            var path = component.name;
            var parent = component.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }

        [MenuItem("DI/Quick Setup Game Scene", false, 301)]
        public static void QuickSetupGameScene()
        {
            // Check if we're in the Game scene
            var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (!sceneName.ToLower().Contains("game"))
            {
                if (!EditorUtility.DisplayDialog("Not in Game Scene",
                    $"You're in '{sceneName}' scene. This setup is for the Game scene. Continue anyway?",
                    "Yes", "Cancel"))
                {
                    return;
                }
            }

            // Find or create GameLifetimeScope
            var lifetimeScope = GameObject.FindFirstObjectByType<GameLifetimeScope>();
            if (lifetimeScope == null)
            {
                var go = new GameObject("GameLifetimeScope");
                lifetimeScope = go.AddComponent<GameLifetimeScope>();
                Debug.Log("[DI Setup] Created GameLifetimeScope GameObject");
            }

            Selection.activeObject = lifetimeScope.gameObject;
            EditorGUIUtility.PingObject(lifetimeScope);

            Debug.Log("[DI Setup] === Quick Setup Complete ===");
            Debug.Log("[DI Setup] GameLifetimeScope created. You'll need to:");
            Debug.Log("[DI Setup] 1. Add component references to its Configure method");
            Debug.Log("[DI Setup] 2. Run 'DI/Scan Current Scene' to see what needs injection");
        }
    }
}