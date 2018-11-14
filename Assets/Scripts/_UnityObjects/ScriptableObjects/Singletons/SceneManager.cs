using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Placeholder for the SceneManager, will be expanded in the future
/// </summary>
public class SceneManager : SCSingletonSO<SceneManager>
{
    private List<Scenes> activeScenes;

    public override void OnInstantiated()
    {
        activeScenes = new List<Scenes>();
    }

    public void LoadScene(Scenes scene)
    {
        Debug.Log("Loading scene: " + scene);
    }
}