using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private const string TypeElementResourcePath = "GameData/TypeElement";
    private const string MoveResourcePath = "GameData/Move";
    private const string CreatureSpeciesResourcePath = "GameData/CreatureSpecies";
    private const string BattleTeamsResourcePath = "GameData/BattleTeams";

    [SerializeField]
    private SceneLoader sceneLoader;

    [SerializeField]
    private OverworldManager overworldManager;

    [SerializeField]
    private CreaturePrefabRegistry creaturePrefabRegistry;

    private GameDataService gameDataService;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (creaturePrefabRegistry != null && creaturePrefabRegistry.gameObject != gameObject)
            DontDestroyOnLoad(creaturePrefabRegistry.gameObject);
        var typeElementJson = LoadTextOrEmpty(TypeElementResourcePath);
        var moveJson = LoadTextOrEmpty(MoveResourcePath);
        var speciesJson = LoadTextOrEmpty(CreatureSpeciesResourcePath);
        var teamsJson = LoadTextOrEmpty(BattleTeamsResourcePath);
        gameDataService = new GameDataService(typeElementJson, moveJson, speciesJson, teamsJson);
        creaturePrefabRegistry?.Initialize(gameDataService);
    }

    private void Start()
    {
        overworldManager.Initialize(gameDataService);
        sceneLoader.Initialize(gameDataService, creaturePrefabRegistry);
    }

    private static string LoadTextOrEmpty(string resourcePath)
    {
        var asset = Resources.Load<TextAsset>(resourcePath);
        if (asset == null)
        {
            Debug.LogError($"GameRoot: Resources.Load failed for '{resourcePath}'.");
            return string.Empty;
        }

        return asset.text ?? string.Empty;
    }
}
