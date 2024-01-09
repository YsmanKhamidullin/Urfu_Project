using System;
using System.Collections.Generic;
using UnityEngine;

namespace Helpers.Promises
{
    public class Timers : MonoBehaviour, ITimers
    {
        public Action<float> OnTimeUpdate = t => { };
        public Action<float> OnTimeUnscaledUpdate = t => { };

        private struct Awaiter
        {
            public float duration;
            public float finishTime;
            public bool unscaledTime;
            public Func<bool> additionalCondition;
            public Action<float> progressCallback;
            public Deferred resolver;
        }

        private List<Awaiter> awaiters = new List<Awaiter>();

        public static ITimers Instance { get; private set; }

        private readonly Queue<Action> mainThreadActionsQueue = new Queue<Action>();

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            lock (mainThreadActionsQueue)
            {
                while (mainThreadActionsQueue.Count > 0)
                {
                    mainThreadActionsQueue.Dequeue().Invoke();
                }
            }

            for (int i = 0; i < awaiters.Count; i++)
            {
                Awaiter candidate = awaiters[i];

                float currentTime = candidate.unscaledTime ? Time.realtimeSinceStartup : Time.time;

                if (currentTime >= candidate.finishTime)
                {
                    if (candidate.additionalCondition == null || candidate.additionalCondition() == true)
                    {
                        awaiters.RemoveAt(i);
                        if (candidate.progressCallback != null)
                        {
                            candidate.progressCallback(1f);
                        }

                        candidate.resolver.Resolve();
                        i--;
                    }
                }
                else
                {
                    if (candidate.progressCallback != null)
                    {
                        float startTime = candidate.finishTime - candidate.duration;
                        float progress = Mathf.Clamp01((currentTime - startTime) / candidate.duration);
                        candidate.progressCallback(progress);
                    }
                }
            }

            OnTimeUpdate(Time.time);
            OnTimeUnscaledUpdate(Time.realtimeSinceStartup);
        }

        public IPromise WaitOneFrame()
        {
            return WaitUnscaled(0.001f);
        }

        public IPromise Wait(float seconds, Action<float> progressCallback = null)
        {
            Deferred deferred = Deferred.GetFromPool();
            awaiters.Add(new Awaiter
            {
                duration = seconds,
                finishTime = Time.time + seconds,
                unscaledTime = false,
                additionalCondition = null,
                progressCallback = progressCallback,
                resolver = deferred,
            });
            return deferred;
        }

        public IPromise WaitUnscaled(float seconds, Action<float> progressCallback = null)
        {
            Deferred deferred = Deferred.GetFromPool();
            awaiters.Add(new Awaiter
            {
                duration = seconds,
                finishTime = Time.realtimeSinceStartup + seconds,
                unscaledTime = true,
                additionalCondition = null,
                progressCallback = progressCallback,
                resolver = deferred,
            });
            return deferred;
        }

        public IPromise WaitForTrue(Func<bool> condition)
        {
            Deferred deferred = Deferred.GetFromPool();
            awaiters.Add(new Awaiter
            {
                duration = 0f,
                finishTime = Time.realtimeSinceStartup,
                unscaledTime = true,
                additionalCondition = condition,
                progressCallback = null,
                resolver = deferred,
            });
            return deferred;
        }

        public void WaitForMainThread(Action action)
        {
            lock (mainThreadActionsQueue)
            {
                mainThreadActionsQueue.Enqueue(action);
            }
        }
    }
}