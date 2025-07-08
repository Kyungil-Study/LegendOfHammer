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
    public static EnemyConfig? FromCsvLine(string csvLine)
    {
        var cols = csvLine.Split(',');
        if (int.TryParse(cols[0].Trim(), out var id) == false) return null;

        if (!int.TryParse(cols[4].Trim(), out var hp))           // HP
            return null;
        if (!int.TryParse(cols[5].Trim(), out var atkPower))     // AtkPower
            return null;
        if (!float.TryParse(cols[6].Trim(), out var moveSpeed))  // MoveSpeed
            return null;
        if (!int.TryParse(cols[7].Trim(), out var chaseInc))     // ChasingIncrease
            return null;
        if (!int.TryParse(cols[8].Trim(), out var firstStage))   // FirstAppearStage
            return null;

        // 3) ID 디코딩
        var cfg = new EnemyConfig
        {
            EnemyID          = id,
            Rank             = (EnemyRank)(id / 10000),
            Type             = (EnemyType)((id / 1000) % 10),
            AtkType          = (EnemyAttackType)(id % 1000),
            HP               = hp,
            AtkPower         = atkPower,
            MoveSpeed        = moveSpeed,
            ChasingIncrease  = chaseInc,
            FirstAppearStage = firstStage,
        };
        return cfg;
    }
}