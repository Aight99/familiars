using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    // TODO: Заменить на прямой референс к сцене
    [SerializeField]
    private string battleSceneName;

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
        UnityEngine.SceneManagement.SceneManager.LoadScene(battleSceneName);
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
