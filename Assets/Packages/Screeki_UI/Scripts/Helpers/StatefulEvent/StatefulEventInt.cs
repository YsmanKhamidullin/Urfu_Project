using System;
using UnityEngine;

namespace Helpers.StatefulEvent
{
    public static class StatefulEventInt
    {
        public static StatefulEventInt<bool> Create(bool defaultValue)
        {
            return new StatefulEventInt<bool>(defaultValue, (a, b) => a == b);
        }
        public static StatefulEventInt<string> Create(string defaultValue)
        {
            return new StatefulEventInt<string>(defaultValue, (a, b) => a == b);
        }
        public static StatefulEventInt<int> Create(int defaultValue)
        {
            return new StatefulEventInt<int>(defaultValue, (a, b) => a == b);
        }
        public static StatefulEventInt<float> Create(float defaultValue)
        {
            return new StatefulEventInt<float>(defaultValue, (a, b) => a == b);
        }
        public static StatefulEventInt<uint> Create(uint defaultValue)
        {
            return new StatefulEventInt<uint>(defaultValue, (a, b) => a == b);
        }
        public static StatefulEventInt<ulong> Create(ulong defaultValue)
        {
            return new StatefulEventInt<ulong>(defaultValue, (a, b) => a == b);
        }
        public static StatefulEventInt<Vector3Int> Create(Vector3Int defaultValue)
        {
            return new StatefulEventInt<Vector3Int>(defaultValue, (a, b) => a == b);
        }
        public static StatefulEventInt<T> CreateEnum<T>(T defaultValue) where T : Enum
        {
            return new StatefulEventInt<T>(defaultValue, (a, b) => a.GetHashCode() == b.GetHashCode());
        }
        public static StatefulEventInt<T> CreateGenericStruct<T>(T defaultValue) where T : struct, IValue<T>
        {
            return new StatefulEventInt<T>(defaultValue, (a, b) => a.Equals(b));
        }
        public static StatefulEventInt<T> CreateGenericClass<T>(T defaultValue) where T : class, IValue<T>
        {
            return new StatefulEventInt<T>(defaultValue, (a, b) => a != null && b != null && a.Equals(b));
        }
    }

    public class StatefulEventInt<T> : IStatefulEvent<T>
    {
        public event Action<T> OnValueChanged = t => { };
        public T Value { get; private set; }

        private readonly T defaultValue;
        private Func<T, T, bool> equator;

        internal StatefulEventInt(T defaultValue, Func<T, T, bool> equator)
        {
            this.equator = equator;
            this.defaultValue = defaultValue;
            this.Value = defaultValue;
        }

        public void Set(T newValue)
        {
            if (equator(Value, newValue) == false)
            {
                Value = newValue;
                OnValueChanged(Value);
            }
        }

        public StatefulEventInt<T, T2> Add<T2>(StatefulEventInt<T2> value2)
        {
            return new StatefulEventInt<T, T2>(this, value2);
        }

        public void Reset()
        {
            OnValueChanged = v => { };
            Value = defaultValue;
        }
    }

    public class StatefulEventInt<T1, T2> : IStatefulEvent<T1, T2>
    {
        public event Action<T1, T2> OnValueChanged = (t1, t2) => { };
        public T1 Value1 => value1.Value;
        public T2 Value2 => value2.Value;

        private readonly StatefulEventInt<T1> value1;
        private readonly StatefulEventInt<T2> value2;
        private bool someValueChanged = false;

        public StatefulEventInt(StatefulEventInt<T1> value1, StatefulEventInt<T2> value2)
        {
            this.value1 = value1;
            this.value2 = value2;
            Reset();
        }

        public void SetValue1(T1 newValue1)
        {
            value1.Set(newValue1);
            FireEventIfChanged();
        }

        public void SetValue2(T2 newValue2)
        {
            value2.Set(newValue2);
            FireEventIfChanged();
        }

        public void SetValues(T1 newValue1, T2 newValue2)
        {
            value1.Set(newValue1);
            value2.Set(newValue2);
            FireEventIfChanged();
        }

        private void FireEventIfChanged()
        {
            if (someValueChanged == true)
            {
                someValueChanged = false;
                OnValueChanged(value1.Value, value2.Value);
            }
        }

        public void Reset()
        {
            OnValueChanged = (t1, t2) => { };

            value1.Reset();
            value1.OnValueChanged += v => { someValueChanged = true; };

            value2.Reset();
            value2.OnValueChanged += v => { someValueChanged = true; };
        }
    }
}
