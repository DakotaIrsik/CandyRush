using UnityEngine;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Interface for camera management system
    /// Provides camera positioning, bounds checking, and effects
    /// </summary>
    public interface ICameraManager
    {
        // Properties
        float HalfHeight { get; }
        float HalfWidth { get; }
        Vector2 Position { get; }
        float LeftBound { get; }
        float RightBound { get; }
        float TopBound { get; }
        float BottomBound { get; }
        Camera MainCamera { get; }

        // Target and bounds management
        void SetTarget(Transform target);
        void SetBounds(Bounds bounds);
        void SetSize(float size);

        // Effects
        void Shake(float intensity, float duration);

        // Utility methods
        bool IsPointOutsideCameraRight(Vector2 point);
        bool IsPointOutsideCameraRight(Vector2 point, out float distance);
        bool IsPointOutsideCameraLeft(Vector2 point, out float distance);
        bool IsPointOutsideCameraBottom(Vector2 point, out float distance);
        bool IsPointOutsideCameraTop(Vector2 point, out float distance);
        Vector2 GetPointInsideCamera(float padding = 0);
        Vector2 GetRandomPointOutsideCamera(float padding = 0);
    }
}