using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SceneLoader
{
    [SerializeField]
    private SceneField sessionScene;

    public void LoadSessionScene()
    {
        if (cachedScenes.ContainsKey(sessionScene.SceneName))
            return;

        StartCoroutine(LoadSessionSceneCoroutine());
    }

    private IEnumerator LoadSessionSceneCoroutine()
    {
        yield return SceneManager.LoadSceneAsync(sessionScene, LoadSceneMode.Additive);
        cachedScenes[sessionScene.SceneName] = SceneManager.GetSceneByName(sessionScene.SceneName);
    }
}
