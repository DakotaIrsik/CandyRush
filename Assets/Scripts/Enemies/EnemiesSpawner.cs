using OctoberStudio.Bossfight;
using OctoberStudio.DI;
using OctoberStudio.Extensions;
using OctoberStudio.Pool;
using OctoberStudio.Save;
using OctoberStudio.Timeline;
using OctoberStudio.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using VContainer;

namespace OctoberStudio
{
    [DefaultExecutionOrder(-10)]
    public class EnemiesSpawner : MonoBehaviour, IEnemiesSpawner
    {
        private List<EnemyBehavior> enemies = new List<EnemyBehavior>();

        [SerializeField] EnemiesDatabase database;
        [SerializeField] BossfightDatabase bossfightDatabase;

        [Space]
        [SerializeField] ScalingLabelBehavior enemiesDiedLabel;

        [Space]
        [SerializeField] int enemiesCap = 500;

        [Header("Offscreen Teleport")]
        [SerializeField] bool isOffscreenTeleportEnabled = true;
        [SerializeField] float diagonalDistanceMultiplier = 1.3f;
        [SerializeField, Range(0, 1f)] float teleportConeSize = 0.8f;

        private int enemiesDiedCounter;

        private Dictionary<EnemyType, PoolComponent<EnemyBehavior>> enemyPools;

        StageSave stageSave;

        public bool IsBossfightActive { get; set; }

        // Injected dependencies
        private ISaveManager saveManager;
        private IStageFieldManager stageFieldManager;
        private IDropManager dropManager;
        private IAbilityManager abilityManager;
        private ICameraManager cameraManager;
        private VContainer.IObjectResolver container;

        [Inject]
        public void Construct(ISaveManager saveManager, IStageFieldManager stageFieldManager,
                             IDropManager dropManager, IAbilityManager abilityManager, ICameraManager cameraManager, VContainer.IObjectResolver container)
        {
            this.saveManager = saveManager;
            this.stageFieldManager = stageFieldManager;
            this.dropManager = dropManager;
            this.abilityManager = abilityManager;
            this.cameraManager = cameraManager;
            this.container = container;
        }

        // Were creating pools only for the enemies that are present in the Stage Timeline
        public void Init(PlayableDirector director)
        {
            stageSave = saveManager.GetSave<StageSave>("Stage");

            Dictionary<EnemyType, int> enemiesOnLevel = new Dictionary<EnemyType, int>();

            var waves = director.GetAssets<WaveTrack, WaveAsset>();

            for(int i = 0; i < waves.Count; i++)
            {
                var wave = waves[i];
                var enemyType = wave.EnemyType;
                var enemiesCount = wave.EnemiesCount;

                if (enemiesOnLevel.ContainsKey(enemyType))
                {
                    if (enemiesOnLevel[enemyType] < enemiesCount)
                    {
                        enemiesOnLevel[enemyType] = enemiesCount;
                    }
                }
                else
                {
                    enemiesOnLevel.Add(enemyType, enemiesCount);
                }
            }

            var trackEnemies = new List<EnemyType>();
            foreach (var output in director.playableAsset.outputs)
            {
                if(output.sourceObject is WaveTrack waveTrack)
                {
                    if (!trackEnemies.Contains(waveTrack.EnemyType))
                    {
                        trackEnemies.Add(waveTrack.EnemyType);
                    }
                }
            }

            enemyPools = new Dictionary<EnemyType, PoolComponent<EnemyBehavior>>();

            foreach(var enemyType in enemiesOnLevel.Keys)
            {
                var data = database.GetEnemyData(enemyType);

                var amount = enemiesOnLevel[enemyType];
                if (amount > 100) amount = 100;
                if (amount < 0) amount = 1;

                var pool = new PoolComponent<EnemyBehavior>($"Enemy {enemyType}", data.Prefab, amount);

                enemyPools.Add(data.Type, pool);
            }

            foreach(var enemyType in trackEnemies)
            {
                if (!enemyPools.ContainsKey(enemyType))
                {
                    var data = database.GetEnemyData(enemyType);
                    var pool = new PoolComponent<EnemyBehavior>($"Enemy {enemyType}", data.Prefab, 1);

                    enemyPools.Add(data.Type, pool);
                }
            }

            enemiesDiedCounter = 0;
            if (!stageSave.ResetStageData)
            {
                enemiesDiedCounter = stageSave.EnemiesKilled;
            }
            
            enemiesDiedLabel.SetAmount(enemiesDiedCounter);
        }

