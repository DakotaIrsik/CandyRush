using System.Collections.Generic;
using UnityEngine;

namespace OctoberStudio.DI
{
    [CreateAssetMenu(fileName = "PoolConfiguration", menuName = "October Studio/Configuration/Pool Configuration")]
    public class PoolConfiguration : ScriptableObject
    {
        [System.Serializable]
        public class PoolData
        {
            public string name;
            public GameObject prefab;
            public int size = 10;
        }

        [SerializeField] public List<PoolData> preloadedPools = new List<PoolData>();
    }
}