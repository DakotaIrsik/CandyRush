using OctoberStudio.Easing;
using OctoberStudio.Input;
using OctoberStudio.Save;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
#if UNITY_WEBGL
using UnityEngine.InputSystem.WebGL;
#endif

namespace OctoberStudio.Vibration
{
    /// <summary>
    /// DEPRECATED: Use VibrationService via dependency injection instead
    /// This MonoBehaviour wrapper is kept for backward compatibility only
    /// </summary>
    [System.Obsolete("Use VibrationService via dependency injection instead")]
    public class VibrationManager : MonoBehaviour, IVibrationManager
    {

        private VibrationSave save;

        private SimpleVibrationHandler vibrationHandler;

        // Injected dependencies
        private ISaveManager saveManager;
        private IInputManager inputManager;
        private IEasingManager easingManager;

        [Inject]
        public void Construct(ISaveManager saveManager, IInputManager inputManager, IEasingManager easingManager)
        {
            this.saveManager = saveManager;
            this.inputManager = inputManager;
            this.easingManager = easingManager;
        }

        public bool IsVibrationEnabled { get => save.IsVibrationEnabled; set => save.IsVibrationEnabled = value; }

        private IEasingCoroutine gamepadVibrationCoroutine;

        private void Awake()
        {
            // Legacy initialization - service pattern doesn't need singleton
            DontDestroyOnLoad(gameObject);
        }

        public void Start()
        {
            // Check if dependencies are available (VContainer injection)
            if (saveManager == null)
            {
                Debug.LogWarning("[VibrationManager] SaveManager dependency not injected - disabling VibrationManager in favor of pure DI services");
                gameObject.SetActive(false);
                return;
            }

            save = saveManager.GetSave<VibrationSave>("Vibration");
            IsVibrationEnabled = true;
#if UNITY_EDITOR
            vibrationHandler = new SimpleVibrationHandler();
#elif UNITY_IOS
            vibrationHandler = new IOSVibrationHandler();
#elif UNITY_ANDROID
            vibrationHandler = new AndroidVibrationHandler();
#elif UNITY_WEBGL
            vibrationHandler = new WebGLVibrationHandler();
#else
            vibrationHandler = new SimpleVibrationHandler();
#endif
        }

        public void Vibrate(float duration, float intensity = 1.0f)
        {
            if (!IsVibrationEnabled) return;

            if (duration <= 0) return;

            if(inputManager.ActiveInput != InputType.Gamepad)
            {
                vibrationHandler.Vibrate(duration, intensity);
            }
            else
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                if (WebGLGamepad.current != null) WebGLGamepad.current.SetMotorSpeeds(intensity, intensity);
#else
                if (Gamepad.current != null) Gamepad.current.SetMotorSpeeds(intensity, intensity);
#endif
                gamepadVibrationCoroutine.StopIfExists();

                gamepadVibrationCoroutine = easingManager.DoAfter(duration, () => 
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                if (WebGLGamepad.current != null) WebGLGamepad.current.SetMotorSpeeds(0, 0);
#else
                    if (Gamepad.current != null) Gamepad.current.SetMotorSpeeds(0, 0);
#endif
                });
            }
        }

        public void LightVibration() => Vibrate(0.08f, 0.4f);
        public void MediumVibration() => Vibrate(0.1f, 0.6f);
        public void StrongVibration() => Vibrate(0.15f, 1f);
    }
}