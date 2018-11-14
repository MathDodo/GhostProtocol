using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The singleton manager gets all singletons references
/// </summary>
public class SingletonManager : SCSingletonMB<SingletonManager>
{
    [SerializeField] //The instances of the active singletons
    private List<Object> instances;

    /// <summary>
    /// Overriden the abstract method from the singleton base class which this derives from
    /// </summary>
    public override void OnInstantiated()
    {
        instances = new List<Object>();
    }

    /// <summary>
    /// Method to add new instances of singletons when they are created
    /// </summary>
    /// <param name="instance">The instance of the singleton</param>
    public void AddInstance(Object instance)
    {
        instances.Add(instance);
    }
}