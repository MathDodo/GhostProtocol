using System.Collections.Generic;
using UnityEngine;

public class SingletonManager : SCSingletonMB<SingletonManager>
{
    [SerializeField]
    private List<Object> instances;

    [SerializeField]
    private List<Object> resourceSingletons;

    public override void OnInstantiated()
    {
        instances = new List<Object>();

        resourceSingletons = new List<Object>(Resources.LoadAll("Singletons", typeof(ISingleton)));
    }

    public void AddInstance(Object instance)
    {
        instances.Add(instance);
    }

    public T GetInstance<T>() where T : Object, ISingleton
    {
        T instance;

        if ((instance = (T)resourceSingletons.Find(i => i.GetType() == typeof(T))) != null)
        {
            return Instantiate(instance);
        }

        return null;
    }
}