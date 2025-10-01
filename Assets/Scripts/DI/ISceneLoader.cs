using System.Collections;

namespace OctoberStudio.DI
{
    /// <summary>
    /// Service interface for handling scene loading operations.
    /// Replaces static GameController.LoadStage() and GameController.LoadMainMenu() methods.
    /// </summary>
    public interface ISceneLoader
    {
        /// <summary>
        /// Loads the game stage scene with proper save data initialization.
        /// </summary>
        void LoadStage();

        /// <summary>
        /// Loads the main menu scene and transfers temporary gold to permanent gold.
        /// </summary>
        void LoadMainMenu();
    }
}