        private void Update()
        {
            if (!isOffscreenTeleportEnabled || IsBossfightActive) return;

            var diagonalSqr = (cameraManager.HalfWidth * cameraManager.HalfWidth + cameraManager.HalfHeight * cameraManager.HalfHeight) * diagonalDistanceMultiplier;
            var diagonal = Mathf.Sqrt(diagonalSqr);

            var dotValue = teleportConeSize - 1;
            var modValue = Mathf.Clamp(enemies.Count / 20, 1, 100);
            int frame = Time.frameCount % modValue;
            for(int i = frame; i < enemies.Count; i += modValue)
            {
                var enemy = enemies[i];

                if (enemy.WaveOverride != null && enemy.WaveOverride.DisableOffscreenTeleport) continue;

                var enemyToPlayer = enemy.transform.position - PlayerBehavior.Player.transform.position;
                var direction = enemyToPlayer.normalized;
                var dot = Vector2.Dot(direction, PlayerBehavior.Player.LookDirection);

                if(diagonalSqr < enemyToPlayer.sqrMagnitude && dot < dotValue)
                {
                    var teleportPosition = PlayerBehavior.Player.transform.position + Quaternion.Euler(0, 0, Random.Range(-45, 45)) * PlayerBehavior.Player.LookDirection * diagonal;
                    enemy.transform.position = teleportPosition;
                }
            }
        }

        public EnemyBehavior GetClosestEnemy(Vector2 point)
        {
            EnemyBehavior closestEnemy = null;
            float closestDistance = float.MaxValue;

            for(int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null)
                {
                    enemies.RemoveAt(i);
                    i--;
                    continue;
                }

                float distance = (point - enemies[i].transform.position.XY()).sqrMagnitude;

                if(distance < closestDistance)
                {
                    closestEnemy = enemies[i];
                    closestDistance = distance;
                }
            }

            return closestEnemy;
        }

        public EnemyBehavior Spawn(EnemyType enemyType, Vector2 position, UnityAction<EnemyBehavior> onEnemyDiedCallback = null)
        {
            if (enemies.Count >= enemiesCap) return null;

            if(!enemyPools.ContainsKey(enemyType))
            {
                var data = database.GetEnemyData(enemyType);

                var pool = new PoolComponent<EnemyBehavior>($"Enemy {enemyType}", data.Prefab, 10);

                enemyPools.Add(data.Type, pool);
            }

            var enemy = enemyPools[enemyType].GetEntity();

            // Inject dependencies into the pooled enemy
            container.Inject(enemy);

            enemy.SetData(database.GetEnemyData(enemyType));

            enemy.transform.position = position;

            enemy.onEnemyDied += OnEnemyDied;
            if (onEnemyDiedCallback != null) enemy.onEnemyDied += onEnemyDiedCallback;

            enemy.Play();

            enemies.Add(enemy);

            return enemy;
        }

