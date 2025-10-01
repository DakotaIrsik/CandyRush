using UnityEngine.Events;

namespace OctoberStudio.DI
{
    public interface IExperienceManager
    {
        float XP { get; }
        float TargetXP { get; }
        int Level { get; }
        void AddXP(float xp);
        event UnityAction<int> onXpLevelChanged;
    }
}