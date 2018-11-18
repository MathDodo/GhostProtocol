using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Placeholder for the SceneManager, will be expanded in the future
/// </summary>
public class SceneManager : SCSingletonSO<SceneManager>
{
    //The currently active scenes
    private Dictionary<SceneID, UnityEngine.SceneManagement.Scene> _ActiveScenes;

    //Acces to the loading process of a scene
    public float _LoadingProcess;

    //Invoked when any scene is loaded
    public event Action<SceneID> _OnAnySceneLoaded;

    //Invoked when any scene is unloaded
    public event Action<SceneID> _OnAnySceneUnloaded;

    //Invoked before any scene is loaded
    public event Action<SceneID> _BeforeAnySceneLoaded;

    //Invoked before any scene is unloaded
    public event Action<SceneID> _BeforeAnySceneUnloaded;

    /// <summary>
    /// Getting the ids of the active scenes
    /// </summary>
    public List<SceneID> GetActiveScenes { get { return _ActiveScenes.Keys.ToList(); } }

    /// <summary>
    /// Giving the activescenes a value so it can be filled, also adding the current active scenes
    /// </summary>
    public override void OnInstantiated()
    {
        _ActiveScenes = new Dictionary<SceneID, UnityEngine.SceneManagement.Scene>();

        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

            _ActiveScenes.Add((SceneID)scene.buildIndex, scene);
        }
    }

    /// <summary>
    /// Load a scene with the single mode, will unload all other scenes automatically (Except DontDestroyOnLoad)
    /// </summary>
    /// <param name="scene"></param>
    public void LoadSceneSingle(SceneID scene)
    {
        if (!_ActiveScenes.ContainsKey(scene))
        {
            if (_BeforeAnySceneLoaded != null)
            {
                _BeforeAnySceneLoaded.Invoke(scene);
            }

            if (_BeforeAnySceneUnloaded != null)
            {
                foreach (var pair in _ActiveScenes)
                {
                    _BeforeAnySceneUnloaded.Invoke(pair.Key);
                }
            }

            SingletonManager.Instance.StartCoroutine(LoadSceneAsync(scene, UnityEngine.SceneManagement.LoadSceneMode.Single));
        }
    }

    /// <summary>
    /// Load a new scene without unloading other scenes, only if isn't already active will it be loaded
    /// </summary>
    /// <param name="scene">The id of the scene to be loaded</param>
    public void LoadSceneAdditive(SceneID scene)
    {
        if (!_ActiveScenes.ContainsKey(scene))
        {
            if (_BeforeAnySceneLoaded != null)
            {
                _BeforeAnySceneLoaded.Invoke(scene);
            }

            SingletonManager.Instance.StartCoroutine(LoadSceneAsync(scene, UnityEngine.SceneManagement.LoadSceneMode.Additive));
        }
    }

    /// <summary>
    /// Unload an active scene, will only work when multiple scenes are active
    /// </summary>
    /// <param name="scene">The scene to be unloaded</param>
    public void UnloadScene(SceneID scene)
    {
        if (_ActiveScenes.ContainsKey(scene))
        {
            if (_BeforeAnySceneUnloaded != null)
            {
                _BeforeAnySceneUnloaded.Invoke(scene);
            }

            SingletonManager.Instance.StartCoroutine(UnloadSceneAsync(scene));
        }
    }

    /// <summary>
    /// The coroutine for loading a scene async
    /// </summary>
    /// <param name="id">The id of the scene</param>
    /// <param name="loadSceneMode">How the scene should be loaded</param>
    /// <returns></returns>
    private IEnumerator LoadSceneAsync(SceneID id, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
    {
        var opreation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)id, loadSceneMode);

        while (opreation.progress < 1)
        {
            _LoadingProcess = opreation.progress + .1f * 100;

            yield return null;
        }

        if (_OnAnySceneLoaded != null)
        {
            _OnAnySceneLoaded.Invoke(id);
        }

        if (loadSceneMode == UnityEngine.SceneManagement.LoadSceneMode.Single && _OnAnySceneUnloaded != null)
        {
            foreach (var pair in _ActiveScenes)
            {
                _OnAnySceneUnloaded.Invoke(pair.Key);
            }

            _ActiveScenes.Clear();
        }

        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).buildIndex == (int)id)
            {
                _ActiveScenes.Add(id, UnityEngine.SceneManagement.SceneManager.GetSceneAt(i));
            }
        }
    }

    /// <summary>
    /// Coroutine for unloading a scene async
    /// </summary>
    /// <param name="id">The id of the scene</param>
    /// <returns></returns>
    private IEnumerator UnloadSceneAsync(SceneID id)
    {
        var operation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync((int)id);

        while (operation.progress < 1)
        {
            yield return null;
        }

        _ActiveScenes.Remove(id);

        if (_OnAnySceneUnloaded != null)
        {
            _OnAnySceneUnloaded.Invoke(id);
        }
    }
}