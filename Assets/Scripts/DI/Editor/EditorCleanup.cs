using UnityEngine;
using UnityEditor;

namespace OctoberStudio.DI.Editor
{
    [InitializeOnLoad]
    public static class EditorCleanup
    {
        static EditorCleanup()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            // Clear inspector focus when hierarchy changes to prevent null reference errors
            if (Selection.activeGameObject != null && Selection.activeGameObject.activeInHierarchy == false)
            {
                Selection.activeObject = null;
            }
        }

        [MenuItem("DI/Clear Inspector Selection", false, 100)]
        public static void ClearInspectorSelection()
        {
            Selection.activeObject = null;
            EditorUtility.ClearProgressBar();
            System.GC.Collect();
            Debug.Log("[DI] Inspector selection cleared to fix null reference errors.");
        }

        [MenuItem("DI/Refresh Editor State", false, 101)]
        public static void RefreshEditorState()
        {
            Selection.activeObject = null;
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.RepaintProjectWindow();
            System.GC.Collect();
            Debug.Log("[DI] Editor state refreshed.");
        }
    }
}