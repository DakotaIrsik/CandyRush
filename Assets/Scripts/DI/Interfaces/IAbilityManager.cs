using OctoberStudio.Abilities;
using System.Collections.Generic;

namespace OctoberStudio.DI
{
    public interface IAbilityManager
    {
        int ActiveAbilitiesCapacity { get; }
        int PassiveAbilitiesCapacity { get; }
        void AddAbility(AbilityData abilityData, int level = 0);
        int GetActiveAbilitiesCount();
        int GetPassiveAbilitiesCount();
        int GetAbilityLevel(AbilityType abilityType);
        IAbilityBehavior GetAquiredAbility(AbilityType abilityType);
        bool IsAbilityAquired(AbilityType ability);
        bool HasAvailableAbilities();
        AbilityData GetAbilityData(AbilityType abilityType);
        List<AbilityType> GetAquiredAbilityTypes();
        void ShowChest();
    }
}