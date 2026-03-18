using System;

[Serializable]
public class TypeElementEntry
{
    public string name = "";
    public string icon = "";
    public string[] effectiveAgainst = Array.Empty<string>();
    public string[] ineffectiveAgainst = Array.Empty<string>();
}
