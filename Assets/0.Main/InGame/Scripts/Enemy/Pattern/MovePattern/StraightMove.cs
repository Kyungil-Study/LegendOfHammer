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

    public void Tick(float time)
    {
        mMonster.transform.position += Vector3.down * (mMonster.Stat.FinalStat.MoveSpeed * time);
    }

    public void OnTriggerEnter2D(Collider2D col) { }
}