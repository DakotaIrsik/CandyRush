using UnityEngine;

namespace OctoberStudio.DI
{
    public interface IDropManager
    {
        void SpawnDrop(DropType type, Vector2 position);
        void SpawnRandomDrop(Vector2 position);
        bool CheckDropCooldown(DropType dropType);
        void Drop(DropType dropType, Vector3 position);
    }
}