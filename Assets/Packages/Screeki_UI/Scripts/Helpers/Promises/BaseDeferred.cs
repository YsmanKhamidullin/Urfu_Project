using System;
using System.Collections.Generic;

namespace Helpers.Promises
{
    public abstract class BaseDeferred : IPromise
    {
        protected enum States
        {
            Pending,
            Resolved,
            Rejected,
        }

        protected States CurrentState;

        protected List<Action> DoneCallbacks = new List<Action>(10);
        protected List<Action<Exception>> FailCallbacks = new List<Action<Exception>>(10);

        protected Exception RejectReason;

        protected BaseDeferred()
        {
            CurrentState = States.Pending;
        }

        /// <summary>
        /// Executes when Promise successfully resolved
        /// </summary>
        /// <param name="callback">callback to execute</param>
        /// <returns>itself</returns>
        public IPromise Done(Action callback)
        {
            switch (CurrentState)
            {
                case States.Resolved:
                    callback();
                    break;
                case States.Pending:
                    DoneCallbacks.Add(callback);
                    break;
            }

            return this;
        }

        /// <summary>
        /// Executes when Promise failed, Exception provided for details
        /// </summary>
        /// <param name="callback">callback to execute</param>
        /// <returns>itself</returns>
        public IPromise Fail(Action<Exception> callback)
        {
            switch (CurrentState)
            {
                case States.Rejected:
                    callback(RejectReason);
                    break;
                case States.Pending:
                    FailCallbacks.Add(callback);
                    break;
            }

            return this;
        }

        /// <summary>
        /// Executes parameterless callback when Promise failed or resolved, use Fail() or Done() for additional parameters
        /// </summary>
        /// <param name="callback">callback to execute</param>
        /// <returns>itself</returns>
        public IPromise Always(Action callback)
        {
            switch (CurrentState)
            {
                case States.Resolved:
                case States.Rejected:
                    callback();
                    break;
                case States.Pending:
                    DoneCallbacks.Add(callback);
                    FailCallbacks.Add(ex => callback());
                    break;
            }

            return this;
        }

        /// <summary>
        /// adds new promise constructor to current promise and returns wrapper that resolves with added promise
        /// A.Then(B) - returns promise that resolves when promise from B is resolved
        /// </summary>
        /// <param name="next">constructor for next promise</param>
        /// <returns>Promise</returns>
        public IPromise Then(Func<IPromise> next)
        {
            Deferred deferred = Deferred.GetFromPool();

            Done(() =>
            {
                IPromise promise = next();

                promise.Done(() => deferred.Resolve());
                promise.Fail(ex => deferred.Reject(ex));
            });

            Fail(ex => deferred.Reject(ex));

            return deferred;
        }

        protected virtual void ClearCallbacks()
        {
            DoneCallbacks.Clear();
            FailCallbacks.Clear();
        }

        protected virtual void Reset()
        {
            ClearCallbacks();
            CurrentState = States.Pending;
            RejectReason = null;
        }

        protected static bool IsResolved(BaseDeferred baseDeferred)
        {
            return baseDeferred.CurrentState == States.Resolved;
        }

        protected static bool IsPending(BaseDeferred baseDeferred)
        {
            return baseDeferred.CurrentState == States.Pending;
        }

        protected static bool IsRejected(BaseDeferred baseDeferred)
        {
            return baseDeferred.CurrentState == States.Rejected;
        }
    }
}