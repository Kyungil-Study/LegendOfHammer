using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

// StatComponent 책임: “스탯 로딩/오버라이드/디버프 적용/HP관리”
// 받는 피해 증가 디버프: IDamageModifier로 구현, 리스트에서 처리

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

public class MonsterStat : MonoBehaviour
{
    [Header("몬스터 기본 스탯")] [Tooltip("기본 스탯 수정용")]
    public StatField<long>  HP;
    public StatField<int>   Atk;
    public StatField<float> MoveSpeed;
    
    [Header("몬스터 스탯 스케일링")] [Tooltip("스테이지에 따라 HP를 스케일링")]
    [SerializeField] private HPScaler hpScaler = new HPScaler();
    
    readonly List<IDamageModifier> modifiers = new();
    private Monster monster;
    
    public StatBlock FinalStat { get; private set; }
    public long CurrentHP { get; private set; }
    public long MaxHP { get; private set; }
    
    public bool HasModifier<T>() where T : IDamageModifier
    {
        return modifiers.Any(modifier => modifier is T);
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
        
        // ▶ 중복 DoT 방지 로직 추가
        if (newModifier is DamageOverTimeModifier newDot)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier is DamageOverTimeModifier existingDot 
                    // 같은 DPS(초당 피해량)인지 확인
                    && Mathf.Approximately(
                        existingDot.DamagePerSecond, 
                        newDot.DamagePerSecond
                    ))
                {
                    // 동일 DoT이면 지속시간 연장만
                    existingDot.ExtendDuration(
                        newDot.RemainingDuration    // 새로 들어온 duration
                    );
                    return;
                }
            }
        }
        
        modifiers.Add(newModifier);
    }
    
    public void Initialize(EnemyData data, int stageIndex, Monster monster)
    {
        this.monster = monster;
        
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

        var finalStat = FinalStat;
        finalStat.HP = hpScaler.ScaleHP(data.Enemy_Rank, baseStat.HP, stageIndex);
        FinalStat = finalStat;
        
        MaxHP = FinalStat.HP;
        CurrentHP = MaxHP;
    }
    
    private void Update()
    {
        Tick(time: Time.deltaTime);
        
        foreach (var dot in modifiers.OfType<DamageOverTimeModifier>())
        {
            int damageTick = dot.DamageTick(Time.deltaTime);
            
            if (damageTick > 0)
            {
                monster.TakeDamage(new TakeDamageEventArgs(monster, monster, DamageType.DoT, damageTick));
            }
        }
        
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
    
    public int ApplyIncomingDamage(int dmg)
    {
        float damage = dmg;
        
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            damage = modifiers[i].ModifyIncoming(damage);
        }
        
        return Mathf.RoundToInt(damage);
    }
    
    public bool ReduceHP(IBattleCharacter monster, DamageType type, int amount)
    {
        BattleEventManager.CallEvent(new ReceiveDamageEventArgs(monster, type, amount));
        
        CurrentHP -= amount;
        return CurrentHP <= 0;
    }
}

[Serializable]
public struct StatBlock
{
    public long HP;
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
        multipleValue = value;
        endTime = Time.time + duration;
    }

    public float Value => multipleValue;
    public bool IsExpired => Time.time >= endTime;
    public float ModifyIncoming(float baseDamage)
    {
        return baseDamage * multipleValue;
    }

    public void ExtendDuration(float additionalTime)
    {
        endTime = Mathf.Max(endTime, Time.time + additionalTime);
    }
}

public class DamageOverTimeModifier : IDamageModifier
{
    public readonly float DamagePerSecond;
    private float endTime;
    private float tickTimer;

    public DamageOverTimeModifier(float dps, float duration)
    {
        DamagePerSecond = dps;
        endTime = Time.time + duration;
    }

    public bool IsExpired => Time.time >= endTime;
    public float ModifyIncoming(float baseDamage) => baseDamage;

    // 남은 지속시간 계산
    public float RemainingDuration => Mathf.Max(0, endTime - Time.time);

    // 지속시간 연장 메서드
    public void ExtendDuration(float additionalSeconds)
    {
        endTime = Mathf.Max(endTime, Time.time + additionalSeconds);
    }

    public int DamageTick(float deltaTime)
    {
        tickTimer += deltaTime;
        if (tickTimer >= 1f)
        {
            tickTimer -= 1f;
            return Mathf.RoundToInt(DamagePerSecond);
        }
        return 0;
    }
}
