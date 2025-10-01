using OctoberStudio.Audio;
using OctoberStudio.Input;
using OctoberStudio.Save;
using OctoberStudio.Vibration;
using UnityEngine;
using VContainer;

namespace OctoberStudio.DI
{
    public class DITest : MonoBehaviour
    {
        // Injected dependencies to test
        [Inject] private IAudioManager audioManager;
        [Inject] private IVibrationManager vibrationManager;
        [Inject] private IInputManager inputManager;
        [Inject] private ISaveManager saveManager;

        private void Start()
        {
            // Test injection by logging if dependencies are properly injected
            Debug.Log($"DI Test Results:");
            Debug.Log($"AudioManager injected: {audioManager != null}");
            Debug.Log($"VibrationManager injected: {vibrationManager != null}");
            Debug.Log($"InputManager injected: {inputManager != null}");
            Debug.Log($"SaveManager injected: {saveManager != null}");

            if (audioManager != null && vibrationManager != null && inputManager != null && saveManager != null)
            {
                Debug.Log("✅ VContainer dependency injection is working correctly!");
            }
            else
            {
                Debug.LogError("❌ VContainer dependency injection failed - some dependencies are null");
            }
        }
    }
}