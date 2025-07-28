using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAttack : ProjectileAttackBase
{
    readonly SniperAttackConfig mConfig;
    bool mIsFiring;

    public SniperAttack(SniperAttackConfig cfg) { mConfig = cfg; }

    public override void Start()
    {
        if (mIsFiring)
        {
            return;
        }
        mIsFiring = true;
        StartLoop(Loop());
    }

    IEnumerator Loop()
    {
        while (true)
        {
            yield return FireProjectiles
            (
                mConfig.burstCount,
                mConfig.burstInterval,
                i => (mMonster.Player.transform.position - mMonster.transform.position),
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
