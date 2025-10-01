using OctoberStudio.Audio;
using OctoberStudio.Currency;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Pool;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace OctoberStudio.Drop
{
    public abstract class DropBehavior : MonoBehaviour
    {
        protected IEasingManager easingManager;
        protected IAudioManager audioManager;
        protected ICurrenciesManager currenciesManager;

        [Inject]
        public void Construct(IEasingManager easingManager, IAudioManager audioManager, ICurrenciesManager currenciesManager)
        {
            this.easingManager = easingManager;
            this.audioManager = audioManager;
            this.currenciesManager = currenciesManager;
        }

        [SerializeField] GameObject pickUpParticle;
        [SerializeField] float particleDisableDelay;
        [SerializeField] string pickUpSoundName;
        private int pickUpSoundHash;
         
        private static Dictionary<DropType, PoolComponent<ParticleSystem>> particlePools = new Dictionary<DropType, PoolComponent<ParticleSystem>>();

        private DropData dropData;
        public DropData DropData => dropData;

        public DropType DropType => dropData.DropType;

        private void Awake()
        {
            pickUpSoundHash = pickUpSoundName.GetHashCode();
        }

        public virtual void Init(DropData dropData)
        {
            this.dropData = dropData;

            if (!particlePools.ContainsKey(dropData.DropType) && pickUpParticle != null)
            {
                var particlePool = new PoolComponent<ParticleSystem>(pickUpParticle, 2);

                particlePools[dropData.DropType] = particlePool;
            }
        }

        public virtual void OnPickedUp()
        {
            if(particlePools.TryGetValue(dropData.DropType, out var particlePool))
            {
                var particle = particlePool.GetEntity();
                particle.transform.position = transform.position;
                particle.Play();

                easingManager.DoAfter(particleDisableDelay, () => { particle.gameObject.SetActive(false); });
            }

            audioManager.PlaySound(pickUpSoundHash);
        }


        private void OnDestroy()
        {
            if (particlePools != null){
                foreach(var pool in particlePools.Values)
                {
                    pool.Destroy();
                }

                particlePools.Clear();
            }
        }
    }
}