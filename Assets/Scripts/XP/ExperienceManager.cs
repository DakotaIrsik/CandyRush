using OctoberStudio.Audio;
using OctoberStudio.DI;
using OctoberStudio.Easing;
using OctoberStudio.Save;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace OctoberStudio
{
    public class ExperienceManager : MonoBehaviour, IExperienceManager
    {
        [SerializeField] ExperienceData experienceData;
        [SerializeField] ExperienceUI experienceUI;

        private static readonly int LEVEL_UP_HASH = "Level Up".GetHashCode();

        public float XP { get; private set; }
        public float TargetXP { get; private set; }
        public int Level { get; private set; }

        public event UnityAction<int> onXpLevelChanged;

        StageSave stageSave;

        // Injected dependencies
        private ISaveManager saveManager;
        private IAudioManager audioManager;
        private IEasingManager easingManager;

        [Inject]
        public void Construct(ISaveManager saveManager, IAudioManager audioManager, IEasingManager easingManager)
        {
            this.saveManager = saveManager;
            this.audioManager = audioManager;
            this.easingManager = easingManager;
        }

        public void Init(PresetData testingPreset)
        {
            stageSave = saveManager.GetSave<StageSave>("Stage");

            XP = 0;
            Level = 0;
            if(testingPreset != null)
            {
                Level = testingPreset.XPLevel;
            } else if(!stageSave.ResetStageData)
            {
                Level = stageSave.XPLEVEL;
                XP = stageSave.XP;
            } else
            {
                stageSave.XPLEVEL = 0;
                stageSave.XP = 0;
            }

            TargetXP = experienceData.GetXP(Level);
            easingManager.DoNextFrame().SetOnFinish(() => experienceUI.SetProgress(XP / TargetXP));
            experienceUI.SetLevelText(Level + 1);
        }

        public void AddXP(float xp)
        {
            XP += xp * PlayerBehavior.Player.XPMultiplier;
            stageSave.XP = XP;
            if (XP >= TargetXP)
            {
                var nextTarget = experienceData.GetXP(Level + 1);

                if(XP >= TargetXP + nextTarget)
                {
                    StartCoroutine(IncreaseLevelCoroutine());
                } else
                {
                    IncreaseLevel();
                }
            }

            experienceUI.SetProgress(XP / TargetXP);
        }

        private IEnumerator IncreaseLevelCoroutine()
        {
            while(XP >= TargetXP)
            {
                IncreaseLevel();

                // We are allowing abilities manager to set timescale to zero and show the abilities panel for each upgrade
                yield return new WaitForSeconds(0.001f);
            }

            experienceUI.SetProgress(XP / TargetXP);
        }

        private void IncreaseLevel()
        {
            Level++;
            XP -= TargetXP;

            stageSave.XPLEVEL = Level;
            stageSave.XP = XP;

            TargetXP = experienceData.GetXP(Level);

            experienceUI.SetLevelText(Level + 1);

            audioManager.PlaySound(LEVEL_UP_HASH);

            onXpLevelChanged?.Invoke(Level);
        }
    }
}