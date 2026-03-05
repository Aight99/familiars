using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField]
    private SceneField battleScene;

    private BattleConfig pendingBattleConfig;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadBattleScene(PredefinedCreature playerCreature, PredefinedCreature rivalCreature)
    {
        pendingBattleConfig = ScriptableObject.CreateInstance<BattleConfig>();
        pendingBattleConfig.Setup(playerCreature, rivalCreature);
        UnityEngine.SceneManagement.SceneManager.LoadScene(battleScene);
    }

    public static BattleConfig LoadBattleConfig()
    {
        if (Instance == null)
        {
            return null;
        }

        var config = Instance.pendingBattleConfig;
        Instance.pendingBattleConfig = null;
        return config;
    }
}
