using UnityEngine;

namespace OctoberStudio.DI
{
    public class StageState : IStageState
    {
        public StageData CurrentStage { get; set; }
        public bool IsGamePaused { get; set; }
        public float ElapsedTime => Time.time;
    }
}