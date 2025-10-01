using OctoberStudio.DI;
using UnityEngine;
using VContainer;

namespace OctoberStudio
{
    /// <summary>
    /// CameraManager - MonoBehaviour implementation of ICameraManager
    /// Handles camera following, bounds, and effects
    /// Can be injected via dependency injection
    /// </summary>
    public class CameraManager : MonoBehaviour, ICameraManager
    {

        [SerializeField] Transform target;
        [SerializeField] SpriteRenderer spotlightRenderer;
        [SerializeField] SpriteRenderer spotlightShadowRenderer;

        private Vector3 offset;
        private Camera mainCamera;

        // ICameraManager interface implementation
        public float HalfHeight => mainCamera.orthographicSize;
        public float HalfWidth => mainCamera.orthographicSize * mainCamera.aspect;
        public Vector2 Position => transform.position;
        public float LeftBound => Position.x - HalfWidth;
        public float RightBound => Position.x + HalfWidth;
        public float TopBound => Position.y + HalfHeight;
        public float BottomBound => Position.y - HalfHeight;
        public Camera MainCamera => mainCamera;



        private void Awake()
        {
            mainCamera = GetComponent<Camera>();

            offset = transform.position - target.position;
            spotlightShadowRenderer.size = new Vector2(HalfWidth, HalfHeight) * 2;
        }

        public void Init(StageData stageData)
        {
            spotlightRenderer.color = stageData.SpotlightColor;
            spotlightShadowRenderer.color = stageData.SpotlightShadowColor;
        }

        private void LateUpdate()
        {
            transform.position = target.position + offset;
        }

        public void SetSize(float size)
        {
            mainCamera.orthographicSize = size;
            spotlightShadowRenderer.size = new Vector2(HalfWidth, HalfHeight) * 2;
        }

        // ICameraManager interface implementation
        public bool IsPointOutsideCameraRight(Vector2 point)
        {
            return point.x > RightBound;
        }

        public bool IsPointOutsideCameraRight(Vector2 point, out float distance)
        {
            bool result = point.x > RightBound;
            distance = result ? point.x - RightBound : 0;
            return result;
        }

        public bool IsPointOutsideCameraLeft(Vector2 point, out float distance)
        {
            bool result = point.x < LeftBound;
            distance = result ? LeftBound - point.x : 0;
            return result;
        }

        public bool IsPointOutsideCameraBottom(Vector2 point, out float distance)
        {
            bool result = point.y < BottomBound;
            distance = result ? BottomBound - point.y : 0;
            return result;
        }

        public bool IsPointOutsideCameraTop(Vector2 point, out float distance)
        {
            bool result = point.y > TopBound;
            distance = result ? point.y - TopBound : 0;
            return result;
        }

        public Vector2 GetPointInsideCamera(float padding = 0)
        {
            return new Vector2(Random.Range(LeftBound + padding, RightBound - padding), Random.Range(BottomBound + padding, TopBound - padding));
        }

        public Vector2 GetRandomPointOutsideCamera(float padding = 0)
        {
            if(Random.value > mainCamera.aspect / (mainCamera.aspect + 1))
            {
                float x = Random.value > 0.5f ? LeftBound - padding : RightBound + padding;
                return new Vector2(x, Random.Range(BottomBound - padding, TopBound + padding));
            } else
            {
                float y = Random.value > 0.5f ? TopBound + padding : BottomBound - padding;
                return new Vector2(Random.Range(LeftBound - padding, RightBound + padding), y);
            }
        }


        // ICameraManager interface implementation
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            offset = transform.position - target.position;
        }

        public void SetBounds(Bounds bounds)
        {
            // Update camera bounds - this could be used to clamp camera movement
            // Implementation depends on specific requirements
            // For now, we'll just update the orthographic size to fit the bounds
            float boundsAspect = bounds.size.x / bounds.size.y;
            float cameraAspect = mainCamera.aspect;

            if (boundsAspect > cameraAspect)
            {
                mainCamera.orthographicSize = bounds.size.y / 2f;
            }
            else
            {
                mainCamera.orthographicSize = bounds.size.x / (2f * cameraAspect);
            }

            spotlightShadowRenderer.size = new Vector2(HalfWidth, HalfHeight) * 2;
        }

        public void Shake(float intensity, float duration)
        {
            // Basic camera shake implementation
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ShakeCoroutine(intensity, duration));
            }
        }

        private System.Collections.IEnumerator ShakeCoroutine(float intensity, float duration)
        {
            Vector3 originalOffset = offset;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                float currentIntensity = intensity * (1f - progress);

                Vector3 randomOffset = Random.insideUnitCircle * currentIntensity;
                offset = originalOffset + randomOffset;

                elapsed += Time.deltaTime;
                yield return null;
            }

            offset = originalOffset;
        }
    }
}