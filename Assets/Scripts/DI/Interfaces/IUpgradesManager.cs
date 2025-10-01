using OctoberStudio.Upgrades;

namespace OctoberStudio.DI
{
    public interface IUpgradesManager
    {
        bool IsUpgradeAquired(UpgradeType type);
        float GetUpgadeValue(UpgradeType type);
        void IncrementUpgradeLevel(UpgradeType upgradeType);
        int GetUpgradeLevel(UpgradeType upgradeType);
        UpgradeData GetUpgradeData(UpgradeType upgradeType);
    }
}