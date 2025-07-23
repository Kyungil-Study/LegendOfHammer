using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMove : IMoveBehaviour
{
    private Monster mMonster;

    public void Init(Monster monster)
    {
        mMonster = monster;
    }

    public void Tick(float dt)
    {
        mMonster.transform.position += Vector3.down * (mMonster.Stat.FinalStat.MoveSpeed * dt);
    }

    public void OnTriggerEnter2D(Collider2D col) { }
}