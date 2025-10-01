using OctoberStudio.Pool;
using UnityEngine;

namespace OctoberStudio.DI
{
    public interface IPoolsManager
    {
        T GetFromPool<T>(PoolType type) where T : Component;
        void ReturnToPool<T>(T item) where T : Component;

        // Add DI-aware pool methods
        T GetFromPoolWithInjection<T>(PoolType type) where T : Component;
        GameObject GetEntityWithInjection(string name);
        GameObject GetEntityWithInjection(int hash);
    }
}