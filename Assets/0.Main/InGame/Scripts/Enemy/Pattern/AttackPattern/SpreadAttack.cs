using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadAttack : ProjectileAttackBase
{
    readonly SpreadAttackConfig mConfig;
    bool mIsFiring;

    public SpreadAttack(SpreadAttackConfig cfg) { mConfig = cfg; }

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
        for (int i = 0; i < mConfig.burstCount; i++)
        {
            bool isLeft = mMonster.Player != null && mMonster.Player.transform.position.x < mMonster.transform.position.x;
            var angles = isLeft ? mConfig.leftAngles : mConfig.rightAngles;

            yield return FireProjectiles
            (
                angles.Length,
                0f,
                idx => SetAngle(Vector2.down, angles[idx]),
                mConfig.projectilePrefab,
                mConfig.projectileSpeed,
                mMonster.Stat.FinalStat.Atk,
                mMonster.PlayerLayerMask
            );

            yield return new WaitForSeconds(mConfig.burstInterval);
        }
        mIsFiring = false;
    }
}
