using UnityEngine;

/// <summary>
/// Base class for self creating monobehaviour singletons, which other classes can derive from to become singletons
/// </summary>
/// <typeparam name="T">This generic type, needs to be the type of the derived class</typeparam>
public abstract class SCSingletonMB<T> : MonoBehaviour, ISingleton where T : SCSingletonMB<T>
{
    //The singleton instance field
    private static T instance;

    //Object to achieve a lock from
    private static readonly object lockObject = new object();

    //Whether if the singleton has been destroyed should only happen when the game closes
    protected static bool destroyed = false;

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
                    //Creating a gameobject with the name of the class type
                    GameObject go = new GameObject(typeof(T).ToString());
                    instance = go.AddComponent<T>();
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