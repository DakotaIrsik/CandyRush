using OctoberStudio.DI;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace OctoberStudio.UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public class ScalerHelper : MonoBehaviour
    {
        private ICameraManager cameraManager;

        [Inject]
        public void Construct(ICameraManager cameraManager)
        {
            this.cameraManager = cameraManager;
        }

        private void Awake()
        {
            GetComponent<CanvasScaler>().matchWidthOrHeight = IsWideScreen() ? 1 : 0;
        }

        public bool IsWideScreen()
        {
#if UNITY_IOS
            return UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
#else
            var camera = cameraManager?.MainCamera ?? Camera.main;
            return camera.aspect > (9f / 18f);
#endif
        }

        public static bool IsWideScreenStatic()
        {
#if UNITY_IOS
            return UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
#else
            return Camera.main.aspect > (9f / 18f);
#endif
        }
    }

}
