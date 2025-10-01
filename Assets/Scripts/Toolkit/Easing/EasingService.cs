using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OctoberStudio.Easing
{
    /// <summary>
    /// EasingService - Pure C# implementation of IEasingManager
    /// Created and managed entirely by VContainer
    /// Uses an updater MonoBehaviour for coroutine execution
    /// </summary>
    public class EasingService : IEasingManager
    {
        private MonoBehaviour coroutineRunner;
        private readonly List<IEasingCoroutine> activeCoroutines = new List<IEasingCoroutine>();

        public EasingService()
        {
            // Create a GameObject to run coroutines
            var easingSystemGO = new GameObject("[EasingSystem]");
            UnityEngine.Object.DontDestroyOnLoad(easingSystemGO);
            coroutineRunner = easingSystemGO.AddComponent<EasingSystemUpdater>();
        }

        /// <summary>
        /// Set the coroutine runner (for dependency injection of the updater)
        /// </summary>
        public void SetCoroutineRunner(MonoBehaviour runner)
        {
            coroutineRunner = runner;
        }

        public IEasingCoroutine DoFloat(float from, float to, float duration, UnityAction<float> action, float delay = 0)
        {
            var coroutine = new ServiceFloatEasingCoroutine(from, to, duration, delay, action, this);
            activeCoroutines.Add(coroutine);
            return coroutine;
        }

        public IEasingCoroutine DoAfter(float seconds, UnityAction action, bool unscaledTime = false)
        {
            var coroutine = new ServiceWaitCoroutine(seconds, unscaledTime, this).SetOnFinish(action);
            activeCoroutines.Add(coroutine);
            return coroutine;
        }

        public IEasingCoroutine DoAfter(Func<bool> condition)
        {
            var coroutine = new ServiceWaitForConditionCoroutine(condition, this);
            activeCoroutines.Add(coroutine);
            return coroutine;
        }

        public IEasingCoroutine DoNextFrame()
        {
            var coroutine = new ServiceNextFrameCoroutine(this);
            activeCoroutines.Add(coroutine);
            return coroutine;
        }

        public IEasingCoroutine DoNextFrame(UnityAction action)
        {
            var coroutine = new ServiceNextFrameCoroutine(this).SetOnFinish(action);
            activeCoroutines.Add(coroutine);
            return coroutine;
        }

        public IEasingCoroutine DoNextFixedFrame()
        {
            var coroutine = new ServiceNextFixedFrameCoroutine(this);
            activeCoroutines.Add(coroutine);
            return coroutine;
        }

        public Coroutine StartCustomCoroutine(IEnumerator coroutine)
        {
            if (coroutineRunner != null)
            {
                return coroutineRunner.StartCoroutine(coroutine);
            }
            Debug.LogError("[EasingService] Coroutine runner not available");
            return null;
        }

        public void StopCustomCoroutine(Coroutine coroutine)
        {
            if (coroutineRunner != null && coroutine != null)
            {
                coroutineRunner.StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Remove a coroutine from active list when it completes
        /// </summary>
        internal void RemoveCoroutine(IEasingCoroutine coroutine)
        {
            activeCoroutines.Remove(coroutine);
        }

        /// <summary>
        /// Stop all active coroutines
        /// </summary>
        public void StopAllCoroutines()
        {
            var coroutinesToStop = new List<IEasingCoroutine>(activeCoroutines);
            foreach (var coroutine in coroutinesToStop)
            {
                coroutine.Stop();
            }
            activeCoroutines.Clear();
        }
    }

    /// <summary>
    /// MonoBehaviour component to handle coroutine execution for EasingService
    /// </summary>
    public class EasingSystemUpdater : MonoBehaviour
    {
        // This component exists solely to run coroutines for the EasingService
        // No additional logic needed here
    }
}