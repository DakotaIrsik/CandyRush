using OctoberStudio.Abilities;
using OctoberStudio.Audio;
using OctoberStudio.DI;
using OctoberStudio.Easing;

namespace OctoberStudio
{
    public interface IAbilityBehavior
    {
        AbilityType AbilityType { get; }
        AbilityData AbilityData { get; }
        void Construct(IAudioManager audioManager, ICameraManager cameraManager, IEasingManager easingManager);
        void Init(AbilityData data, int stageId);
        void Clear();
    }
}