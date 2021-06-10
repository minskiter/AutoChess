using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public abstract class GlobalSingleton<T> where T : new()
    {
        private static T _instance;
        private static readonly object Mutex = new object();

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Mutex)
                {
                    _instance ??= new T();
                }
                return _instance;
            }
        }
    }
}