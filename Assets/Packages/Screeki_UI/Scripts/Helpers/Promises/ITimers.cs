using System;

namespace Helpers.Promises
{
    public interface ITimers
    {
        IPromise WaitOneFrame();
        IPromise Wait(float seconds, Action<float> progressCallback = null);
        IPromise WaitUnscaled(float seconds, Action<float> progressCallback = null);
        IPromise WaitForTrue(Func<bool> condition);
        void WaitForMainThread(Action action);
    }
}