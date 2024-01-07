using UnityEngine;

namespace Algorithms
{
    public class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
    {
        public static T Instance;

        public virtual void Awake()
        {
            Instance = (T)this;
        }
    }
}