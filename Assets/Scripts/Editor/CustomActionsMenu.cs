using UnityEngine;
using UnityEditor;
using OctoberStudio.Abilities;

namespace OctoberStudio.Save
{
    public static class SaveActionsMenu
    {
        [MenuItem("Tools/October/Delete Save File", priority = 3)]
        private static void DeleteSaveFile()
        {
            PlayerPrefs.DeleteAll();
            SaveManager.DeleteSaveFile();
        }

        [MenuItem("Tools/October/Delete Save File", true)]
        private static bool DeleteSaveFileValidation()
        {
            return !Application.isPlaying;
        }

        [MenuItem("Tools/October/Open All Stages", priority = 2)]
        private static void OpenAllStages()
        {
            // Editor script - access through Unity's Object.FindObjectOfType for development tools
            var saveManager = Object.FindObjectOfType<SaveManager>();
            var stageSave = saveManager.GetSave<StageSave>("Stage");

            string[] guiID = AssetDatabase.FindAssets("t:StagesDatabase");

            if (guiID != null)
            {
                var database = AssetDatabase.LoadAssetAtPath<StagesDatabase>(AssetDatabase.GUIDToAssetPath(guiID[0]));

                if(database != null)
                {
                    stageSave.SetMaxReachedStageId(database.StagesCount - 1);

                    EditorApplication.isPlaying = false;
                }
            }
        }

        [MenuItem("Tools/October/Open All Stages", true)]
        private static bool OpenAllStagesValidation()
        {
            return Application.isPlaying;
        }

        [MenuItem("Tools/October/Get 1K Gold", priority = 1)]
        private static void GetGold()
        {
            // Editor script - access through Unity's Object.FindObjectOfType for development tools
            var saveManager = Object.FindObjectOfType<SaveManager>();
            var gold = saveManager.GetSave<CurrencySave>("gold");

            gold.Deposit(1000);
        }

        [MenuItem("Tools/October/Get 1K Gold", true)]
        private static bool GetGoldValidation()
        {
            return Application.isPlaying;
        }
    }
}