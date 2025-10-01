using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OctoberStudio.Easing
{
    /// <summary>
    /// Interface for easing animation system
    /// Provides methods for creating and managing easing animations
    /// </summary>
    public interface IEasingManager
    {
        /// <summary>
        /// Animate a float value over time with easing
        /// </summary>
        IEasingCoroutine DoFloat(float from, float to, float duration, UnityAction<float> action, float delay = 0);

        /// <summary>
        /// Execute an action after a specified delay
        /// </summary>
        IEasingCoroutine DoAfter(float seconds, UnityAction action, bool unscaledTime = false);

        /// <summary>
        /// Wait for a condition to be true
        /// </summary>
        IEasingCoroutine DoAfter(Func<bool> condition);

        /// <summary>
        /// Execute an action on the next frame
        /// </summary>
        IEasingCoroutine DoNextFrame();

        /// <summary>
        /// Execute an action on the next frame
        /// </summary>
        IEasingCoroutine DoNextFrame(UnityAction action);

        /// <summary>
        /// Execute an action on the next fixed update frame
        /// </summary>
        IEasingCoroutine DoNextFixedFrame();

        /// <summary>
        /// Start a custom coroutine
        /// </summary>
        Coroutine StartCustomCoroutine(IEnumerator coroutine);

        /// <summary>
        /// Stop a custom coroutine
        /// </summary>
        void StopCustomCoroutine(Coroutine coroutine);
    }
}