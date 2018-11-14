using UnityEngine;

public abstract class SCSingletonMB<T> : MonoBehaviour, ISingleton where T : SCSingletonMB<T>
{
    private static T instance;
    private static bool destroyed = false;
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
                    var go = new GameObject(typeof(T).ToString());

                    instance = go.AddComponent<T>();
                    DontDestroyOnLoad(instance);
                    instance.OnInstantiated();
                    SingletonManager.Instance.AddInstance(instance);
                }

                return instance;
            }
        }
    }

    public abstract void OnInstantiated();

    protected virtual void OnDestroy()
    {
        destroyed = false;
    }
}