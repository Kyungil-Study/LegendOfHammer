using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StatField<T>
{
    public bool Override;
    public T Value;
    
    public T Apply(T baseV) => Override? Value : baseV;
}

[Serializable]
public struct StatBlock
{
    public int HP, Atk;
    public float MoveSpeed, FireInterval, ProjectileSpeed;
}
public class MonsterStatComponent : MonoBehaviour
{
    [SerializeField] private EnemyID enemyID;

    public StatField<int>   HP;
    public StatField<int>   Atk;
    public StatField<float> MoveSpeed;
    public StatField<float> FireInterval;
    public StatField<float> ProjectileSpeed;

    public StatBlock Final { get; private set; }

    readonly List<IDamageModifier> modifiers = new();

    public void Initialize(int stage, Func<int,int,int> hpScaler)
    {
        var data = EnemyDataManager.Instance.Records[enemyID];
        
        var baseStat = new StatBlock 
        {
            HP = data.HP,
            Atk = data.Atk_Power,
            MoveSpeed = data.Move_Speed,
            FireInterval = 3f,     // TSV에 없으면 기본값
            ProjectileSpeed = 5f,
        };

        if (hpScaler != null) baseStat.HP = hpScaler(baseStat.HP, stage);

        Final = new StatBlock 
        {
            HP              = HP.Apply(baseStat.HP),
            Atk             = Atk.Apply(baseStat.Atk),
            MoveSpeed       = MoveSpeed.Apply(baseStat.MoveSpeed),
            FireInterval    = FireInterval.Apply(baseStat.FireInterval),
            ProjectileSpeed = ProjectileSpeed.Apply(baseStat.ProjectileSpeed),
        };
    }

    public int ApplyIncomingDamage(int raw)
    {
        float dmg = raw;
        for (int i = modifiers.Count-1; i>=0; i--)
        {
            dmg = modifiers[i].ModifyIncoming(dmg);
            if (modifiers[i].IsExpired) modifiers.RemoveAt(i);
        }
        return Mathf.RoundToInt(dmg);
    }

    public void AddModifier(IDamageModifier mod) => modifiers.Add(mod);
    public EnemyID ID => enemyID;
}

public interface IDamageModifier{ float ModifyIncoming(float baseDamage); bool IsExpired{get;} }