        public void Spawn(EnemyType type, WaveOverride waveOverride, bool circularSpawn = false, int amount = 1, UnityAction<EnemyBehavior> onEnemyDiedCallback = null)
        {
            for(int i = 0; i < amount; i++)
            {
                if (enemies.Count >= enemiesCap) return;

                var enemy = enemyPools[type].GetEntity();

                // Inject dependencies into the pooled enemy
                container.Inject(enemy);

                enemy.SetData(database.GetEnemyData(type));
                enemy.SetWaveOverride(waveOverride);

                var triesCount = 0;
                var maxTriesCount = 10;

                Vector3 position = Vector3.zero;
                bool foundPosition = false;

                while(triesCount < maxTriesCount)
                {
                    triesCount++;

                    if (circularSpawn)
                    {
                        float height = cameraManager.HalfHeight;
                        float width = cameraManager.HalfWidth;
                        float diagonal = Mathf.Sqrt(width * width + height * height);
                        position = PlayerBehavior.Player.transform.position + Random.onUnitSphere.SetZ(0).normalized * diagonal * 1.05f;
                    } else
                    {
                        position = cameraManager.GetRandomPointOutsideCamera(0.5f);
                    }
                    
                    if(stageFieldManager.ValidatePosition(position, Vector2.zero))
                    {
                        foundPosition = true;
                        break;
                    }
                }

                if (!foundPosition)
                {
                    for (int j = 1; j < 10; j++)
                    {
                        var middlePosition = Vector3.Lerp(position, PlayerBehavior.Player.transform.position, 1 - j / 10f);

                        if (stageFieldManager.ValidatePosition(middlePosition, Vector2.zero))
                        {
                            foundPosition = true;
                            position = middlePosition;
                            break;
                        }
                    }
                }

                if (!foundPosition)
                {
                    position = stageFieldManager.GetRandomPositionOnBorder();
                }

                enemy.transform.position = position;

                enemy.onEnemyDied += OnEnemyDied;
                if (onEnemyDiedCallback != null) enemy.onEnemyDied += onEnemyDiedCallback;

                enemy.Play();

                enemies.Add(enemy);
            }
        }

        public EnemyBehavior GetRandomVisibleEnemy()
        {
            if (enemies.Count == 0) return null;

            // Trying to find random visible enemy 10 times
            for(int i = 0; i < 10; i++)
            {
                var randomIndex = Random.Range(0, enemies.Count);

                var enemy = enemies[randomIndex];

                if (enemy.IsVisible) return enemy;
            }

            for(int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];

                if (enemy.IsVisible) return enemy;
            }

