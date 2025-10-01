using OctoberStudio.UI;
using UnityEngine;
using UnityEngine.Playables;
using VContainer;
using VContainer.Unity;

namespace OctoberStudio.DI
{
    public class StageLifetimeScope : LifetimeScope
    {
        [SerializeField] private StageController stageController;

        protected override void Configure(IContainerBuilder builder)
        {
            // Register scene-specific managers with their interface implementations
            builder.RegisterComponent(stageController.ExperienceManager).As<IExperienceManager>();
            builder.RegisterComponent(stageController.AbilityManager).As<IAbilityManager>();
            builder.RegisterComponent(stageController.FieldManager).As<IStageFieldManager>();
            builder.RegisterComponent(stageController.DropManager).As<IDropManager>();
            builder.RegisterComponent(stageController.CameraController).As<ICameraManager>();
            builder.RegisterComponent(stageController.PoolsManager).As<IPoolsManager>();
            builder.RegisterComponent(stageController.WorldSpaceTextManager).As<IWorldSpaceTextManager>();
            builder.RegisterComponent(stageController.EnemiesSpawner).As<IEnemiesSpawner>();

            // Register scene-specific data
            builder.RegisterInstance(stageController.Director);
            builder.RegisterInstance(stageController.GameScreen);

            // Register stage state
            builder.Register<IStageState, StageState>(Lifetime.Singleton);

            // Register player for dependency injection
            builder.RegisterComponentInHierarchy<PlayerBehavior>();
        }
    }
}