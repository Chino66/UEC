using System;
using System.Collections.Generic;

namespace UEC.UIFramework
{
    public class Pool<T>
    {
        protected readonly Queue<T> _queue;

        protected Func<T> CreateFunc;

        protected Action<T> GetAction;

        protected Action<T> ReturnAction;

        public int Count => _queue.Count;

        public int GetCount { get; private set; }
        public int ReuseCount { get; private set; }
        public int CreateCount { get; private set; }
        public int ReturnCount { get; private set; }

        public Pool()
        {
            _queue = new Queue<T>();
        }

        public string Information()
        {
            return
                $"Count is {Count}, GetCount is {GetCount}, ReuseCount is {ReuseCount}, CreateCount is {CreateCount}, ReturnCount is {ReturnCount}";
        }

        public Pool<T> SetCreateFunc(Func<T> func)
        {
            CreateFunc = func;
            return this;
        }

        public Pool<T> SetGetAction(Action<T> action)
        {
            GetAction = action;
            return this;
        }

        public Pool<T> SetReturnAction(Action<T> action)
        {
            ReturnAction = action;
            return this;
        }

        protected virtual T Create()
        {
            CreateCount++;
            if (CreateFunc == null)
            {
                return System.Activator.CreateInstance<T>();
            }

            return CreateFunc.Invoke();
        }

        public virtual T Get()
        {
            GetCount++;

            T item;
            if (_queue.Count > 0)
            {
                ReuseCount++;
                item = _queue.Dequeue();
            }
            else
            {
                item = Create();
            }

            GetAction?.Invoke(item);

            return item;
        }

        public virtual void Return(T item, bool checkContains = false)
        {
            if (item == null)
            {
                return;
            }

            if (checkContains && _queue.Contains(item))
            {
                return;
            }

            ReturnCount++;
            ReturnAction?.Invoke(item);

            _queue.Enqueue(item);
        }
    }
}