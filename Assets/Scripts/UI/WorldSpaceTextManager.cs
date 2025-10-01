using OctoberStudio.DI;
using OctoberStudio.Pool;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace OctoberStudio.UI
{
    public class WorldSpaceTextManager : MonoBehaviour, IWorldSpaceTextManager
    {
        [SerializeField] RectTransform canvasRect;

        [SerializeField] GameObject textIndicatorPrefab;

        [SerializeField] AnimationCurve scaleCurve;
        [SerializeField] AnimationCurve positionCurve;
        [SerializeField] float maxScale;
        [SerializeField] float maxY;
        [SerializeField] float duration;

        private ICameraManager cameraManager;
        private PoolComponent<TextIndicatorBehavior> indicatorsPool;
        private Queue<IndicatorData> indicators = new Queue<IndicatorData>();

        [Inject]
        public void Construct(ICameraManager cameraManager)
        {
            this.cameraManager = cameraManager;
        }

        private void Start()
        {
            indicatorsPool = new PoolComponent<TextIndicatorBehavior>(textIndicatorPrefab, 500, canvasRect);
        }

        public void SpawnText(Vector2 worldPos, string text)
        {
            var viewportPos = cameraManager.MainCamera.WorldToViewportPoint(worldPos);

            var indicator = indicatorsPool.GetEntity();

            indicator.SetText(text);
            indicator.SetAnchors(viewportPos);
            indicator.SetPosition(Vector2.zero);
            indicators.Enqueue(new IndicatorData() { indicator = indicator, spawnTime = Time.time, startPosition = Vector2.zero, worldPosition = worldPos }); ;
        }

        private void Update()
        {
            while (indicators.Count > 0)
            {
                var data = indicators.Peek();

                if(Time.time > data.spawnTime + duration)
                {
                    indicators.Dequeue();

                    data.indicator.gameObject.SetActive(false);
                } else
                {
                    break;
                }
            }

            foreach(var data in indicators)
            {
                var t = (Time.time - data.spawnTime) / duration;

                data.indicator.SetPosition(Vector2.up * positionCurve.Evaluate(t) * maxY);
                data.indicator.SetScale(Vector3.one * scaleCurve.Evaluate(t) * maxScale);

                data.indicator.SetAnchors(cameraManager.MainCamera.WorldToViewportPoint(data.worldPosition));
            }
        }

        // IWorldSpaceTextManager interface implementation
        public void ShowDamageText(Vector2 position, float damage)
        {
            string damageText = damage.ToString("F0");
            SpawnText(position, damageText);
        }

        public void ShowHealText(Vector2 position, float heal)
        {
            string healText = "+" + heal.ToString("F0");
            SpawnText(position, healText);
        }

        private class IndicatorData
        {
            public TextIndicatorBehavior indicator;
            public float spawnTime;
            public Vector2 startPosition;
            public Vector2 worldPosition;
        }
    }
}