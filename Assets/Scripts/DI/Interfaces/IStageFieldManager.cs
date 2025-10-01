using UnityEngine;

namespace OctoberStudio.DI
{
    public interface IStageFieldManager
    {
        bool ValidatePosition(Vector2 position, Vector2 offset, bool withFence = true);
        Vector2 GetRandomPositionOnBorder();
        void RemoveFence();
    }
}