using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonDataService
{
    public static List<T> Load<T>(string fileName)
    {
        var path = Path.Combine(ContentEditorConfig.JsonFolderPath, fileName);
        if (!File.Exists(path))
        {
            Debug.Log($"JsonDataService: file not found at {path}, returning empty list.");
            return new List<T>();
        }

        try
        {
            var json = File.ReadAllText(path);
            var result = JsonConvert.DeserializeObject<List<T>>(json);
            Debug.Log($"JsonDataService: loaded {result?.Count ?? 0} entries from {path}");
            return result ?? new List<T>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JsonDataService: failed to load {path}: {e.Message}");
            return new List<T>();
        }
    }

    public static void Save<T>(string fileName, List<T> data)
    {
        var path = Path.Combine(ContentEditorConfig.JsonFolderPath, fileName);
        try
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
            Debug.Log($"JsonDataService: exported {data.Count} entries to {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JsonDataService: failed to save {path}: {e.Message}");
        }
    }
}