            return null;
        }

        public List<EnemyBehavior> GetEnemiesInRadius(Vector2 position, float radius)
        {
            var result = new List<EnemyBehavior>();

            float radiusSqr = radius * radius;

            for(int i = 0; i < enemies.Count; i++)
            {
                if ((enemies[i].transform.position.XY() - position).sqrMagnitude <= radiusSqr)
                {
                    result.Add(enemies[i]);
                }
            }

            return result;
        }

        public void KillEveryEnemy()
        {
            foreach(var enemy in enemies)
            {
                enemy.onEnemyDied -= OnEnemyDied;
                enemy.Kill();
            }

            enemiesDiedCounter += enemies.Count;
            stageSave.EnemiesKilled = enemiesDiedCounter;

            enemiesDiedLabel.SetAmount(enemiesDiedCounter);

            enemies.Clear();
        }

        public void DealDamageToAllEnemies(float damage)
        {
            var aliveEnemies = new List<EnemyBehavior>();

            foreach(var enemy in enemies)
            {
                if(enemy.HP <= damage)
                {
                    // if enemy is not a boss
                    if(enemy.Data != null)
                    {
                        enemy.onEnemyDied -= OnEnemyDied;
                        enemy.Kill();

                        foreach (var dropData in enemy.GetDropData())
                        {
                            if (dropData.Chance == 0) continue;

                            if (Random.value * 100 <= dropData.Chance && dropManager.CheckDropCooldown(dropData.DropType))
                            {
                                dropManager.Drop(dropData.DropType, enemy.transform.position.XY() + Random.insideUnitCircle * 0.2f);
                            }
                        }
                    } else
                    {
                        aliveEnemies.Add(enemy);
                    }
                }
                else
                {
                    // if enemy is not a boss
                    if (enemy.Data != null)
                    {
                        enemy.TakeDamage(damage);
                    }
                    aliveEnemies.Add(enemy);
                }
            }

            enemiesDiedCounter += enemies.Count - aliveEnemies.Count;

            stageSave.EnemiesKilled = enemiesDiedCounter;
            enemiesDiedLabel.SetAmount(enemiesDiedCounter);

            enemies.Clear();
            enemies.AddRange(aliveEnemies);
        }

        private void OnEnemyDied(EnemyBehavior enemy)
        {
            enemies.Remove(enemy);
            enemy.onEnemyDied -= OnEnemyDied;

            foreach(var dropData in enemy.GetDropData())
            {
                if(dropData.Chance == 0) continue;
                if(Random.value * 100 <= dropData.Chance && dropManager.CheckDropCooldown(dropData.DropType))
                {
                    dropManager.Drop(dropData.DropType, enemy.transform.position.XY() + Random.insideUnitCircle * 0.2f);
                }
            }

            enemiesDiedCounter++;
            stageSave.EnemiesKilled = enemiesDiedCounter;
            enemiesDiedLabel.SetAmount(enemiesDiedCounter);
        }

        private void OnBossDied(EnemyBehavior boss)
        {
            enemies.Remove(boss);
            boss.onEnemyDied -= OnBossDied;

            if (boss.ShouldSpawnChestOnDeath && abilityManager.HasAvailableAbilities()) dropManager.Drop(DropType.Chest, boss.transform.position.XY() + Random.insideUnitCircle);
            dropManager.Drop(DropType.Magnet, boss.transform.position.XY() + Random.insideUnitCircle);
            dropManager.Drop(DropType.Food, boss.transform.position.XY() + Random.insideUnitCircle);

            enemiesDiedCounter++;
            stageSave.EnemiesKilled = enemiesDiedCounter;
            enemiesDiedLabel.SetAmount(enemiesDiedCounter);
        }

        public EnemyBehavior SpawnBoss(BossType bossType, Vector2 spawnPosition, UnityAction<EnemyBehavior> onBossDied = null)
        {
            var bossData = bossfightDatabase.GetBossfight(bossType);

            var boss = Instantiate(bossData.BossPrefab).GetComponent<EnemyBehavior>();
            boss.transform.position = spawnPosition;

            boss.Play();

            boss.onEnemyDied += OnBossDied;
            boss.onEnemyDied += onBossDied;

            enemies.Add(boss);

            return boss;
        }

        public BossfightData GetBossData(BossType bossType)
        {
            return bossfightDatabase.GetBossfight(bossType);
        }

        // IEnemiesSpawner interface implementation
        private Coroutine currentWaveCoroutine;

        public void StartWave(WaveData waveData)
        {
            if (currentWaveCoroutine != null)
            {
                StopCoroutine(currentWaveCoroutine);
            }
            currentWaveCoroutine = StartCoroutine(SpawnWaveCoroutine(waveData));
        }

        public void StopSpawning()
        {
            if (currentWaveCoroutine != null)
            {
                StopCoroutine(currentWaveCoroutine);
                currentWaveCoroutine = null;
            }
        }

        public void SpawnEnemy(EnemyType type, Vector2 position)
        {
            Spawn(type, position);
        }

        private IEnumerator SpawnWaveCoroutine(WaveData waveData)
        {
            int spawned = 0;
            while (spawned < waveData.enemyCount)
            {
                Vector2 spawnPos = waveData.spawnPosition;
                if (spawnPos == Vector2.zero)
                {
                    // Use default spawn logic if no position specified
                    spawnPos = cameraManager.GetRandomPointOutsideCamera(2f);
                }

                SpawnEnemy(waveData.enemyType, spawnPos);
                spawned++;

                if (spawned < waveData.enemyCount)
                {
                    yield return new WaitForSeconds(waveData.spawnInterval);
                }
            }
            currentWaveCoroutine = null;
        }
    }
}