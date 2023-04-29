using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = null;
        public static T Instance
        {
            get
            {
#if UNITY_EDITOR                
                if (_instance == null)
                {
                    return FindObjectOfType<T>();
                }
#endif
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            Initialise();
        }

        protected abstract void Initialise();

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
