using OctoberStudio.Audio;
using OctoberStudio.Currency;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OctoberStudio.Abilities
{
    public abstract class AbilityBehavior<T, K> : MonoBehaviour, IAbilityBehavior where T: GenericAbilityData<K> where K : AbilityLevel
    {
        public T Data { get; private set; }
        public AbilityData AbilityData => Data;
        public AbilityType AbilityType => Data.AbilityType;

        public K AbilityLevel { get; private set; }
        public int LevelId { get; private set; }

        // Injected dependencies available to all derived classes
        protected IAudioManager audioManager;
        protected ICameraManager cameraManager;
        protected IEasingManager easingManager;

        [Inject]
        public void Construct(IAudioManager audioManager, ICameraManager cameraManager, IEasingManager easingManager)
        {
            this.audioManager = audioManager;
            this.cameraManager = cameraManager;
            this.easingManager = easingManager;
        }

        public virtual void Init(AbilityData data, int levelId)
        {
            SetData(data as T);
            SetAbilityLevel(levelId);

            Data.onAbilityUpgraded += OnAbilityUpgraded;
        }

        protected virtual void SetData(T data)
        {
            Data = data;
        }

        protected virtual void SetAbilityLevel(int levelId)
        {
            LevelId = levelId;
            AbilityLevel = Data.GetLevel(levelId);
        }

        protected virtual void OnAbilityUpgraded(int levelId)
        {
            SetAbilityLevel(levelId);
        }

        /// <summary>
        /// Helper method to access currency service without breaking IAbilityBehavior interface contract
        /// </summary>
        protected ICurrenciesManager GetCurrenciesManager()
        {
            // Find any LifetimeScope in the current scene
            var lifetimeScope = LifetimeScope.Find<LifetimeScope>();
            if (lifetimeScope != null)
            {
                return lifetimeScope.Container.Resolve<ICurrenciesManager>();
            }
            Debug.LogWarning("[AbilityBehavior] Could not find LifetimeScope to resolve ICurrenciesManager");
            return null;
        }

        private void OnDestroy()
        {
            Data.onAbilityUpgraded -= OnAbilityUpgraded;
        }

        public virtual void Clear()
        {
            Destroy(gameObject);
        }
    }
}