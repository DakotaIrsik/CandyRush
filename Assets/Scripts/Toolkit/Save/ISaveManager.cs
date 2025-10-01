using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OctoberStudio.Save
{
    public interface ISaveManager
    {
        T GetSave<T>(int hash) where T : ISave, new();
        T GetSave<T>(string uniqueName) where T : ISave, new();

        void Save(bool multithreading = false);

        // Properties for initialization checking
        bool IsSaveLoaded { get; }
        event UnityAction OnSaveLoaded;
    }
}