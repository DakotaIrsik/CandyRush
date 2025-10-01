using OctoberStudio.Audio;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace OctoberStudio
{
    public class ProjectileBehavior : MonoBehaviour
    {
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
        public List<Effect> Effects { get; private set; }

        public float DamageMultiplier { get; set; }
        public bool KickBack { get; set; }

        public virtual void Init()
        {
            Effects = new List<Effect>();
        }
    }
}