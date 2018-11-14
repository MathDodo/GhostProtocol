using UnityEngine;

/// <summary>
/// Base class for self creating scribtable object singletons, which other classes can derive from to become singletons
/// </summary>
/// <typeparam name="T">This generic type, needs to be the type of the derived class</typeparam>
public abstract class SCSingletonSO<T> : ScriptableObject, ISingleton where T : SCSingletonSO<T>
{
    //The singleton instance field
    private static T instance;

    //Whether if the singleton has been destroyed should only happen when the game closes
    private static bool destroyed = false;

    //Object to achieve a lock from
    private static readonly object lockObject = new object();

    /// <summary>
    /// Get accesor for the singleton Instance
    /// </summary>
    public static T Instance
    {
        get
        {
            //Locking for thread safe
            lock (lockObject)
            {
                if (destroyed)
                {
                    return null;
                }

                if (instance == null)
                {
                    instance = CreateInstance<T>();
                    instance.OnInstantiated();
                    DontDestroyOnLoad(instance);
                    SingletonManager.Instance.AddInstance(instance);
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