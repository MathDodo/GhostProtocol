using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Placeholder for the objectpool, will be expanded in the future
/// </summary>
public class ObjectPool : SCSingletonMB<ObjectPool>
{
    private List<Object> _Pool;

    public override void OnInstantiated()
    {
        _Pool = new List<Object>();
    }

    public T Spawn<T>(T original) where T : Object
    {
        var instance = Instantiate(original);
        _Pool.Add(instance);
        return instance;
    }
}