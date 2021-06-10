using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<T>();
                if (_instance != null) return _instance;
                var gameObject = new GameObject();
                gameObject.AddComponent<T>();
                gameObject.hideFlags = HideFlags.DontSave;
                gameObject.name = typeof(T).Name;
                return _instance;
            }
        }

        public virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (_instance == null)
                _instance = this as T;
            else
            {
                Destroy(gameObject);
            }
        }

    }
}