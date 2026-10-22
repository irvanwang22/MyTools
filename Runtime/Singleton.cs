using UnityEngine;

namespace MyTools
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        //static so we dont need an instance
        //this getter will make a new one or find one if it already exists
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        //Awake is called first frame a thing is active, before Start()
        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                Debug.LogFormat("Found second instance of {0}, Destroying one...", typeof(T).Name);
            }
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }
    }
}