using OctoberStudio.DI;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace OctoberStudio.Pool
{
    /// <summary>
    /// PoolsService - Pure C# implementation of IPoolsManager
    /// Created and managed entirely by VContainer
    /// Follows the AudioService pattern for clean DI
    /// Now supports automatic dependency injection for pooled objects
    /// </summary>
    public class PoolsService : IPoolsManager
    {
        private readonly Dictionary<int, PoolObject> pools;
        private readonly IObjectResolver container;

        public PoolsService(PoolConfiguration config = null, IObjectResolver container = null)
        {
            pools = new Dictionary<int, PoolObject>();
            this.container = container;

            if (config?.preloadedPools != null)
            {
                InitializePools(config.preloadedPools);
            }
        }

        private void InitializePools(List<PoolConfiguration.PoolData> preloadedPools)
        {
            for (int i = 0; i < preloadedPools.Count; i++)
            {
                var data = preloadedPools[i];
                if (data.prefab == null || string.IsNullOrEmpty(data.name))
                {
                    Debug.LogWarning($"[PoolsService] Invalid pool data at index {i}: name='{data.name}', prefab={data.prefab}");
                    continue;
                }

                int hash = data.name.GetHashCode();

                if (pools.ContainsKey(hash))
                {
                    Debug.LogWarning($"[PoolsService] Pool with name '{data.name}' already exists. Skipping duplicate.");
                    continue;
                }

                var pool = new PoolObject(data.name, data.prefab, data.size);
                pools[hash] = pool;
                Debug.Log($"[PoolsService] Initialized pool '{data.name}' with size {data.size}");
            }
        }

        public PoolObject GetPool(string name)
        {
            return GetPool(name.GetHashCode());
        }

        public PoolObject GetPool(int hash)
        {
            return pools.TryGetValue(hash, out var pool) ? pool : null;
        }

        public GameObject GetEntity(string name)
        {
            return GetEntity(name.GetHashCode());
        }

        public GameObject GetEntity(int hash)
        {
            var pool = GetPool(hash);
            return pool?.GetEntity();
        }

        public T GetEntity<T>(string name) where T : Component
        {
            return GetEntity<T>(name.GetHashCode());
        }

        public T GetEntity<T>(int hash) where T : Component
        {
            var pool = GetPool(hash);
            return pool?.GetEntity<T>();
        }

        // IPoolsManager interface implementation
        public T GetFromPool<T>(PoolType type) where T : Component
        {
            string poolName = GetPoolName(type);
            return GetEntity<T>(poolName);
        }

        public void ReturnToPool<T>(T item) where T : Component
        {
            // In this implementation, entities are automatically returned to pool
            // when their gameObject is disabled. This is typical for Unity object pooling.
            if (item != null && item.gameObject != null)
            {
                item.gameObject.SetActive(false);
            }
        }

        private string GetPoolName(PoolType type)
        {
            // Map PoolType enum to string names used by the existing pool system
            return type switch
            {
                PoolType.Enemy => "Enemy",
                PoolType.Projectile => "Projectile",
                PoolType.Drop => "Drop",
                PoolType.Effect => "Effect",
                PoolType.UI => "UI",
                PoolType.Audio => "Audio",
                PoolType.Custom => "Custom",
                _ => type.ToString()
            };
        }

        // DI-aware pool methods for pure IoC pattern
        public T GetFromPoolWithInjection<T>(PoolType type) where T : Component
        {
            string poolName = GetPoolName(type);
            var entity = GetEntity<T>(poolName);

            if (entity != null && container != null)
            {
                InjectDependencies(entity.gameObject);
            }

            return entity;
        }

        public GameObject GetEntityWithInjection(string name)
        {
            var entity = GetEntity(name);

            if (entity != null && container != null)
            {
                InjectDependencies(entity);
            }

            return entity;
        }

        public GameObject GetEntityWithInjection(int hash)
        {
            var entity = GetEntity(hash);

            if (entity != null && container != null)
            {
                InjectDependencies(entity);
            }

            return entity;
        }

        private void InjectDependencies(GameObject gameObject)
        {
            if (container == null) return;

            // Inject dependencies into all MonoBehaviours on this GameObject
            var components = gameObject.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component != null)
                {
                    container.Inject(component);
                }
            }
        }
    }
}