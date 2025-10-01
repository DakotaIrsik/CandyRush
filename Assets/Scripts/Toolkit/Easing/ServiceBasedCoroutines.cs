using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace OctoberStudio.Easing
{
    /// <summary>
    /// Base class for service-based easing coroutines
    /// Uses injected EasingService instead of static EasingManager
    /// </summary>
    public abstract class ServiceBasedCoroutine : IEasingCoroutine
    {
        protected Coroutine coroutine;
        protected readonly EasingService easingService;

        public bool IsActive { get; protected set; }

        protected UnityAction finishCallback;
        protected EasingType easingType = EasingType.Linear;
        protected float delay = -1;
        protected bool unscaledTime;
        protected bool useCurve;
        protected AnimationCurve easingCurve;

        public ServiceBasedCoroutine(EasingService easingService)
        {
            this.easingService = easingService;
        }

        public IEasingCoroutine SetEasing(EasingType easingType)
        {
            this.easingType = easingType;
            useCurve = false;
            return this;
        }

        public IEasingCoroutine SetOnFinish(UnityAction callback)
        {
            finishCallback = callback;
            return this;
        }

        public IEasingCoroutine SetUnscaledTime(bool unscaledTime)
        {
            this.unscaledTime = unscaledTime;
            return this;
        }

        public IEasingCoroutine SetEasingCurve(AnimationCurve curve)
        {
            easingCurve = curve;
            useCurve = true;
            return this;
        }

        public IEasingCoroutine SetDelay(float delay)
        {
            this.delay = delay;
            return this;
        }

        public virtual void Stop()
        {
            if (coroutine != null)
            {
                easingService?.StopCustomCoroutine(coroutine);
                coroutine = null;
            }
            IsActive = false;
            easingService?.RemoveCoroutine(this);
        }

        protected virtual void OnComplete()
        {
            finishCallback?.Invoke();
            IsActive = false;
            easingService?.RemoveCoroutine(this);
        }
    }

    public class ServiceNextFrameCoroutine : ServiceBasedCoroutine
    {
        public ServiceNextFrameCoroutine(EasingService easingService) : base(easingService)
        {
            coroutine = easingService.StartCustomCoroutine(Coroutine());
        }

        private IEnumerator Coroutine()
        {
            IsActive = true;
            yield return null;
            OnComplete();
        }
    }

    public class ServiceNextFixedFrameCoroutine : ServiceBasedCoroutine
    {
        public ServiceNextFixedFrameCoroutine(EasingService easingService) : base(easingService)
        {
            coroutine = easingService.StartCustomCoroutine(Coroutine());
        }

        private IEnumerator Coroutine()
        {
            IsActive = true;
            yield return new WaitForFixedUpdate();
            OnComplete();
        }
    }

    public class ServiceWaitCoroutine : ServiceBasedCoroutine
    {
        private float duration;

        public ServiceWaitCoroutine(float duration, bool unscaledTime, EasingService easingService) : base(easingService)
        {
            this.duration = duration;
            this.unscaledTime = unscaledTime;
            coroutine = easingService.StartCustomCoroutine(Coroutine());
        }

        private IEnumerator Coroutine()
        {
            IsActive = true;

            while (delay > 0)
            {
                yield return null;
                delay -= unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            if (unscaledTime)
            {
                yield return new WaitForSecondsRealtime(duration);
            }
            else
            {
                yield return new WaitForSeconds(duration);
            }

            OnComplete();
        }
    }

    public class ServiceWaitForConditionCoroutine : ServiceBasedCoroutine
    {
        private Func<bool> condition;

        public ServiceWaitForConditionCoroutine(Func<bool> condition, EasingService easingService) : base(easingService)
        {
            this.condition = condition;
            coroutine = easingService.StartCustomCoroutine(Coroutine());
        }

        private IEnumerator Coroutine()
        {
            IsActive = true;

            while (delay > 0)
            {
                yield return null;
                delay -= unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            }

            do
            {
                yield return null;
            } while (!condition());

            OnComplete();
        }
    }

    public abstract class ServiceBasedEasingCoroutine<T> : ServiceBasedCoroutine
    {
        protected T from;
        protected T to;
        private float duration;
        protected UnityAction<T> callback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract T Lerp(T a, T b, float t);

        public ServiceBasedEasingCoroutine(T from, T to, float duration, float delay, UnityAction<T> callback, EasingService easingService) : base(easingService)
        {
            this.from = from;
            this.to = to;
            this.duration = duration;
            this.callback = callback;
            this.delay = delay;
            coroutine = easingService.StartCustomCoroutine(Coroutine());
        }

        private IEnumerator Coroutine()
        {
            IsActive = true;
            float time = 0;

            while (time < duration)
            {
                yield return null;

                if (delay > 0)
                {
                    delay -= unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    if (delay > 0) continue;
                }

                time += unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float t;
                if (useCurve)
                {
                    t = easingCurve.Evaluate(time / duration);
                }
                else
                {
                    t = EasingFunctions.ApplyEasing(time / duration, easingType);
                }

                T value = Lerp(from, to, t);
                callback?.Invoke(value);
            }

            callback?.Invoke(to);
            OnComplete();
        }
    }

    public class ServiceFloatEasingCoroutine : ServiceBasedEasingCoroutine<float>
    {
        public ServiceFloatEasingCoroutine(float from, float to, float duration, float delay, UnityAction<float> callback, EasingService easingService)
            : base(from, to, duration, delay, callback, easingService)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override float Lerp(float a, float b, float t)
        {
            return Mathf.LerpUnclamped(a, b, t);
        }
    }

    public class ServiceVectorEasingCoroutine3 : ServiceBasedEasingCoroutine<Vector3>
    {
        public ServiceVectorEasingCoroutine3(Vector3 from, Vector3 to, float duration, float delay, UnityAction<Vector3> callback, EasingService easingService)
            : base(from, to, duration, delay, callback, easingService)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return Vector3.LerpUnclamped(a, b, t);
        }
    }

    public class ServiceVectorEasingCoroutine2 : ServiceBasedEasingCoroutine<Vector2>
    {
        public ServiceVectorEasingCoroutine2(Vector2 from, Vector2 to, float duration, float delay, UnityAction<Vector2> callback, EasingService easingService)
            : base(from, to, duration, delay, callback, easingService)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return Vector2.LerpUnclamped(a, b, t);
        }
    }

    public class ServiceColorEasingCoroutine : ServiceBasedEasingCoroutine<Color>
    {
        public ServiceColorEasingCoroutine(Color from, Color to, float duration, float delay, UnityAction<Color> callback, EasingService easingService)
            : base(from, to, duration, delay, callback, easingService)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Color Lerp(Color a, Color b, float t)
        {
            return Color.LerpUnclamped(a, b, t);
        }
    }
}