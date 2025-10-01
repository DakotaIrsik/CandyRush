using OctoberStudio.DI;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OctoberStudio.Pool
{
    public class PoolsManager : MonoBehaviour, IPoolsManager
    {
        [SerializeField] List<PoolData> preloadedPools;

        private Dictionary<int, PoolObject> pools;

        private void Awake()
        {
            pools = new Dictionary<int, PoolObject>();

            for (int i = 0; i < preloadedPools.Count; i++)
            {
                var data = preloadedPools[i];

                int hash = data.name.GetHashCode();

                var pool = new PoolObject(data.name, data.prefab, data.size);
                pools[hash] = pool;
            }
        }

        public PoolObject GetPool(string name)
        {
            return GetPool(name.GetHashCode());
        }

        public PoolObject GetPool(int hash)
        {
            if (pools.ContainsKey(hash))
            {
                return pools[hash];
            }

            return null;
        }

        public GameObject GetEntity(string name)
        {
            return GetEntity(name.GetHashCode());
        }

        public GameObject GetEntity(int hash)
        {
            var pool = GetPool(hash);

            if (pool != null) return pool.GetEntity();

            return null;
        }

        public T GetEntity<T>(string name) where T : Component
        {
            return GetEntity<T>(name.GetHashCode());
        }

        public T GetEntity<T>(int hash) where T : Component
        {
            var pool = GetPool(hash);

            if (pool != null) return pool.GetEntity<T>();

            return null;
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

        // DI-aware pool methods
        public T GetFromPoolWithInjection<T>(PoolType type) where T : Component
        {
            var entity = GetFromPool<T>(type);
            if (entity != null)
            {
                // Perform dependency injection on the pooled entity
                var lifetimeScope = VContainer.Unity.LifetimeScope.Find<LifetimeScope>();
                if (lifetimeScope != null)
                {
                    lifetimeScope.Container.InjectGameObject(entity.gameObject);
                }
            }
            return entity;
        }

        public GameObject GetEntityWithInjection(string name)
        {
            return GetEntityWithInjection(name.GetHashCode());
        }

        public GameObject GetEntityWithInjection(int hash)
        {
            var entity = GetEntity(hash);
            if (entity != null)
            {
                // Perform dependency injection on the pooled entity
                var lifetimeScope = VContainer.Unity.LifetimeScope.Find<LifetimeScope>();
                if (lifetimeScope != null)
                {
                    lifetimeScope.Container.InjectGameObject(entity);
                }
            }
            return entity;
        }

        [System.Serializable]
        private class PoolData
        {
            public string name;
            public GameObject prefab;
            public int size;
        }
    }
}