using System;
using UnityEngine;

[Serializable]
public class HPScaler
{
    [Header("30 스테이지 이전 곱연산")]
    [SerializeField] private float oneTo10Scale = 1.12f;
    [SerializeField] private float tenTo20Scale = 1.12f;
    [SerializeField] private float tewentyTo30Scale = 1.25f;
    
    [Header("30 스테이지 이후 합연산")]
    [SerializeField] private long NormalMonsterAdd  = 200_000L;
    [SerializeField] private long EliteMonsterAdd   = 500_000L;
    [SerializeField] private long BossMonsterAdd    = 1_000_000L;
    
    public long ScaleHP(EnemyRank myRank, long baseHP, int stageIndex)
    {
        long hp = baseHP;

        for (int i = 2; i <= stageIndex; i++)
        {
            if (i <= 10) { hp = (long)(hp * oneTo10Scale); }
            else if (i <= 20) { hp = (long)(hp * tenTo20Scale); }
            else if (i <= 30) { hp = (long)(hp * tewentyTo30Scale); }
            else
            {
                switch (myRank)
                {
                    case EnemyRank.Normal: hp += NormalMonsterAdd; break;
                    case EnemyRank.Elite:  hp += EliteMonsterAdd;  break;
                    case EnemyRank.Boss:   hp += BossMonsterAdd;   break;
                }
            }
        }
        
        return hp;
    }
}