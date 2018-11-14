using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Placeholder for the objectpool, will be expanded in the future
/// </summary>
public class ObjectPool : SCSingletonMB<ObjectPool>
{
    private List<Object> pool;

    public override void OnInstantiated()
    {
        pool = new List<Object>();
    }

    public T Spawn<T>(T original) where T : Object
    {
        var instance = Instantiate(original);
        pool.Add(instance);
        return instance;
    }
}