using UnityEngine;

public abstract class RSingletonMB<T> : MonoBehaviour, ISingleton where T : RSingletonMB<T>
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
                    instance = SingletonManager.Instance.GetInstance<T>();
                    SingletonManager.Instance.AddInstance(instance);
                    instance.OnInstantiated();
                    DontDestroyOnLoad(instance);
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