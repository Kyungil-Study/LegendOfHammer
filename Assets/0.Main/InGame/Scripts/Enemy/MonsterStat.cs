using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// StatComponent 책임: “스탯 로딩/오버라이드/디버프 적용/HP관리”
// 받는 피해 증가 디버프: IDamageModifier로 구현, 리스트에서 처리

public class MonsterStat : MonoBehaviour
{
    [Header("몬스터 기본 스탯")] [Tooltip("기본 스탯 수정용")]
    public StatField<int>   HP;
    public StatField<int>   Atk;
    public StatField<float> MoveSpeed;
    
    public StatBlock FinalStat { get; private set; }
    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }
    
    readonly List<IDamageModifier> modifiers = new();
    
    public bool HasModifier<T>() where T : IDamageModifier
    {
        return modifiers.Any(m => m is T);
    }
    
    public void AddModifier(IDamageModifier newModifier)
    {
        if (newModifier is DamageAmpModifier newAmp)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier is DamageAmpModifier existingAmp && Mathf.Approximately(existingAmp.Value, newAmp.Value))
                {
                    existingAmp.ExtendDuration(newAmp.IsExpired ? 0 : (newAmp.endTime - Time.time)); // 시간 연장
                    return;
                }
            }
        }

        modifiers.Add(newModifier);
    }
    
    public IEnumerable<T> GetModifiersOfType<T>() where T : IDamageModifier
    {
        return modifiers.OfType<T>();
    }
    
    /// <summary> TSV값 불러오기 + 변경하기 (스테이지 스케일링은 나중에 필요 시 내부에서 처리)</summary>
    public void Initialize(EnemyData data, int stageIndex)
    {
        var baseStat = new StatBlock
        {
            HP        = data.HP,
            Atk       = data.Atk_Power,
            MoveSpeed = data.Move_Speed
        };

        FinalStat = new StatBlock
        {
            HP        = HP.Apply(baseStat.HP),
            Atk       = Atk.Apply(baseStat.Atk),
            MoveSpeed = MoveSpeed.Apply(baseStat.MoveSpeed)
        };

        MaxHP = FinalStat.HP;
        CurrentHP = MaxHP;
    }
    
    public int ApplyIncomingDamage(int dmg)
    {
        float damage = dmg;
        
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            damage = modifiers[i].ModifyIncoming(damage);
        }
        return Mathf.RoundToInt(damage);
    }
    
    public void Tick(float time)
    {
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            if (modifiers[i].IsExpired)
            {
                modifiers.RemoveAt(i);
            }
        }
    }
    
    public bool ReduceHP(int amount)
    {
        CurrentHP -= amount;
        return CurrentHP <= 0;
    }
}

[Serializable]
public struct StatBlock
{
    public int HP;
    public int Atk;
    public float MoveSpeed;
}

[Serializable]
public struct StatField<T>
{
    public bool Modify;
    public T Value;
    
    public T Apply(T baseValue) => Modify? Value : baseValue;
}

public interface IDamageModifier
{
    float ModifyIncoming(float baseDamage); 
    bool IsExpired{ get; }
}

public class DamageAmpModifier : IDamageModifier
{
    private readonly float multipleValue;
    public float endTime;

    public DamageAmpModifier(float value, float duration)
    {
        this.multipleValue = value;
        endTime = Time.time + duration;
    }

    public bool IsExpired => Time.time >= endTime;
    public float ModifyIncoming(float baseDamage) => baseDamage * multipleValue;
    public void ExtendDuration(float additionalTime)
    {
        endTime = Mathf.Max(endTime, Time.time + additionalTime);
    }

    // 같은 종류의 디버프인지 확인용
    public float Value => multipleValue;
}

public class DamageOverTimeModifier : IDamageModifier
{
    private readonly float damagePerSecond; // 초당 피해량
    private readonly float endTime;         // 만료 시각
    private float accumulator;              // 누적 잔여 피해

    public DamageOverTimeModifier(float dps, float duration)
    {
        damagePerSecond = dps;
        endTime         = Time.time + duration;
        accumulator     = 0f;
    }

    public bool IsExpired => Time.time >= endTime;
    public float ModifyIncoming(float baseDamage) => baseDamage;
    public int DamageTick(float deltaTime)
    {
        accumulator += damagePerSecond * deltaTime;
        int toDeal = Mathf.FloorToInt(accumulator);
        if (toDeal > 0) { accumulator -= toDeal; }
        return toDeal;
    }
}
