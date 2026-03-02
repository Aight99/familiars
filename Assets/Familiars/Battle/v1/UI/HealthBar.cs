using UnityEngine;
using UnityEngine.UIElements;

public enum HealthBarAnchor
{
    TopRight,
    BottomLeft,
}

public class HealthBar
{
    private readonly VisualElement barFill;
    private readonly Label nameLabel;
    private readonly Label healthText;

    private static readonly Color ColorGreen = new Color(0.35f, 0.82f, 0.27f);
    private static readonly Color ColorYellow = new Color(1f, 0.84f, 0.18f);
    private static readonly Color ColorRed = new Color(0.88f, 0.22f, 0.18f);

    public HealthBar(
        UIDocument uiDocument,
        StyleSheet styleSheet,
        HealthBarAnchor anchor,
        bool showHealthText
    )
    {
        var root = uiDocument.rootVisualElement;

        if (!root.styleSheets.Contains(styleSheet))
            root.styleSheets.Add(styleSheet);

        var container = new VisualElement();
        container.AddToClassList("health-bar");

        if (anchor == HealthBarAnchor.TopRight)
            container.AddToClassList("health-bar--top-right");
        else
            container.AddToClassList("health-bar--bottom-left");

        nameLabel = new Label();
        nameLabel.AddToClassList("health-bar__name");
        container.Add(nameLabel);

        var barBackground = new VisualElement();
        barBackground.AddToClassList("health-bar__background");

        barFill = new VisualElement();
        barFill.AddToClassList("health-bar__fill");
        barBackground.Add(barFill);

        container.Add(barBackground);

        if (showHealthText)
        {
            healthText = new Label();
            healthText.AddToClassList("health-bar__text");
            container.Add(healthText);
        }

        root.Add(container);
    }

    public void SetCreatureName(string creatureName) => nameLabel.text = creatureName;

    public void SetHealth(int current, int max)
    {
        float percent = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;

        barFill.style.width = new StyleLength(new Length(percent * 100f, LengthUnit.Percent));
        barFill.style.backgroundColor = GetBarColor(percent);

        if (healthText != null)
            healthText.text = $"{current} / {max}";
    }

    private static Color GetBarColor(float percent)
    {
        if (percent > 0.5f)
            return ColorGreen;
        if (percent > 0.2f)
            return ColorYellow;
        return ColorRed;
    }
}
