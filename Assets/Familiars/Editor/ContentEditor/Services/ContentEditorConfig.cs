public static class ContentEditorConfig
{
    private const string GameDataFolderPath = "Assets/Familiars/GameData";
    private const string JsonAssetsFolderPath = "Assets/Familiars/Resources/GameData";
    private const string IconsFolderPath = GameDataFolderPath + "/Icons";

    public static string JsonFolderPath => JsonAssetsFolderPath;
    public static string TypeIconsFolderPath => IconsFolderPath + "/Types";
    public static string SpeciesIconsFolderPath => IconsFolderPath + "/Species";
    public static string MoveApplicationTypeIconsFolderPath =>
        IconsFolderPath + "/MoveApplicationType";
    public static string TypeElementFileName => "TypeElement.json";
    public static string CreatureSpeciesFileName => "CreatureSpecies.json";
    public static string MoveFileName => "Move.json";
    public static string BattleTeamsFileName => "BattleTeams.json";
    public static string LegacyBattleTeamsFileName => "PredefinedBattle.json";
    public static string LegacyBattleTeamCreaturesFileName => "PredefinedCreature.json";
}
