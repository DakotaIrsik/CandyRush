using UnityEngine;

namespace OctoberStudio.DI
{
    [System.Serializable]
    public class WaveData
    {
        [SerializeField] public EnemyType enemyType;
        [SerializeField] public int enemyCount;
        [SerializeField] public float spawnInterval;
        [SerializeField] public bool circularSpawn;
        [SerializeField] public Vector2 spawnPosition;

        public WaveData(EnemyType type, int count, float interval = 1f, bool circular = false)
        {
            enemyType = type;
            enemyCount = count;
            spawnInterval = interval;
            circularSpawn = circular;
            spawnPosition = Vector2.zero;
        }
    }
}