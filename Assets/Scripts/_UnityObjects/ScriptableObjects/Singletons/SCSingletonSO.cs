using UnityEngine;

public abstract class SCSingletonSO<T> : ScriptableObject, ISingleton where T : SCSingletonSO<T>
{
    private static T instance;
    private static bool destroyed;
    private static readonly object lockObject = new object();

    public static T Instance
    {
        get
        {
            lock (lockObject)
            {
                if (destroyed)
                {
                    return null;
                }

                if (instance == null)
                {
                    instance = CreateInstance<T>();
                    SingletonManager.Instance.AddInstance(instance);
                    DontDestroyOnLoad(instance);
                    instance.OnInstantiated();
                }

                return instance;
            }
        }
    }

    public abstract void OnInstantiated();

    protected virtual void OnDestroy()
    {
        destroyed = true;
    }
}