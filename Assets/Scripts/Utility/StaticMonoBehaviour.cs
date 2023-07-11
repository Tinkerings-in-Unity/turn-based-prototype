using UnityEngine;

namespace Utility
{
    public class StaticMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected StaticMonoBehaviour() { }

        private static T staticInstance;

        public static T GetInstance()
        {
            if (staticInstance == null)
            {
                Debug.LogError($"Trying to get instance of {typeof(T)} static mono behaviour while it's not available.");
            }
            return staticInstance;
        }
        
        
        public static bool TryGetInstance(out T instance)
        {
            instance = GetInstance();
            return instance != null;
        }
        
        
        protected virtual void Awake()
        {
            if (staticInstance != null && staticInstance != gameObject)
            {
                Debug.LogError($"A new instance of {typeof(T)} static mono behaviour got created while another one is still present.");
                Destroy(gameObject);
            }
            else
            {
                staticInstance = (T)(object)this;
            }
            Initialize();
        }

        public virtual void Initialize()
        {
            
        }

        protected virtual void OnDestroy()
        {
            staticInstance = null;
        }


        public static T GetInstanceIfExists()
        {
            return staticInstance;
        }

        public static bool TryGetInstanceIfExists(out T instance)
        {
            instance = GetInstanceIfExists();
            return instance != null;
        }


        public static bool IsInstantiated()
        {
            return staticInstance != null;
        }
    }
}