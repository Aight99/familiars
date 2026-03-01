using System;
using System.Threading.Tasks;
using UnityEngine;

public class BattleAnimationSequencer : MonoBehaviour
{
    [SerializeField]
    private AttackVfxPlayer vfxPlayer;

    public async Task Play(
        MoveAnimationData data,
        CreatureView userView,
        CreatureView targetView,
        Action onHit
    )
    {
        var attackerEnd = new TaskCompletionSource<bool>();
        var vfxMoment = new TaskCompletionSource<bool>();
        var hitEnd = new TaskCompletionSource<bool>();

        void OnAttackerEnd()
        {
            userView.OnAnimationEnd -= OnAttackerEnd;
            attackerEnd.SetResult(true);
            Debug.Log("OnAttackerEnd");
        }
        void OnVfxMoment()
        {
            userView.OnVfxMoment -= OnVfxMoment;
            vfxMoment.SetResult(true);
            Debug.Log("OnVfxMoment");
        }
        void OnHitEnd()
        {
            targetView.OnHitAnimationEnd -= OnHitEnd;
            hitEnd.SetResult(true);
            Debug.Log("OnHitEnd");
        }

        userView.OnAnimationEnd += OnAttackerEnd;
        userView.OnVfxMoment += OnVfxMoment;
        targetView.OnHitAnimationEnd += OnHitEnd;

        userView.PlayAttackAnimation(data.ApplicationType);

        await vfxMoment.Task;

        if (data.AttackEffect != null)
        {
            var vfxEnd = new TaskCompletionSource<bool>();

            void OnVfxEnd()
            {
                vfxPlayer.OnAnimationEnd -= OnVfxEnd;
                vfxEnd.SetResult(true);
            }
            void OnHitMoment()
            {
                vfxPlayer.OnHitMoment -= OnHitMoment;
                onHit();
            }

            vfxPlayer.OnAnimationEnd += OnVfxEnd;
            vfxPlayer.OnHitMoment += OnHitMoment;

            vfxPlayer.Play(
                data.AttackEffect,
                userView.transform.position,
                targetView.transform.position
            );

            await Task.WhenAll(attackerEnd.Task, vfxEnd.Task, hitEnd.Task);
            Debug.Log("Completed!!!!!! AttackEffect != null");
        }
        else
        {
            onHit();
            await Task.WhenAll(attackerEnd.Task, hitEnd.Task);
            Debug.Log("Completed!!!!!! null");
        }
    }
}
