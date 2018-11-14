using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    private bool doSpawn = false;

    private void Start()
    {
        Debug.Log(CameraManager.Instance.Cam);

        SaveManager.Instance.StartSave();
    }

    private void Update()
    {
        if (doSpawn)
        {
            SceneManager.Instance.LoadSceneAdditive(SceneID.TestScene);
            doSpawn = false;
        }
    }
}