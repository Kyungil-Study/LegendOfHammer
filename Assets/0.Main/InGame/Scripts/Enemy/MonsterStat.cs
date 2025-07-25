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
    
    public void AddModifier(IDamageModifier mod) => modifiers.Add(mod);
    
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
    
    // 디버프 Monster.cs의 Update()에서 시간 체크
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
    
    // HP 감소 및 죽음 여부 반환
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

// 받는 피해 증가 디버프용
// 디버프 중에는 적용되지 않도록 수정하기
public class DamageAmpModifier : IDamageModifier
{
    private readonly float multipleValue;
    private readonly float endTime;

    public DamageAmpModifier(float value, float duration)
    {
        this.multipleValue = value;
        endTime  = Time.time + duration;
    }
    public float ModifyIncoming(float baseDamage) => baseDamage * multipleValue;
    public bool IsExpired => Time.time >= endTime;
}

// 도트 딜도 필요함.. 최대체력의 1.5%
public class DamageOverTimeModifier : IDamageModifier
{
    private readonly float damagePerSecond;
    private readonly float endTime;
    
    public DamageOverTimeModifier(float value, float duration)
    {
        this.damagePerSecond = value;
        endTime  = Time.time + duration;
    }
    
    public float ModifyIncoming(float baseDamage) => baseDamage + damagePerSecond * Time.deltaTime;
    public bool IsExpired => Time.time >= endTime;
}