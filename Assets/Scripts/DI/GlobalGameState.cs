using OctoberStudio.Currency;
using OctoberStudio.Save;
using UnityEngine;

namespace OctoberStudio.DI
{
    public class GlobalGameState : IGlobalGameState
    {
        private readonly ISaveManager saveManager;

        public CurrencySave Gold { get; set; }
        public CurrencySave TempGold { get; set; }
        public AudioSource Music { get; set; }
        public bool FirstTimeLoaded { get; set; }

        public GlobalGameState(ISaveManager saveManager)
        {
            this.saveManager = saveManager;
            FirstTimeLoaded = true;
        }

        public void Initialize()
        {
            if (!saveManager.IsSaveLoaded)
            {
                Debug.LogError("[GlobalGameState] Cannot initialize - save manager not ready");
                return;
            }

            Gold = saveManager.GetSave<CurrencySave>("gold");
            TempGold = saveManager.GetSave<CurrencySave>("temp_gold");
            Debug.Log("[GlobalGameState] Initialized successfully");
        }
    }
}