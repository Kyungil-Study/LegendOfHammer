using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAttack : CoroutineAttackBase
{
    readonly ShieldConfig mConfig;

    public ShieldAttack(ShieldConfig cfg) { mConfig = cfg; }

    public override void Tick(float time)
    {
        float rate = 1f;

        Vector2 origin   = (Vector2)mMonster.transform.position + mConfig.pivotOffset;
        float halfCos    = Mathf.Cos((mConfig.angleDeg * 0.5f) * Mathf.Deg2Rad);
        LayerMask combo  = mMonster.PlayerLayerMask | mMonster.ProjectileLayerMask;
        
        var hits = Physics2D.OverlapCircleAll(origin, mConfig.radius, combo);
        foreach (var col in hits)
        {
            Vector2 hitPos  = col.ClosestPoint(origin);
            Vector2 dir     = (hitPos - origin).normalized;
            Vector2 forward = -mMonster.transform.up;

            float dot      = Vector2.Dot(forward, dir);
            float distance = Vector2.Distance(origin, hitPos);

            if (dot >= halfCos && distance <= mConfig.radius)
            {
                rate = mConfig.rate;
                break;
            }
        }

        mMonster.State.ShieldRate = rate;
    }
}
