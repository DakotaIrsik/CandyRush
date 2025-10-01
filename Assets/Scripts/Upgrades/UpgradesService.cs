using OctoberStudio.DI;
using OctoberStudio.Save;

namespace OctoberStudio.Upgrades
{
    public class UpgradesService : IUpgradesManager
    {
        private readonly ISaveManager saveManager;
        private readonly UpgradesDatabase database;
        private UpgradesSave save;

        public UpgradesService(ISaveManager saveManager, UpgradesDatabase database)
        {
            this.saveManager = saveManager;
            this.database = database;
            // Don't initialize in constructor to avoid Unity API calls on background thread
        }

        private void Initialize()
        {
            if (saveManager != null && saveManager.IsSaveLoaded)
            {
                InitializeSave();
            }
            else if (saveManager != null)
            {
                saveManager.OnSaveLoaded += InitializeSave;
            }
        }

        private void InitializeSave()
        {
            save = saveManager.GetSave<UpgradesSave>("Upgrades Save");
            save.Init();

            if (database != null)
            {
                for (int i = 0; i < database.UpgradesCount; i++)
                {
                    var upgrade = database.GetUpgrade(i);

                    if (save.GetUpgradeLevel(upgrade.UpgradeType) < upgrade.DevStartLevel)
                    {
                        save.SetUpgradeLevel(upgrade.UpgradeType, upgrade.DevStartLevel);
                    }
                }
            }
        }

        private void EnsureInitialized()
        {
            if (save == null)
            {
                Initialize();
            }
        }

        public void IncrementUpgradeLevel(UpgradeType upgradeType)
        {
            EnsureInitialized();
            if (save == null) return;

            var level = save.GetUpgradeLevel(upgradeType);
            save.SetUpgradeLevel(upgradeType, level + 1);
        }

        public int GetUpgradeLevel(UpgradeType upgradeType)
        {
            EnsureInitialized();
            return save?.GetUpgradeLevel(upgradeType) ?? -1;
        }

        public bool IsUpgradeAquired(UpgradeType upgradeType)
        {
            var level = GetUpgradeLevel(upgradeType);
            return level != -1;
        }

        public UpgradeData GetUpgradeData(UpgradeType upgradeType)
        {
            EnsureInitialized();
            return database?.GetUpgrade(upgradeType);
        }

        public float GetUpgadeValue(UpgradeType upgradeType)
        {
            var data = GetUpgradeData(upgradeType);
            var level = GetUpgradeLevel(upgradeType);

            if (level >= 0 && data != null)
            {
                return data.GetLevel(level).Value;
            }

            return 0;
        }
    }
}