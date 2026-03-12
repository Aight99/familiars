using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct BattleSceneHandler
{
    public Action OnBattleEnd;
}

public partial class SceneLoader
{
    [SerializeField]
    private SceneField battleScene;

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

    private IEnumerator OpenBattleSceneCoroutine(BattleConfig config)
    {
        yield return StartCoroutine(transition.Show());

        if (!cachedScenes.ContainsKey(battleScene.SceneName))
            yield return StartCoroutine(PreloadSceneCoroutine(battleScene));

        var cachedBattleScene = cachedScenes[battleScene.SceneName];
        var handler = new BattleSceneHandler { OnBattleEnd = ReturnToOverworld };
        FindComponentInScene<BattleManager>(cachedBattleScene)?.Initialize(config, handler);

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
}
