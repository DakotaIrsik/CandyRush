using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

namespace OctoberStudio.Save
{
    public enum SaveType
    {
        SaveFile,
        PlayerPrefs,
    }
    public class SaveService : ISaveManager
    {
        public static readonly string SAVE_FILE_NAME = "game_save";

        private readonly SaveType saveType;
        private readonly bool clearSave;
        private readonly bool autoSaveEnabled;
        private readonly float autoSaveDelay;

        private SaveDatabase SaveDatabase { get; set; }

        public bool IsSaveLoaded { get; private set; }
        public event UnityAction OnSaveLoaded;

        public SaveService(SaveType saveType = SaveType.SaveFile, bool clearSave = false,
                          bool autoSaveEnabled = true, float autoSaveDelay = 10f)
        {
            this.saveType = saveType;
            this.clearSave = clearSave;
            this.autoSaveEnabled = autoSaveEnabled;
            this.autoSaveDelay = autoSaveDelay;

            // Don't initialize in constructor to avoid Unity API calls on background thread
        }

        private void Initialize()
        {
            if (clearSave)
            {
                InitClear();
            }
            else
            {
                Load();
            }

            // Note: Auto-save would need to be handled by a MonoBehaviour bootstrapper
        }

        public T GetSave<T>(int hash) where T : ISave, new()
        {
            // Initialize on first access if not already done
            if (!IsSaveLoaded)
            {
                Initialize();
            }

            if (!IsSaveLoaded)
            {
                Debug.LogError("Save file has not been loaded yet");
                return default;
            }

            return SaveDatabase.GetSave<T>(hash);
        }

        public T GetSave<T>(string uniqueName) where T : ISave, new()
        {
            return GetSave<T>(uniqueName.GetHashCode());
        }

        public void Save(bool multithreading = false)
        {
            if (!IsSaveLoaded)
            {
                Debug.LogError("Cannot save - save file not loaded");
                return;
            }

            if (saveType == SaveType.SaveFile)
            {
                SaveToFile();
            }
            else if (saveType == SaveType.PlayerPrefs)
            {
                SaveToPlayerPrefs();
            }
        }

        private void InitClear()
        {
            SaveDatabase = new SaveDatabase();
            SaveDatabase.Init();

            Debug.Log("New save is created");
            IsSaveLoaded = true;
            OnSaveLoaded?.Invoke();
        }

        private void Load()
        {
            if (IsSaveLoaded)
                return;

            if (saveType == SaveType.SaveFile)
            {
                SaveDatabase = LoadSaveFromFile();
                SaveDatabase.Init();
                // Debug.Log("Save file is loaded");
            }
            else if (saveType == SaveType.PlayerPrefs)
            {
                SaveDatabase = LoadSaveFromPlayerPrefs();
                SaveDatabase.Init();
                // Debug.Log("Save from PlayerPrefs is loaded");
            }

            IsSaveLoaded = true;
            OnSaveLoaded?.Invoke();
        }

        private SaveDatabase LoadSaveFromFile()
        {
            var path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME + ".json");

            if (File.Exists(path))
            {
                var jsonData = File.ReadAllText(path);
                return JsonUtility.FromJson<SaveDatabase>(jsonData) ?? new SaveDatabase();
            }

            return new SaveDatabase();
        }

        private SaveDatabase LoadSaveFromPlayerPrefs()
        {
            var jsonData = PlayerPrefs.GetString(SAVE_FILE_NAME, "{}");
            return JsonUtility.FromJson<SaveDatabase>(jsonData) ?? new SaveDatabase();
        }

        private void SaveToFile()
        {
            var path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME + ".json");
            var jsonData = JsonUtility.ToJson(SaveDatabase, true);
            File.WriteAllText(path, jsonData);
        }

        private void SaveToPlayerPrefs()
        {
            var jsonData = JsonUtility.ToJson(SaveDatabase, true);
            PlayerPrefs.SetString(SAVE_FILE_NAME, jsonData);
            PlayerPrefs.Save();
        }
    }
}