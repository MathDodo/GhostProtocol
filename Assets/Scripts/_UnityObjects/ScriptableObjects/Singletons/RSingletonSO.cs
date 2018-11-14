using UnityEngine;

public abstract class RSingletonSO<T> : ScriptableObject, ISingleton where T : RSingletonSO<T>
{
    private static T instance;
    private static bool destroyed;
    private static readonly object lockObject = new object();

    public static T Instance
    {
        get
        {
            if (destroyed)
            {
                return null;
            }

            if (instance == null)
            {
                instance = SingletonManager.Instance.GetInstance<T>();
                DontDestroyOnLoad(instance);
                SingletonManager.Instance.AddInstance(instance);
                instance.OnInstantiated();
            }

            return instance;
        }
    }

    public abstract void OnInstantiated();

    protected virtual void OnDestroy()
    {
        destroyed = true;
    }
}