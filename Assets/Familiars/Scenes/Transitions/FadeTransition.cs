using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class FadeTransition : SceneTransition
{
    [SerializeField]
    private float duration = 0.3f;

    private VisualElement overlay;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        overlay = new VisualElement();
        overlay.style.position = Position.Absolute;
        overlay.style.left = 0;
        overlay.style.top = 0;
        overlay.style.right = 0;
        overlay.style.bottom = 0;
        overlay.style.backgroundColor = new StyleColor(Color.black);
        overlay.style.opacity = 0f;

        root.Add(overlay);
    }

    public override IEnumerator Show()
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            overlay.style.opacity = t / duration;
            yield return null;
        }
        overlay.style.opacity = 1f;
    }

    public override IEnumerator Hide()
    {
        for (float t = duration; t > 0; t -= Time.deltaTime)
        {
            overlay.style.opacity = t / duration;
            yield return null;
        }
        overlay.style.opacity = 0f;
    }
}
