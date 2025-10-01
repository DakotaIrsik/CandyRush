using OctoberStudio.Currency;
using UnityEngine;

namespace OctoberStudio.DI
{
    public interface IGlobalGameState
    {
        CurrencySave Gold { get; set; }
        CurrencySave TempGold { get; set; }
        AudioSource Music { get; set; }
        bool FirstTimeLoaded { get; set; }

        void Initialize();
    }
}