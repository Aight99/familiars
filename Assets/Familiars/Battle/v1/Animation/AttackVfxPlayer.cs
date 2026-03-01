using System;
using System.Collections;
using UnityEngine;

public class AttackVfxPlayer : MonoBehaviour
{
    public event Action OnHitMoment;
    public event Action OnAnimationEnd;

    private GameObject activeVfx;

    public void Play(AttackEffectConfig config, Vector3 from, Vector3 to)
    {
        if (activeVfx != null)
            Destroy(activeVfx);

        if (config == null || config.VfxPrefab == null)
        {
            StartCoroutine(FallbackRoutine());
            return;
        }

        activeVfx = Instantiate(config.VfxPrefab, from, Quaternion.identity);
        StartCoroutine(MoveVfxRoutine(from, to));
    }

    private IEnumerator MoveVfxRoutine(Vector3 from, Vector3 to)
    {
        const float duration = 0.4f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (activeVfx != null)
                activeVfx.transform.position = Vector3.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        // Вызывается через Unity Animation Event, но для VFX без клипа — здесь
        OnHitMoment?.Invoke();

        yield return new WaitForSeconds(0.3f);

        if (activeVfx != null)
            Destroy(activeVfx);

        OnAnimationEnd?.Invoke();
    }

    private IEnumerator FallbackRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        OnHitMoment?.Invoke();
        yield return new WaitForSeconds(0.1f);
        OnAnimationEnd?.Invoke();
    }

    // Вызывается через Unity Animation Event в клипе VFX-эффекта
    public void AnimEvent_HitMoment() => OnHitMoment?.Invoke();

    // Вызывается через Unity Animation Event в конце клипа VFX-эффекта
    public void AnimEvent_VfxEnd() => OnAnimationEnd?.Invoke();
}
