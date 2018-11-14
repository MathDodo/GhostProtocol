using UnityEngine;

/// <summary>
/// This script is for testing purposes
/// </summary>
public class TestScript : MonoBehaviour
{
    [SerializeField] //If it should spawn a clone through the object pool
    private bool doSpawn = false;

    /// <summary>
    /// Starting method called by unity, used to spawn a clone through the object pool
    /// </summary>
    private void Start()
    {
        if (doSpawn)
        {
            doSpawn = false;
            ObjectPool.Instance.Spawn(this);
        }
    }
}