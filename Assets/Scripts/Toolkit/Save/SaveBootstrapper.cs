using System.Collections;
using UnityEngine;
using VContainer;

namespace OctoberStudio.Save
{
    public class SaveBootstrapper : MonoBehaviour
    {
        [SerializeField] private bool autoSaveEnabled = true;
        [SerializeField] private float autoSaveDelay = 10f;

        private ISaveManager saveService;
        private Coroutine saveCoroutine;

        [Inject]
        public void Construct(ISaveManager saveService)
        {
            this.saveService = saveService;
        }

        private void Start()
        {
            // Ensure this persists across scenes
            DontDestroyOnLoad(gameObject);

            if (autoSaveEnabled)
            {
                StartCoroutine(AutoSaveCoroutine());
            }
        }

        private IEnumerator AutoSaveCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(autoSaveDelay);

                if (saveService.IsSaveLoaded)
                {
                    saveService.Save(false);
                }
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && saveService.IsSaveLoaded)
            {
                saveService.Save(false);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && saveService.IsSaveLoaded)
            {
                saveService.Save(false);
            }
        }

        private void OnDestroy()
        {
            if (saveCoroutine != null)
            {
                StopCoroutine(saveCoroutine);
            }

            if (saveService.IsSaveLoaded)
            {
                saveService.Save(false);
            }
        }
    }
}