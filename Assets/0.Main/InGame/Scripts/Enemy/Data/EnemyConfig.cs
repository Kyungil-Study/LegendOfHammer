using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyRank   { Normal = 1, Elite = 2, Boss  = 3 }
public enum EnemyType   { Melee  = 1, Range = 2 }
public enum EnemyAttackType  
{
    Straight = 1,  // 직선형
    Sign     = 2,  // 사인형
    Bomb     = 3,  // 자폭형
    Fast     = 4,  // 고속형
    Shield   = 5,  // 방패형
    Sniper   = 6,  // 저격형
    Spray    = 7,  // 분사형
    Radial   = 8,  // 방사형
    Floating = 9,  // 체공형
}

public class EnemyConfig
{
    // Enemy 행동과 패턴 관련한 필드
    public int         EnemyID;
    public EnemyRank   Rank;
    public EnemyType   Type;
    public EnemyAttackType  AtkType;
    // Enemy 전투 스탯 관련한 필드
    public int         HP;
    public int         AtkPower;
    public float       MoveSpeed;
    public int         ChasingIncrease;
    // Enemy 스폰 관련한 필드
    public int         FirstAppearStage;

    // ID 하나로 Rank/Type/AtkType 채워주는 편의 팩토리
    public static EnemyConfig FromCsvLine(string csvLine)
    {
        var columns = csvLine.Split(',');
        int id = int.Parse(columns[0]);

        var config = new EnemyConfig
        {
            EnemyID          = id,
            Rank             = (EnemyRank)(id / 10000),
            Type             = (EnemyType)((id / 1000) % 10),
            AtkType          = (EnemyAttackType)(id % 1000),
            HP               = int.Parse(columns[4]),
            AtkPower         = int.Parse(columns[5]),
            MoveSpeed        = float.Parse(columns[6]),
            ChasingIncrease  = int.Parse(columns[7]),
            FirstAppearStage = int.Parse(columns[8]),
        };
        return config;
    }
}