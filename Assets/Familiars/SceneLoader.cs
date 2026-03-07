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

    [SerializeField]
    private SceneField battleScene;

    [SerializeField]
    private SceneField sessionScene;

    private readonly Dictionary<string, Scene> cachedScenes = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSessionScene()
    {
        if (cachedScenes.ContainsKey(sessionScene.SceneName))
            return;

        StartCoroutine(LoadSessionSceneCoroutine());
    }

    public void OpenBattleScene(PredefinedCreature playerCreature, PredefinedCreature rivalCreature)
    {
        var config = ScriptableObject.CreateInstance<BattleConfig>();
        config.Setup(playerCreature, rivalCreature);
        StartCoroutine(OpenBattleSceneCoroutine(config));
    }

    public void ReturnToOverworld()
    {
        StartCoroutine(ReturnToOverworldCoroutine());
    }

    public void ClearCache()
    {
        foreach (var scene in cachedScenes.Values)
            UnloadScene(scene);
    }
}

public partial class SceneLoader
{
    private void UnloadScene(Scene scene)
    {
        if (!cachedScenes.Remove(scene.name))
            return;

        SceneManager.UnloadSceneAsync(scene);
    }

    private IEnumerator LoadSessionSceneCoroutine()
    {
        yield return SceneManager.LoadSceneAsync(sessionScene, LoadSceneMode.Additive);
        cachedScenes[sessionScene.SceneName] = SceneManager.GetSceneByName(sessionScene.SceneName);
    }

    private IEnumerator OpenBattleSceneCoroutine(BattleConfig config)
    {
        yield return StartCoroutine(transition.Show());

        if (!cachedScenes.ContainsKey(battleScene.SceneName))
            yield return StartCoroutine(PreloadSceneCoroutine(battleScene));

        var cachedBattleScene = cachedScenes[battleScene.SceneName];
        FindComponentInScene<BattleManager>(cachedBattleScene)?.Initialize(config);

        SetSceneRootsActive(SceneManager.GetSceneByName(overworldScene.SceneName), false);
        SetSceneRootsActive(cachedBattleScene, true);
        SceneManager.SetActiveScene(cachedBattleScene);

        yield return StartCoroutine(transition.Hide());
    }

    private IEnumerator ReturnToOverworldCoroutine()
    {
        yield return StartCoroutine(transition.Show());

        if (cachedScenes.TryGetValue(battleScene.SceneName, out var cachedBattleScene))
            SetSceneRootsActive(cachedBattleScene, false);

        var overworld = SceneManager.GetSceneByName(overworldScene.SceneName);
        SetSceneRootsActive(overworld, true);
        SceneManager.SetActiveScene(overworld);

        yield return StartCoroutine(transition.Hide());
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
