namespace OctoberStudio.DI
{
    public interface IStageState
    {
        StageData CurrentStage { get; set; }
        bool IsGamePaused { get; set; }
        float ElapsedTime { get; }
    }
}