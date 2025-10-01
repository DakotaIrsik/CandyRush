using UnityEngine;
using VContainer;

namespace OctoberStudio.Input
{
    /// <summary>
    /// MonoBehaviour updater for InputService
    /// Calls InputService.Update() every frame since pure C# services don't have Update loops
    /// </summary>
    public class InputServiceUpdater : MonoBehaviour
    {
        private IInputManager inputManager;

        [Inject]
        public void Construct(IInputManager inputManager)
        {
            this.inputManager = inputManager;
            Debug.Log("[InputServiceUpdater] Constructed with InputManager: " + inputManager?.GetType().Name);
        }

        private void Start()
        {
            Debug.Log("[InputServiceUpdater] Started - InputManager type: " + inputManager?.GetType().Name);
        }

        private void Update()
        {
            if (inputManager is InputService inputService)
            {
                inputService.Update();
            }
        }
    }
}