using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// StatComponent 책임: “스탯 로딩/오버라이드/디버프 적용/HP관리”
// 받는 피해 증가 디버프: IDamageModifier로 구현, 리스트에서 처리

public class MonsterStatComponent : MonoBehaviour
{
    [SerializeField] private EnemyID enemyID;

    [Header("몬스터 기본 스탯")] [Tooltip("기본 스탯 수정용")]
    public StatField<int>   HP;
    public StatField<int>   Atk;
    public StatField<float> MoveSpeed;

    [HideInInspector] public StatBlock FinalStat { get; private set; }
    [HideInInspector] public EnemyID   EnemyID   => enemyID;

    // (선택) 현재 HP를 여기서 관리한다면:
    public int CurrentHP { get; private set; }
    
    readonly List<IDamageModifier> modifiers = new();

    /// <summary>TSV값을 읽어와 최종값을 만든다. (스테이지 스케일링은 나중에 필요 시 내부에서 처리)</summary>
    public void Initialize(int stageIndex)
    {
        var data = EnemyDataManager.Instance.Records[enemyID];

        var baseStat = new StatBlock
        {
            HP        = data.HP,
            Atk       = data.Atk_Power,
            MoveSpeed = data.Move_Speed
        };

        // (선택) 스테이지 HP 보정이 필요해지면 여기서 직접 처리
        // baseStat.HP = Mathf.RoundToInt(baseStat.HP * GetStageHpMultiplier(stageIndex));

        FinalStat = new StatBlock
        {
            HP        = HP.Apply(baseStat.HP),
            Atk       = Atk.Apply(baseStat.Atk),
            MoveSpeed = MoveSpeed.Apply(baseStat.MoveSpeed)
        };
        
        CurrentHP = FinalStat.HP; // 여기서 HP도 세팅
    }

    public void AddModifier(IDamageModifier mod) => modifiers.Add(mod);

    public int ApplyIncomingDamage(int dmg)
    {
        float damage = dmg;
        
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            damage = modifiers[i].ModifyIncoming(damage);
            
            if (modifiers[i].IsExpired)
            {
                modifiers.RemoveAt(i);
            }
        }
        return Mathf.RoundToInt(damage);
    }
    
    /// <summary>HP 감소 및 죽음 여부 반환</summary>
    public bool ReduceHP(int amount)
    {
        CurrentHP -= amount;
        return CurrentHP <= 0;
    }
}

[Serializable]
public struct StatField<T>
{
    public bool Modify;
    public T Value;
    
    public T Apply(T baseValue) => Modify? Value : baseValue;
}

[Serializable]
public struct StatBlock
{
    public int HP;
    public int Atk;
    public float MoveSpeed;
}

public interface IDamageModifier
{
    float ModifyIncoming(float baseDamage); 
    bool IsExpired{ get; }
}

// 받는 피해 증가 디버프용
public class DamageAmpModifier : IDamageModifier
{
    private readonly float mul;
    private readonly float endTime;
    
    public DamageAmpModifier(float mul, float duration) // 언제까지 ?
    {
        this.mul = mul;
        endTime  = Time.time + duration;
    }
    public float ModifyIncoming(float baseDamage) => baseDamage * mul;
    public bool IsExpired => Time.time >= endTime;
}