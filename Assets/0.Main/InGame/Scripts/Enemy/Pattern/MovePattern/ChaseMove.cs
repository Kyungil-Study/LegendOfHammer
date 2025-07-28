using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseMove : IMoveBehaviour
{
    private Monster mMonster;
    private ChaseMoveConfig mConfig;

    public ChaseMove(ChaseMoveConfig cfg) { mConfig = cfg; }

    public void Init(Monster monster) => mMonster = monster;

    public void Tick(float time)
    {
        if (mMonster.State.Detected) return;

        var hits = Physics2D.OverlapCircleAll
            (mMonster.transform.position, mConfig.detectRange, mMonster.PlayerLayerMask);
        
        if (hits.Length > 0)
        {
            mMonster.State.Detected = true;
            return;
        }

        if (mMonster.Player != null)
        {
            mMonster.transform.position = Vector2.MoveTowards
            (
                mMonster.transform.position,
                mMonster.Player.transform.position,
                mMonster.Stat.FinalStat.MoveSpeed * time
            );
        }
        else
        {
            mMonster.transform.position += Vector3.down * (mMonster.Stat.FinalStat.MoveSpeed * time);
        }
    }

    public void OnTriggerEnter2D(Collider2D col) { }
}
