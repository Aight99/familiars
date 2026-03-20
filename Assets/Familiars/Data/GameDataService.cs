using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GameDataService
{
    private readonly Dictionary<string, TypeElement> typeElementsByName;
    private readonly Dictionary<string, Move> movesByName;
    private readonly Dictionary<string, CreatureSpecies> speciesByName;
    private readonly Dictionary<string, BattleTeam> battleTeamsByName;

    public GameDataService(
        string typeElementJson,
        string moveJson,
        string creatureSpeciesJson,
        string battleTeamsJson
    )
    {
        typeElementsByName = new Dictionary<string, TypeElement>(StringComparer.Ordinal);
        movesByName = new Dictionary<string, Move>(StringComparer.Ordinal);
        speciesByName = new Dictionary<string, CreatureSpecies>(StringComparer.Ordinal);
        battleTeamsByName = new Dictionary<string, BattleTeam>(StringComparer.Ordinal);

        var typeDtos =
            Deserialize<List<TypeElementDto>>(typeElementJson) ?? new List<TypeElementDto>();
        var moveDtos = Deserialize<List<MoveDto>>(moveJson) ?? new List<MoveDto>();
        var speciesDtos =
            Deserialize<List<CreatureSpeciesDto>>(creatureSpeciesJson)
            ?? new List<CreatureSpeciesDto>();
        var teamDtos =
            Deserialize<List<BattleTeamDto>>(battleTeamsJson) ?? new List<BattleTeamDto>();

        BuildTypeElements(typeDtos);
        BuildMoves(moveDtos);
        BuildSpecies(speciesDtos);
        BuildBattleTeams(teamDtos);
    }

    public TypeElement GetTypeElement(string name)
    {
        if (string.IsNullOrEmpty(name))
            return default;
        return typeElementsByName.TryGetValue(name, out var element) ? element : default;
    }

    public Move GetMove(string name)
    {
        if (string.IsNullOrEmpty(name))
            return Move.None;
        return movesByName.TryGetValue(name, out var move) ? move : Move.None;
    }

    public CreatureSpecies GetCreatureSpecies(string name)
    {
        if (string.IsNullOrEmpty(name))
            return default;
        return speciesByName.TryGetValue(name, out var species) ? species : default;
    }

    public BattleTeam GetBattleTeam(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;
        return battleTeamsByName.TryGetValue(name, out var team) ? team : null;
    }

    private static T Deserialize<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
            return default;
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception exception)
        {
            Debug.LogError($"GameDataService: failed to parse JSON: {exception.Message}");
            return default;
        }
    }

    private void BuildTypeElements(List<TypeElementDto> dtos)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var dto in dtos)
        {
            var key = dto.name ?? string.Empty;
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("GameDataService: TypeElement entry has empty name.");
                continue;
            }

            if (!seen.Add(key))
                Debug.LogError($"GameDataService: duplicate TypeElement name '{key}'.");

            var effective = dto.effectiveAgainst ?? Array.Empty<string>();
            var ineffective = dto.ineffectiveAgainst ?? Array.Empty<string>();
            var element = new TypeElement(key, effective, ineffective);
            typeElementsByName[key] = element;
        }

        foreach (var pair in typeElementsByName)
        {
            var element = pair.Value;
            foreach (var target in element.EffectiveAgainst)
            {
                if (string.IsNullOrEmpty(target))
                    continue;
                if (!typeElementsByName.ContainsKey(target))
                    Debug.LogError(
                        $"GameDataService: TypeElement '{element.Name}' effectiveAgainst references unknown type '{target}'."
                    );
            }

            foreach (var target in element.IneffectiveAgainst)
            {
                if (string.IsNullOrEmpty(target))
                    continue;
                if (!typeElementsByName.ContainsKey(target))
                    Debug.LogError(
                        $"GameDataService: TypeElement '{element.Name}' ineffectiveAgainst references unknown type '{target}'."
                    );
            }
        }
    }

    private void BuildMoves(List<MoveDto> dtos)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var dto in dtos)
        {
            var key = dto.name ?? string.Empty;
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("GameDataService: Move entry has empty name.");
                continue;
            }

            if (!seen.Add(key))
                Debug.LogError($"GameDataService: duplicate Move name '{key}'.");

            var typeName = dto.type ?? string.Empty;
            if (string.IsNullOrEmpty(typeName) || !typeElementsByName.ContainsKey(typeName))
                Debug.LogError(
                    $"GameDataService: Move '{key}' references unknown type '{typeName}'."
                );

            var typeElement = GetTypeElement(typeName);
            var applicationType = ParseMoveApplicationType(key, dto.applicationType);
            movesByName[key] = new Move(key, dto.power, typeElement, applicationType);
        }
    }

    private void BuildSpecies(List<CreatureSpeciesDto> dtos)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var dto in dtos)
        {
            var key = dto.name ?? string.Empty;
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("GameDataService: CreatureSpecies entry has empty name.");
                continue;
            }

            if (!seen.Add(key))
                Debug.LogError($"GameDataService: duplicate CreatureSpecies name '{key}'.");

            var typeName = dto.type ?? string.Empty;
            if (string.IsNullOrEmpty(typeName) || !typeElementsByName.ContainsKey(typeName))
                Debug.LogError(
                    $"GameDataService: CreatureSpecies '{key}' references unknown type '{typeName}'."
                );

            var typeElement = GetTypeElement(typeName);
            var stats = dto.stats ?? new StatsDto();
            speciesByName[key] = new CreatureSpecies(
                key,
                typeElement,
                stats.health,
                stats.attack,
                stats.speed
            );
        }
    }

    private void BuildBattleTeams(List<BattleTeamDto> dtos)
    {
        var seenTeamNames = new HashSet<string>(StringComparer.Ordinal);
        var seenCreatureIds = new HashSet<string>(StringComparer.Ordinal);

        foreach (var dto in dtos)
        {
            var teamName = dto.name ?? string.Empty;
            if (!seenTeamNames.Add(teamName))
                Debug.LogError($"GameDataService: duplicate BattleTeam name '{teamName}'.");

            var creatures = dto.creatures ?? new List<BattleTeamCreatureDto>();
            var resolved = new List<BattleTeamCreature>(creatures.Count);

            foreach (var creatureDto in creatures)
            {
                var creatureId = creatureDto.creatureId ?? string.Empty;
                if (string.IsNullOrEmpty(creatureId))
                {
                    Debug.LogError(
                        $"GameDataService: BattleTeam '{teamName}' has creature with empty creatureId."
                    );
                }
                else if (!seenCreatureIds.Add(creatureId))
                {
                    Debug.LogError(
                        $"GameDataService: duplicate creatureId '{creatureId}' across BattleTeams."
                    );
                }

                var speciesName = creatureDto.species ?? string.Empty;
                if (string.IsNullOrEmpty(speciesName) || !speciesByName.ContainsKey(speciesName))
                    Debug.LogError(
                        $"GameDataService: BattleTeam '{teamName}', creature '{creatureId}' references unknown species '{speciesName}'."
                    );

                var species = GetCreatureSpecies(speciesName);
                var moveSlots = creatureDto.moves ?? Array.Empty<string>();
                var moves = new Move[4];
                for (var i = 0; i < moves.Length; i++)
                {
                    var moveName = i < moveSlots.Length ? moveSlots[i] : null;
                    if (string.IsNullOrEmpty(moveName))
                    {
                        moves[i] = Move.None;
                        continue;
                    }

                    if (!movesByName.ContainsKey(moveName))
                    {
                        Debug.LogError(
                            $"GameDataService: BattleTeam '{teamName}', creature '{creatureId}' references unknown move '{moveName}'."
                        );
                        moves[i] = Move.None;
                    }
                    else
                    {
                        moves[i] = movesByName[moveName];
                    }
                }

                resolved.Add(new BattleTeamCreature(creatureId, species, moves));
            }

            battleTeamsByName[teamName] = new BattleTeam(teamName, resolved);
        }
    }

    private static MoveApplicationType ParseMoveApplicationType(string moveName, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError($"GameDataService: Move '{moveName}' has empty applicationType.");
            return MoveApplicationType.Status;
        }

        return value switch
        {
            "Physical" => MoveApplicationType.Physical,
            "Ranged" => MoveApplicationType.Ranged,
            "Status" => MoveApplicationType.Status,
            _ => throw new NotImplementedException(),
        };
    }

    private sealed class TypeElementDto
    {
        public string name;
        public string icon;
        public string[] effectiveAgainst;
        public string[] ineffectiveAgainst;
    }

    private sealed class MoveDto
    {
        public string name;
        public int power;
        public string type;
        public string applicationType;
    }

    private sealed class StatsDto
    {
        public int attack;
        public int health;
        public int speed;
    }

    private sealed class CreatureSpeciesDto
    {
        public string icon;
        public string name;
        public string type;
        public StatsDto stats;
    }

    private sealed class BattleTeamCreatureDto
    {
        public string creatureId;
        public string species;
        public string[] moves;
    }

    private sealed class BattleTeamDto
    {
        public string name;
        public List<BattleTeamCreatureDto> creatures;
    }
}
