using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private SceneField overworldScene;

    [SerializeField]
    private SceneField battleScene;

    [SerializeField]
    private SceneField sessionScene;

    // FIXME: Пока не понятно, как это должно работать
    [SerializeField]
    private MonoBehaviour transitionBehaviour;

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

    // FIXME: Обращение по строкам — плохо
    // FIXME: Но возможно этот метод нужно удалить из публичного API
    public void UnloadScene(string sceneName)
    {
        if (!cachedScenes.Remove(sceneName))
            return;

        SceneManager.UnloadSceneAsync(sceneName);
    }

    public void ClearCache()
    {
        foreach (var sceneName in new List<string>(cachedScenes.Keys))
            UnloadScene(sceneName);
    }
}

public partial class SceneLoader
{
    private ISceneTransition Transition => transitionBehaviour as ISceneTransition;

    private IEnumerator LoadSessionSceneCoroutine()
    {
        yield return SceneManager.LoadSceneAsync(sessionScene, LoadSceneMode.Additive);
        cachedScenes[sessionScene.SceneName] = SceneManager.GetSceneByName(sessionScene.SceneName);
    }

    private IEnumerator OpenBattleSceneCoroutine(BattleConfig config)
    {
        if (Transition != null)
            yield return StartCoroutine(Transition.Show());

        if (!cachedScenes.ContainsKey(battleScene.SceneName))
            yield return StartCoroutine(PreloadSceneCoroutine(battleScene));

        var cachedBattleScene = cachedScenes[battleScene.SceneName];
        FindComponentInScene<BattleManager>(cachedBattleScene)?.Initialize(config);

        SetSceneRootsActive(SceneManager.GetSceneByName(overworldScene.SceneName), false);
        SetSceneRootsActive(cachedBattleScene, true);
        SceneManager.SetActiveScene(cachedBattleScene);

        if (Transition != null)
            yield return StartCoroutine(Transition.Hide());
    }

    private IEnumerator ReturnToOverworldCoroutine()
    {
        if (Transition != null)
            yield return StartCoroutine(Transition.Show());

        if (cachedScenes.TryGetValue(battleScene.SceneName, out var cachedBattleScene))
            SetSceneRootsActive(cachedBattleScene, false);

        var overworld = SceneManager.GetSceneByName(overworldScene.SceneName);
        SetSceneRootsActive(overworld, true);
        SceneManager.SetActiveScene(overworld);

        if (Transition != null)
            yield return StartCoroutine(Transition.Hide());
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
