using OctoberStudio.Save;
using UnityEngine;

namespace OctoberStudio.DI
{
    public interface ICurrenciesManager
    {
        void Init();
        Sprite GetIcon(string currencyId);
        string GetName(string currencyId);
        CurrencySave GetCurrency(string currencyId);
    }
}