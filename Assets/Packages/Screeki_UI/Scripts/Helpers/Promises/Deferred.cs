using System;
using System.Collections.Generic;

namespace Helpers.Promises
{
    public class Deferred : BaseDeferred, IPromise
    {
        public static int PoolCapacity
        {
            get { return poolQueue.Count; }
        }

        public static int CreatedCount { get; private set; }
        private static Queue<Deferred> poolQueue = new Queue<Deferred>();

        protected Deferred()
        {
        }

        ~Deferred()
        {
            lock (poolQueue)
            {
                if (poolQueue.Contains(this) == false)
                {
                    Reset();
                    poolQueue.Enqueue(this);
                    GC.ReRegisterForFinalize(this);
                }
            }
        }

        public static Deferred GetFromPool()
        {
            if (poolQueue.Count == 0)
            {
                CreatedCount++;
                return new Deferred();
            }

            lock (poolQueue)
            {
                var element = poolQueue.Dequeue();
                if (element == null)
                {
                    return new Deferred();
                }

                return element;
            }
        }

        public static IPromise Rejected(string reason, params object[] args)
        {
            return GetFromPool().Reject(reason, args);
        }

        public IPromise Resolve()
        {
            if (CurrentState != States.Pending)
            {
                throw new InvalidOperationException(string.Format("CurrentState == {0}", CurrentState));
            }

            CurrentState = States.Resolved;

            for (int i = 0, maxi = DoneCallbacks.Count - 1; i <= maxi; i++)
            {
                DoneCallbacks[i]();
            }

            ClearCallbacks();

            return this;
        }

        public IPromise Reject(string reason, params object[] args)
        {
            try
            {
                return Reject(new Exception(string.Format(reason, args)));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
                return Reject(new Exception(reason));
            }
        }

        public IPromise Reject(Exception exception)
        {
            if (CurrentState != States.Pending)
            {
                throw new InvalidOperationException();
            }

            CurrentState = States.Rejected;
            RejectReason = exception;

            for (int i = 0, maxi = FailCallbacks.Count - 1; i <= maxi; i++)
            {
                FailCallbacks[i](exception);
            }

            ClearCallbacks();

            return this;
        }

        /// <summary>
        /// Resolves when all in IPromise array are resolved
        /// Fails if any in IPromise array failed
        /// </summary>
        /// <param name="collection">promises to complete</param>
        /// <returns>IPromise</returns>
        public static IPromise All(params IPromise[] collection)
        {
            return AllInternal(collection);
        }

        /// <summary>
        /// Resolves when all in IPromise array are resolved
        /// Fails if any in IPromise array failed
        /// </summary>
        /// <param name="collection">promises to complete</param>
        /// <returns>IPromise</returns>
        public static IPromise All(List<IPromise> collection)
        {
            return AllInternal(collection);
        }

        private static IPromise AllInternal(ICollection<IPromise> collection)
        {
            Deferred deferred = Deferred.GetFromPool();

            if (collection.Count == 0)
            {
                deferred.Resolve();
            }
            else
            {
                int promisesToComplete = collection.Count;

                foreach (IPromise element in collection)
                {
                    element.Done(() =>
                    {
                        promisesToComplete--;
                        if (deferred.CurrentState == States.Pending && promisesToComplete == 0)
                        {
                            deferred.Resolve();
                        }
                    });

                    element.Fail(ex =>
                    {
                        if (deferred.CurrentState == States.Pending)
                        {
                            deferred.Reject(ex);
                        }
                    });
                }
            }

            return deferred;
        }

        /// <summary>
        /// Resolves when any in IPromise array is resolved
        /// Fails if all in IPromise array are failed
        /// </summary>
        /// <param name="collection">promises to complete</param>
        /// <returns>IPromise</returns>
        public static IPromise Race(params IPromise[] collection)
        {
            Deferred deferred = Deferred.GetFromPool();

            if (collection.Length == 0)
            {
                deferred.Reject(new Exception("Deferred.Race called with empty array - no winner"));
            }
            else
            {
                int promisesToWait = collection.Length;

                for (int i = 0, maxi = collection.Length - 1; i <= maxi; i++)
                {
                    collection[i].Done(() =>
                    {
                        if (deferred.CurrentState == States.Pending)
                        {
                            deferred.Resolve();
                        }
                    });

                    collection[i].Fail(ex =>
                    {
                        promisesToWait--;
                        if (deferred.CurrentState == States.Pending && promisesToWait == 0)
                        {
                            deferred.Reject(ex);
                        }
                    });
                }
            }

            return deferred;
        }

        /// <summary>
        /// Starts all IPromise constructors step-by-step until one of IPromise failed.
        /// Resolved with last IPromise
        /// Fails if any of IPromise failed
        /// </summary>
        /// <param name="collection">promises contructor array</param>
        /// <returns>IPromise</returns>
        public static IPromise Sequence(params Func<IPromise>[] collection)
        {
            IPromise last = Deferred.GetFromPool().Resolve();

            for (int i = 0, maxi = collection.Length - 1; i <= maxi; i++)
            {
                last = last.Then(collection[i]);
            }

            return last;
        }
    }

