using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SceneManager : SCSingletonSO<SceneManager>
{
    private Dictionary<SceneID, UnityEngine.SceneManagement.Scene> activeScenes;

    public float LoadingProces;

    public event Action<SceneID> OnSceneLoaded;

    public event Action<SceneID> OnSceneUnloaded;

    public List<SceneID> ActiveScenes
    {
        get
        {
            return activeScenes.Keys.ToList();
        }
    }

    public override void OnInstantiated()
    {
        activeScenes = new Dictionary<SceneID, UnityEngine.SceneManagement.Scene>();

        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            activeScenes.Add((SceneID)scene.buildIndex, scene);
        }
    }

    public void LoadSceneAdditive(SceneID scene)
    {
        if (!activeScenes.ContainsKey(scene))
        {
            SingletonManager.Instance.StartCoroutine(LoadSceneAsync(scene, UnityEngine.SceneManagement.LoadSceneMode.Additive));
        }
    }

    public void UnloadScene(SceneID scene)
    {
        if (activeScenes.ContainsKey(scene))
        {
            SingletonManager.Instance.StartCoroutine(UnloadSceneAsync(scene));
        }
    }

    private IEnumerator LoadSceneAsync(SceneID scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)scene, mode);

        while (operation.progress < 1)
        {
            LoadingProces = (operation.progress + .1f) * 100;
            yield return null;
        }

        if (OnSceneLoaded != null)
        {
            OnSceneLoaded.Invoke(scene);
        }

        activeScenes.Add(scene, UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex((int)scene));
    }

    private IEnumerator UnloadSceneAsync(SceneID scene)
    {
        var operation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(activeScenes[scene]);

        while (operation.progress < 1)
        {
            yield return null;
        }

        if (OnSceneUnloaded != null)
        {
            OnSceneUnloaded.Invoke(scene);
        }

        activeScenes.Remove(scene);
    }
}