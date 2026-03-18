using System;
using System.Collections.Generic;

public static class SelectorPopupUtils
{
    public static int FindIndex(string[] options, string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;
        for (var i = 0; i < options.Length; i++)
        {
            if (options[i] == value)
                return i;
        }
        return 0;
    }

    public static string[] BuildNames<T>(List<T> items, Func<T, string> getName)
    {
        if (items == null || items.Count == 0)
            return Array.Empty<string>();
        var names = new string[items.Count];
        for (var i = 0; i < items.Count; i++)
            names[i] = getName(items[i]);
        return names;
    }
}
