using System;

namespace Helpers.Promises
{
    public interface IPromise
    {
        IPromise Done(Action callback);
        IPromise Fail(Action<Exception> callback);
        IPromise Always(Action callback);
        IPromise Then(Func<IPromise> next);
    }

    public interface IPromise<T> : IPromise
    {
        IPromise<T> Done(Action<T> callback);
    }

    public interface IPromise<T1, T2> : IPromise
    {
        IPromise<T1, T2> Done(Action<T1, T2> callback);
    }
}