using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private SceneTransition transition;

    [SerializeField]
    private SceneField overworldScene;

    private readonly Dictionary<string, Scene> cachedScenes = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ClearCache()
    {
        foreach (var scene in cachedScenes.Values)
            UnloadScene(scene);
    }

    private void UnloadScene(Scene scene)
    {
        if (!cachedScenes.Remove(scene.name))
            return;

        SceneManager.UnloadSceneAsync(scene);
    }

    private IEnumerator PreloadSceneCoroutine(SceneField scene)
    {
        yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        var loadedScene = SceneManager.GetSceneByName(scene.SceneName);
        SetSceneRootsActive(loadedScene, false);
        cachedScenes[scene.SceneName] = loadedScene;
    }

    private void SetSceneRootsActive(Scene scene, bool active)
    {
        if (!scene.IsValid())
            return;

        foreach (var root in scene.GetRootGameObjects())
            root.SetActive(active);
    }

    private T FindComponentInScene<T>(Scene scene)
        where T : Component
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            var component = root.GetComponentInChildren<T>(true);
            if (component != null)
                return component;
        }

        return null;
    }
}
