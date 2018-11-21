using UnityEngine;

/// <summary>
/// The camera manager class which needs a prefab in the resources folder, with the attribute requirecomponent and the the type of the camera which means that a camera component will
/// automatically be added when this class is added to a gameobject (in editor at least)
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraManager : RSingletonMB<CameraManager>
{
    //The camera field reference
    private Camera _cam;

    /// <summary>
    /// Get accesor for the camera
    /// </summary>
    public Camera Cam { get { return _cam; } }

    public override void OnInstantiated()
    {
        _cam = GetComponent<Camera>();
    }
}