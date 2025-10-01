using OctoberStudio.Bossfight;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Extensions;
using OctoberStudio.Pool;
using OctoberStudio.Timeline.Bossfight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using VContainer;

namespace OctoberStudio
{
    public class StageFieldManager : MonoBehaviour, IStageFieldManager
    {
        [SerializeField] BossfightDatabase bossfightDatabase;

        private ICameraManager cameraManager;
        private IEasingManager easingManager;

        [Inject]
        public void Construct(ICameraManager cameraManager, IEasingManager easingManager)
        {
            this.cameraManager = cameraManager;
            this.easingManager = easingManager;
        }

        public StageType StageType { get; private set; }
        public GameObject BackgroundPrefab { get; private set; }

        public BossFenceBehavior Fence { get; private set; }

        private IFieldBehavior field;
        private Dictionary<BossType, BossFenceBehavior> fences;

        public void Init(StageData stageData, PlayableDirector director)
        {
            switch (stageData.StageType)
            {
                case StageType.Endless: field = new EndlessFieldBehavior(); break;
                case StageType.VerticalEndless: field = new VerticalFieldBehavior(); break;
                case StageType.HorizontalEndless: field = new HorizontalFieldBehavior(); break;
                case StageType.Rect: field = new RectFieldBehavior(); break;
            }

            // Manually inject dependencies into field behavior
            field.Construct(cameraManager, easingManager);

            field.Init(stageData.StageFieldData, stageData.SpawnProp);

            fences = new Dictionary<BossType, BossFenceBehavior>();

            var bossAssets = director.GetAssets<BossTrack, Boss>();

            for(int i = 0; i < bossAssets.Count; i++)
            {
                var bossAsset = bossAssets[i];
                var bossData = bossfightDatabase.GetBossfight(bossAsset.BossType);

                if (!fences.ContainsKey(bossData.BossType))
                {
                    var fence = Instantiate(bossData.FencePrefab).GetComponent<BossFenceBehavior>();
                    fence.gameObject.SetActive(false);
                    fence.Init();

                    fences.Add(bossData.BossType, fence);
                }
            }
        }

        public Vector2 SpawnFence(BossType bossType, Vector2 offset)
        {
            Fence = fences[bossType];

            var center = field.GetBossSpawnPosition(Fence, offset);

            Fence.SpawnFence(center);

            return center;
        }

        public void RemoveFence()
        {
            Fence.RemoveFence();
            Fence = null;
        }

        public void RemovePropFromFence()
        {
            field.RemovePropFromBossFence(Fence);
        }

        private void Update()
        {
            field.Update();
        }

        public bool ValidatePosition(Vector2 position, Vector2 offset, bool withFence = true)
        {
            var isFenceValid = true;
            if(Fence != null && withFence)
            {
                isFenceValid = Fence.ValidatePosition(position, offset);
            }
            return field.ValidatePosition(position) && isFenceValid;
        }

        public Vector2 GetRandomPositionOnBorder()
        {
            return field.GetRandomPositionOnBorder();
        }

        public bool IsPointOutsideFieldRight(Vector2 point, out float distance)
        {
            return field.IsPointOutsideRight(point, out distance);
        }

        public bool IsPointOutsideFieldLeft(Vector2 point, out float distance)
        {
            return field.IsPointOutsideLeft(point, out distance);
        }

        public bool IsPointOutsideFieldTop(Vector2 point, out float distance)
        {
            return field.IsPointOutsideTop(point, out distance);
        }

        public bool IsPointOutsideFieldBottom(Vector2 point, out float distance)
        {
            return field.IsPointOutsideBottom(point, out distance);
        }
    }
}