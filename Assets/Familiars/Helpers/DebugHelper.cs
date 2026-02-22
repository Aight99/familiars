using UnityEngine;

public static class DebugHelper
{
    public enum MessageType
    {
        Yippee,
        Fiasco,
        Other,
    }

    private static readonly Color Purple = new(0.47f, 0.34f, 0.94f);
    private static readonly Color Yellow = new(0.98f, 0.84f, 0.34f);
    private static readonly Color Red = new(1f, 0.34f, 0.34f);
    private static readonly Color Blue = new(0.34f, 0.6f, 1f);
    private static readonly Color Green = new(0.34f, 1f, 0.34f);

    public static void Log(MessageType type, string message)
    {
        string prefix;
        string colorHex;

        switch (type)
        {
            case MessageType.Yippee:
                prefix = "ЯПИ!";
                colorHex = ColorUtility.ToHtmlStringRGB(Yellow);
                break;
            case MessageType.Fiasco:
                prefix = "ФИАСКО";
                colorHex = ColorUtility.ToHtmlStringRGB(Blue);
                break;
            case MessageType.Other:
                prefix = "";
                colorHex = ColorUtility.ToHtmlStringRGB(Green);
                break;
            default:
                prefix = "";
                colorHex = ColorUtility.ToHtmlStringRGB(Color.white);
                break;
        }

        Debug.Log($"<color=#{colorHex}>{prefix}</color>: {message}");
    }
}