    public class Deferred<T> : BaseDeferred, IPromise<T>
    {
        public static int PoolCapacity
        {
            get { return poolQueue.Count; }
        }

        public static int CreatedCount { get; private set; }
        private static Queue<Deferred<T>> poolQueue = new Queue<Deferred<T>>();

        protected T Result;

        protected Deferred()
        {
        }

        ~Deferred()
        {
            if (poolQueue.Contains(this) == false)
            {
                Reset();
                poolQueue.Enqueue(this);
                GC.ReRegisterForFinalize(this);
            }
        }

        protected override void Reset()
        {
            base.Reset();
            Result = default(T);
        }

        public static Deferred<T> GetFromPool()
        {
            if (poolQueue.Count == 0)
            {
                CreatedCount++;
                return new Deferred<T>();
            }

            var element = poolQueue.Dequeue();
            if (element == null)
            {
                return new Deferred<T>();
            }

            return element;
        }

        public static IPromise<T> Rejected(string reason, params object[] args)
        {
            return GetFromPool().Reject(new Exception(string.Format(reason, args)));
        }

        public IPromise<T> Resolve(T result)
        {
            if (CurrentState != States.Pending)
            {
                throw new InvalidOperationException();
            }

            CurrentState = States.Resolved;
            Result = result;

            for (int i = 0, maxi = DoneCallbacks.Count - 1; i <= maxi; i++)
            {
                DoneCallbacks[i]();
            }

            ClearCallbacks();

            return this;
        }

        public IPromise<T> Reject(string reason, params object[] args)
        {
            return Reject(new Exception(string.Format(reason, args)));
        }

        public IPromise<T> Reject(Exception exception)
        {
            if (CurrentState != States.Pending)
            {
                throw new InvalidOperationException();
            }

            CurrentState = States.Rejected;
            RejectReason = exception;

            for (int i = 0, maxi = FailCallbacks.Count - 1; i <= maxi; i++)
            {
                FailCallbacks[i](exception);
            }

            ClearCallbacks();

            return this;
        }

        public IPromise<T> Done(Action<T> callback)
        {
            switch (CurrentState)
            {
                case States.Resolved:
                    callback(Result);
                    break;
                case States.Pending:
                    DoneCallbacks.Add(() => callback(Result));
                    break;
            }

            return this;
        }
    }

    public class Deferred<T1, T2> : BaseDeferred, IPromise<T1, T2>
    {
        public static int PoolCapacity
        {
            get { return poolQueue.Count; }
        }

        public static int CreatedCount { get; private set; }
        private static Queue<Deferred<T1, T2>> poolQueue = new Queue<Deferred<T1, T2>>();

        protected T1 result1;
        protected T2 result2;

        ~Deferred()
        {
            if (poolQueue.Contains(this) == false)
            {
                Reset();
                poolQueue.Enqueue(this);
                GC.ReRegisterForFinalize(this);
            }
        }

        protected override void Reset()
        {
            base.Reset();
            result1 = default(T1);
            result2 = default(T2);
        }

        public static Deferred<T1, T2> GetFromPool()
        {
            if (poolQueue.Count == 0)
            {
                CreatedCount++;
                return new Deferred<T1, T2>();
            }

            var element = poolQueue.Dequeue();
            if (element == null)
            {
                return new Deferred<T1, T2>();
            }

            return element;
        }

        public static IPromise<T1, T2> Rejected(string reason, params object[] args)
        {
            return GetFromPool().Reject(new Exception(string.Format(reason, args)));
        }

        public IPromise<T1, T2> Resolve(T1 result1, T2 result2)
        {
            if (CurrentState != States.Pending)
            {
                throw new InvalidOperationException();
            }

            CurrentState = States.Resolved;
            this.result1 = result1;
            this.result2 = result2;

            for (int i = 0, maxi = DoneCallbacks.Count - 1; i <= maxi; i++)
            {
                DoneCallbacks[i]();
            }

            ClearCallbacks();

            return this;
        }

        public IPromise<T1, T2> Reject(string reason, params object[] args)
        {
            return Reject(new Exception(string.Format(reason, args)));
        }

        public IPromise<T1, T2> Reject(Exception exception)
        {
            if (CurrentState != States.Pending)
            {
                throw new InvalidOperationException();
            }

            CurrentState = States.Rejected;
            RejectReason = exception;

            for (int i = 0, maxi = FailCallbacks.Count - 1; i <= maxi; i++)
            {
                FailCallbacks[i](exception);
            }

            ClearCallbacks();

            return this;
        }

        public IPromise<T1, T2> Done(Action<T1, T2> callback)
        {
            switch (CurrentState)
            {
                case States.Resolved:
                    callback(result1, result2);
                    break;
                case States.Pending:
                    DoneCallbacks.Add(() => callback(result1, result2));
                    break;
            }

            return this;
        }
    }
}