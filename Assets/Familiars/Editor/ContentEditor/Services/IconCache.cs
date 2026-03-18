using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IconCache
{
    private readonly Dictionary<string, Texture2D> cache = new();
    private static readonly string[] extensions = { ".png", ".jpg", ".tga", ".jpeg" };
    private Texture2D placeholder;

    public Texture2D GetIcon(string iconName)
    {
        if (string.IsNullOrEmpty(iconName))
            return GetPlaceholder();

        if (cache.TryGetValue(iconName, out var cached))
            return cached;

        var texture = TryLoadTexture(iconName);
        cache[iconName] = texture ?? GetPlaceholder();
        return cache[iconName];
    }

    public void Clear()
    {
        cache.Clear();
        placeholder = null;
    }

    private Texture2D TryLoadTexture(string iconName)
    {
        foreach (var ext in extensions)
        {
            var hasExtension = System.IO.Path.HasExtension(iconName);
            var fileName = hasExtension ? iconName : iconName + ext;
            var path = $"{ContentEditorConfig.IconsFolderPath}/{fileName}";
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture != null)
                return texture;

            if (hasExtension)
                break;
        }
        return null;
    }

    private Texture2D GetPlaceholder()
    {
        if (placeholder != null)
            return placeholder;

        placeholder = new Texture2D(32, 32);
        var pixels = new Color[32 * 32];
        for (var i = 0; i < pixels.Length; i++)
            pixels[i] = Color.red;
        placeholder.SetPixels(pixels);
        placeholder.Apply();
        return placeholder;
    }
}
