using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreatureView : MonoBehaviour
{
    private Animator creatureAnimator;
    private Creature creature;

    public event Action OnVfxMoment;
    public event Action OnAnimationEnd;
    public event Action OnHitAnimationEnd;

    private void Awake()
    {
        creatureAnimator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        if (creature != null)
        {
            creature.OnDamaged -= HandleDamaged;
            creature.OnFainted -= HandleFainted;
        }
    }

    public void Init(Creature boundCreature)
    {
        creature = boundCreature;
        creature.OnDamaged += HandleDamaged;
        creature.OnFainted += HandleFainted;
    }

    public void PlayAttackAnimation(MoveApplicationType applicationType)
    {
        if (creatureAnimator != null)
        {
            creatureAnimator.SetTrigger(GetAttackTrigger(applicationType));
        }
        else
        {
            StartCoroutine(FallbackAttackRoutine());
        }
    }

    private void HandleDamaged()
    {
        if (creature.IsFainted)
            return;

        if (creatureAnimator != null)
        {
            creatureAnimator.SetTrigger(AnimatonTriggers.TakeHit);
        }
        else
        {
            StartCoroutine(FallbackHitRoutine());
        }
    }

    private void HandleFainted()
    {
        OnHitAnimationEnd?.Invoke();

        if (creatureAnimator != null)
        {
            creatureAnimator.SetTrigger(AnimatonTriggers.Faint);
        }
        else
        {
            StartCoroutine(FallbackFaintRoutine());
        }
    }

    private static int GetAttackTrigger(MoveApplicationType applicationType)
    {
        return applicationType switch
        {
            MoveApplicationType.Physical => AnimatonTriggers.AttackPhysical,
            MoveApplicationType.Ranged => AnimatonTriggers.AttackRanged,
            MoveApplicationType.Status => AnimatonTriggers.AttackStatus,
            _ => throw new NotImplementedException(),
        };
    }

    private IEnumerator FallbackAttackRoutine()
    {
        yield return new WaitForSeconds(0.4f);
        OnVfxMoment?.Invoke();
        yield return new WaitForSeconds(0.4f);
        OnAnimationEnd?.Invoke();
    }

    private IEnumerator FallbackHitRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        OnHitAnimationEnd?.Invoke();
    }

    private IEnumerator FallbackFaintRoutine()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void AnimEvent_VfxMoment()
    {
        OnVfxMoment?.Invoke();
    }

    public void AnimEvent_AttackEnd()
    {
        OnAnimationEnd?.Invoke();
    }

    public void AnimEvent_HitEnd()
    {
        OnHitAnimationEnd?.Invoke();
    }
}
