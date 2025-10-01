using UnityEngine;

namespace OctoberStudio.DI
{
    public interface IEnemiesSpawner
    {
        void StartWave(WaveData waveData);
        void StopSpawning();
        void SpawnEnemy(EnemyType type, Vector2 position);
    }
}