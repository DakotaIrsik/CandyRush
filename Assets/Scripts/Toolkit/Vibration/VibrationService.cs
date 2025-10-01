using OctoberStudio.Input;
using OctoberStudio.Save;
using UnityEngine;

namespace OctoberStudio.Vibration
{
    public class VibrationService : IVibrationManager
    {
        private readonly ISaveManager saveManager;
        private readonly IInputManager inputManager;
        private VibrationSave save;

        public bool IsVibrationEnabled
        {
            get => save?.IsVibrationEnabled ?? true;
            set
            {
                if (save != null)
                    save.IsVibrationEnabled = value;
            }
        }

        public VibrationService(ISaveManager saveManager, IInputManager inputManager)
        {
            this.saveManager = saveManager;
            this.inputManager = inputManager;
            Initialize();
        }

        private void Initialize()
        {
            if (saveManager.IsSaveLoaded)
            {
                LoadSaveData();
            }
            else
            {
                saveManager.OnSaveLoaded += LoadSaveData;
            }
        }

        private void LoadSaveData()
        {
            save = saveManager.GetSave<VibrationSave>("Vibration");
            saveManager.OnSaveLoaded -= LoadSaveData;
        }

        public void Vibrate(float duration, float intensity = 1.0f)
        {
            if (!IsVibrationEnabled) return;
            Debug.Log($"[VibrationService] Vibrate: duration={duration}, intensity={intensity}");
            // Platform-specific vibration logic would go here
        }

        public void LightVibration()
        {
            Vibrate(0.1f, 0.3f);
        }

        public void MediumVibration()
        {
            Vibrate(0.2f, 0.6f);
        }

        public void StrongVibration()
        {
            Vibrate(0.3f, 1.0f);
        }
    }
}