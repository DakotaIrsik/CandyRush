using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace OctoberStudio
{
    public abstract class AbstractFieldBehavior : IFieldBehavior
    {
        // Injected dependencies available to all derived classes
        protected ICameraManager cameraManager;
        protected IEasingManager easingManager;

        [Inject]
        public void Construct(ICameraManager cameraManager, IEasingManager easingManager)
        {
            this.cameraManager = cameraManager;
            this.easingManager = easingManager;
        }
        public StageFieldData Data { get; private set; }

        private bool spawnProp;

        public abstract void Clear();
        public abstract Vector2 GetBossSpawnPosition(BossFenceBehavior fence, Vector2 offset);
        public abstract Vector2 GetRandomPositionOnBorder();
        public abstract bool IsPointOutsideBottom(Vector2 point, out float distance);
        public abstract bool IsPointOutsideLeft(Vector2 point, out float distance);
        public abstract bool IsPointOutsideRight(Vector2 point, out float distance);
        public abstract bool IsPointOutsideTop(Vector2 point, out float distance);
        public abstract void Update();
        public abstract bool ValidatePosition(Vector2 position);
        public abstract void RemovePropFromBossFence(BossFenceBehavior fence);

        private List<PoolComponent<PropBehavior>> propPools = new List<PoolComponent<PropBehavior>>();

        public virtual void Init(StageFieldData stageFieldData, bool spawnProp)
        {
            Data = stageFieldData;

            this.spawnProp = spawnProp;

            if (!spawnProp) return;

            for (int i = 0; i < stageFieldData.PropChances.Count; i++)
            {
                var propData = stageFieldData.PropChances[i];
                var pool = new PoolComponent<PropBehavior>(propData.Prefab, propData.MaxAmount * 9);

                propPools.Add(pool);
            }
        }

        protected void SpawnProp(StageChunkBehavior chunk)
        {
            if (!spawnProp) return;

            for (int i = 0; i < Data.PropChances.Count; i++)
            {
                var propData = Data.PropChances[i];

                if(Random.value * 100 < propData.Chance)
                {
                    int amount = Mathf.RoundToInt(Mathf.Lerp(1, propData.MaxAmount, Random.value));

                    var pool = propPools[i];
                    for (int j = 0; j < amount; j++)
                    {
                        var prop = pool.GetEntity();

                        chunk.AddProp(prop);
                    }
                }
            }
        }
    }
}