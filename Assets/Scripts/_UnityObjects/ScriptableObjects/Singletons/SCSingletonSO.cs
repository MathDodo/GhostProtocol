using UnityEngine;

/// <summary>
/// Base class for self creating scribtable object singletons, which other classes can derive from to become singletons
/// </summary>
/// <typeparam name="T">This generic type, needs to be the type of the derived class</typeparam>
public abstract class SCSingletonSO<T> : ScriptableObject, ISingleton where T : SCSingletonSO<T>
{
    //The singleton instance field
    private static T _instance;

    //Object to achieve a lock from
    private static readonly object _lockObject = new object();

    //Whether if the singleton has been destroyed should only happen when the game closes
    protected static bool _destroyed = false;

    /// <summary>
    /// Get accesor for the singleton Instance
    /// </summary>
    public static T Instance
    {
        get
        {
            //Locking for thread safe
            lock (_lockObject)
            {
                if (_destroyed)
                {
                    return null;
                }

                if (_instance == null)
                {
                    _instance = CreateInstance<T>();
                    _instance.OnInstantiated();
                    DontDestroyOnLoad(_instance);
                    SingletonManager.Instance.AddInstance(_instance);
                }

                return _instance;
            }
        }
    }

    public abstract void OnInstantiated();

    protected virtual void OnDestroy()
    {
        _destroyed = true;
    }
}