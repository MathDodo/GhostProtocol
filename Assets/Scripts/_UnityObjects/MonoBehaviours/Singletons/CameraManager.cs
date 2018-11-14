using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : RSingletonMB<CameraManager>
{
    private Camera cam;

    public Camera Cam { get { return cam; } }

    public override void OnInstantiated()
    {
        cam = GetComponent<Camera>();
    }
}