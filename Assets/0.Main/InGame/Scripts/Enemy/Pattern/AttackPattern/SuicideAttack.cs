using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideAttack : CoroutineAttackBase
{
    readonly SuicideAttackConfig mConfig;
    bool started;

    public SuicideAttack(SuicideAttackConfig cfg) { mConfig = cfg; }

    public override void Tick(float time)
    {
        if (started == false && mMonster.State.Detected)
        {
            started = true;
            StartLoop(SuicideRoutine());
        }
    }

    IEnumerator SuicideRoutine()
    {
        mMonster.Scale?.EnterSuicideMode(mConfig.warningImage);
        mMonster.Scale?.StartSuicideFlash(mConfig.delay, mConfig.exlposionInverval);

        yield return new WaitForSeconds(mConfig.delay);

        var obj = Object.Instantiate(mConfig.explosionPrefab, mMonster.transform.position, Quaternion.identity);
        var explosion = obj.GetComponent<EnemyExplosion>();
        explosion.Initialize(mMonster, mMonster.Stat.FinalStat.Atk, mConfig.attackRange, mMonster.PlayerLayerMask);
        
        Object.Destroy(mMonster.gameObject);
    }
}