using System.Threading.Tasks;

namespace OctoberStudio.DI
{
    public interface IInitializationService
    {
        Task InitializeAsync();
        bool IsInitialized { get; }
    }
}