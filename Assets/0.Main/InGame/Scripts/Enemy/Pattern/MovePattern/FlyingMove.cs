using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMove : IMoveBehaviour
{
    private Monster mMonster;
    private FlyingMoveConfig mConfig;
    private float startY;

    public FlyingMove(FlyingMoveConfig cfg) { mConfig = cfg; }

    public void Init(Monster monster)
    {
        mMonster = monster;
        startY = mMonster.transform.position.y;
    }

    public void Tick(float dt)
    {
        if (mMonster.State.StoppedFlying) return;

        float traveled = startY - mMonster.transform.position.y;
        if (traveled < mConfig.distanceToStop)
        {
            mMonster.transform.position += Vector3.down * (mMonster.Stat.FinalStat.MoveSpeed * dt);
        }
        else
        {
            mMonster.State.StoppedFlying = true;
        }
    }

    public void OnTriggerEnter2D(Collider2D col) { }
}