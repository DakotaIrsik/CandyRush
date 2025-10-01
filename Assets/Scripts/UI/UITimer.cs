using OctoberStudio.Easing;
using OctoberStudio.Save;
using System;
using TMPro;
using UnityEngine;
using VContainer;

namespace OctoberStudio.UI
{
    public class UITimer : MonoBehaviour
    {
        [SerializeField] TMP_Text timerText;

        private ISaveManager saveManager;
        IEasingCoroutine alphaCoroutine;
        StageSave stageSave;

        [Inject]
        public void Construct(ISaveManager saveManager)
        {
            this.saveManager = saveManager;
        }

        private void Awake()
        {
            stageSave = saveManager.GetSave<StageSave>("Stage");
        }

        private void Update()
        {
            var timespan = TimeSpan.FromSeconds(StageController.Director.time);

            timerText.text = string.Format("{0:mm\\:ss}", timespan);

            stageSave.Time = (float)StageController.Director.time;
        }

        public void Show()
        {
            alphaCoroutine.StopIfExists();

            gameObject.SetActive(true);
            alphaCoroutine = timerText.DoAlpha(1, 0.3f);
        }

        public void Hide()
        {
            alphaCoroutine.StopIfExists();

            timerText.DoAlpha(0, 0.3f).SetOnFinish(() => gameObject.SetActive(false));
        }
    }
}