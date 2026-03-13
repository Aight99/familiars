using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct BattleSceneHandler
{
    public Action<BattleResult> OnBattleEnd;
}

public partial class SceneLoader
{
    [SerializeField]
    private SceneField battleScene;

    public void OpenBattleScene(
        PredefinedCreature playerCreature,
        PredefinedCreature rivalCreature,
        Action<BattleResult> onBattleEnd
    )
    {
        var config = ScriptableObject.CreateInstance<BattleConfig>();
        config.Setup(playerCreature, rivalCreature);
        StartCoroutine(OpenBattleSceneCoroutine(config, onBattleEnd));
    }

    public void ReturnToOverworld(BattleResult result, Action<BattleResult> onBattleEnd)
    {
        StartCoroutine(ReturnToOverworldCoroutine(result, onBattleEnd));
    }

    private IEnumerator OpenBattleSceneCoroutine(
        BattleConfig config,
        Action<BattleResult> onBattleEnd
    )
    {
        yield return StartCoroutine(transition.Show());

        if (!cachedScenes.ContainsKey(battleScene.SceneName))
            yield return StartCoroutine(PreloadSceneCoroutine(battleScene));

        var cachedBattleScene = cachedScenes[battleScene.SceneName];
        var handler = new BattleSceneHandler
        {
            OnBattleEnd = result => ReturnToOverworld(result, onBattleEnd),
        };
        FindComponentInScene<BattleManager>(cachedBattleScene)?.Initialize(config, handler);

        SetSceneRootsActive(SceneManager.GetSceneByName(overworldScene.SceneName), false);
        SetSceneRootsActive(cachedBattleScene, true);
        SceneManager.SetActiveScene(cachedBattleScene);

        yield return StartCoroutine(transition.Hide());
    }

    private IEnumerator ReturnToOverworldCoroutine(
        BattleResult result,
        Action<BattleResult> onBattleEnd
    )
    {
        yield return StartCoroutine(transition.Show());

        if (cachedScenes.TryGetValue(battleScene.SceneName, out var cachedBattleScene))
            SetSceneRootsActive(cachedBattleScene, false);

        var overworld = SceneManager.GetSceneByName(overworldScene.SceneName);
        SetSceneRootsActive(overworld, true);
        SceneManager.SetActiveScene(overworld);

        if (cachedScenes.ContainsKey(battleScene.SceneName))
            yield return SceneManager.UnloadSceneAsync(cachedBattleScene);

        cachedScenes.Remove(battleScene.SceneName);

        yield return StartCoroutine(transition.Hide());

        onBattleEnd?.Invoke(result);
    }
}
