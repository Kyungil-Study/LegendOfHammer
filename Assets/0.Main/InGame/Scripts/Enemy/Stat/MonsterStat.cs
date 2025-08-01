using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

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
                    existingAmp.ExtendDuration(newAmp.IsExpired ? 0 : (newAmp.endTime - Time.time));
                    return;
                }
            }
        }
        
        if (newModifier is DamageOverTimeModifier newDot)
        {
            foreach (var modifier in modifiers)
            {
                if (modifier is DamageOverTimeModifier existingDot
                    && Mathf.Approximately(existingDot.DamagePerSecond, newDot.DamagePerSecond))
                {
                    existingDot.ExtendDuration(newDot.RemainingDuration);
                    return;
                }
            }

            modifiers.Add(newDot);
            StartCoroutine(DoTCoroutine(newDot));
            return;
        }
        
        modifiers.Add(newModifier);
    }
    
    private IEnumerator DoTCoroutine(DamageOverTimeModifier dot)
    {
        float elapsed = 0f;
        
        while (elapsed < dot.RemainingDuration)
        {
            yield return new WaitForSeconds(1f);
    
            int damage = Mathf.RoundToInt(dot.DamagePerSecond);
            monster.TakeDamage(new TakeDamageEventArgs(monster, monster, DamageType.DoT, damage));
    
            elapsed += 1f;
        }
    
        modifiers.Remove(dot);
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