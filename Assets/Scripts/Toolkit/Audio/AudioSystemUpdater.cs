using UnityEngine;
using VContainer;

namespace OctoberStudio.Audio
{
    /// <summary>
    /// Minimal MonoBehaviour to handle Unity Update loop for AudioService
    /// Created automatically by DI container
    /// </summary>
    public class AudioSystemUpdater : MonoBehaviour
    {
        private IAudioManager audioManager;

        [Inject]
        public void Construct(IAudioManager audioManager)
        {
            this.audioManager = audioManager;
        }

        private void Update()
        {
            if (audioManager is AudioService service)
            {
                service.Update();
            }
        }
    }
}