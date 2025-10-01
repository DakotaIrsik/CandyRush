using UnityEngine;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Transitional bridge class to access StageController.FieldManager singleton
    /// during the migration to dependency injection
    /// </summary>
    public class StageFieldManagerBridge : IStageFieldManager
    {
        public bool ValidatePosition(Vector2 position, Vector2 offset, bool withFence = true)
        {
            return StageController.FieldManager.ValidatePosition(position, offset, withFence);
        }

        public Vector2 GetRandomPositionOnBorder()
        {
            return StageController.FieldManager.GetRandomPositionOnBorder();
        }

        public void RemoveFence()
        {
            StageController.FieldManager.RemoveFence();
        }
    }
}