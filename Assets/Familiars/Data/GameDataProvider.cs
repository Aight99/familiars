using UnityEngine;

// TODO: Единая точка входа, убрать MonoBehaviour
public class GameDataProvider : MonoBehaviour
{
    [SerializeField]
    private string typeElementResourcePath = "GameData/TypeElement";

    [SerializeField]
    private string moveResourcePath = "GameData/Move";

    [SerializeField]
    private string creatureSpeciesResourcePath = "GameData/CreatureSpecies";

    [SerializeField]
    private string battleTeamsResourcePath = "GameData/BattleTeams";

    private GameDataService service;

    public GameDataService Service => service;

    private void Awake()
    {
        var typeElementJson = LoadTextOrEmpty(typeElementResourcePath);
        var moveJson = LoadTextOrEmpty(moveResourcePath);
        var speciesJson = LoadTextOrEmpty(creatureSpeciesResourcePath);
        var teamsJson = LoadTextOrEmpty(battleTeamsResourcePath);
        service = new GameDataService(typeElementJson, moveJson, speciesJson, teamsJson);
    }

    private static string LoadTextOrEmpty(string resourcePath)
    {
        var asset = Resources.Load<TextAsset>(resourcePath);
        if (asset == null)
        {
            Debug.LogError($"GameDataProvider: Resources.Load failed for '{resourcePath}'.");
            return string.Empty;
        }

        return asset.text ?? string.Empty;
    }
}
