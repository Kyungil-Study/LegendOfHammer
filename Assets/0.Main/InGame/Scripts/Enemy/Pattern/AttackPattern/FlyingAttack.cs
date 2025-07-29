using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAttack : ProjectileAttackBase
{
    private readonly FlyingAttackConfig mConfig;
    private bool started;

    public FlyingAttack(FlyingAttackConfig cfg) { mConfig = cfg; }

    public override void Tick(float dt)
    {
        if (started == false && mMonster.State.StoppedFlying)
        {
            started = true;
            StartLoop(Loop());
        }
    }

    IEnumerator Loop()
    {
        while (true)
        {
            float t = Random.Range(0f, 1f);
            Vector2 target = Vector2.Lerp
            (
                MapManager.Instance.MidLeft.position,
                MapManager.Instance.MidRight.position,
                t
            );
            Vector2 dir = (target - (Vector2)mMonster.transform.position).normalized;

            yield return FireProjectiles(
                1,
                0f,
                _ => dir,
                mConfig.projectilePrefab,
                mConfig.projectileSpeed,
                mMonster.Stat.FinalStat.Atk,
                mMonster.PlayerLayerMask,
                mConfig.isLookingForTarget
            );

            yield return new WaitForSeconds(mConfig.fireInterval);
        }
    }
}
