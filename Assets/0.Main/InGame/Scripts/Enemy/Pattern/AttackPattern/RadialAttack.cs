using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialAttack : ProjectileAttackBase
{
    private readonly RadialAttackConfig mConfig;
    private bool mIsFiring;

    public RadialAttack(RadialAttackConfig cfg) { mConfig = cfg; }

    public override void Start()
    {
        if (mIsFiring) return;
        mIsFiring = true;
        StartLoop(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return FireProjectiles
            (
                mConfig.projectileCount,
                0f,
                i => SetAngle(Vector2.down, i * mConfig.betweenAngle),
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
