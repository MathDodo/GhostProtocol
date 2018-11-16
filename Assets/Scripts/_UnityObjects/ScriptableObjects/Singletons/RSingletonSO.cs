using UnityEngine;

/// <summary>
/// Base class for resource creating scribtable object singletons, which other classes can derive from to become singletons
/// </summary>
/// <typeparam name="T">This generic type, needs to be the type of the derived class</typeparam>
public abstract class RSingletonSO<T> : ScriptableObject, ISingleton where T : RSingletonSO<T>
{
    //The singleton instance field
    private static T _Instance;

    //Object to achieve a lock from
    private static readonly object _LockObject = new object();

    //Whether if the singleton has been destroyed should only happen when the game closes
    protected static bool _Destroyed = false;

    /// <summary>
    /// Get accesor for the singleton Instance
    /// </summary>
    public static T Instance
    {
        get
        {
            //Locking for thread safe
            lock (_LockObject)
            {
                if (_Destroyed)
                {
                    return null;
                }

                if (_Instance == null)
                {
                    _Instance = SingletonManager.Instance.GetInstance<T>();
                    _Instance.OnInstantiated();
                    DontDestroyOnLoad(_Instance);
                    SingletonManager.Instance.AddInstance(_Instance);
                }

                return _Instance;
            }
        }
    }

    public abstract void OnInstantiated();

    protected virtual void OnDestroy()
    {
        _Destroyed = true;
    }
}