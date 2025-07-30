using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventArgs : System.EventArgs
{
    
}

public class SelectAugmentEventArgs : BattleEventArgs
{ 
    public Augment Data { get; private set; }
    public SelectAugmentEventArgs(Augment data)
    {
        this.Data = data;
    }
}



public class ReadyBattleEventArgs : BattleEventArgs
{
    public int StageIndex { get; private set; }
    public int MaxStageIndex { get; private set; }
    
    public ReadyBattleEventArgs(int stageIndex, int maxStageIndex)
    {
        this.StageIndex = stageIndex;
        this.MaxStageIndex = maxStageIndex;
    }
}

public class StartBattleEventArgs : BattleEventArgs
{
    public int StageIndex { get; private set; }
    
    public StartBattleEventArgs(int stageIndex)
    {
        this.StageIndex = stageIndex;
    }
}

public class EndBattleEventArgs : BattleEventArgs
{
    public bool IsVictory { get; private set; }
    public bool IsBoosDead { get; set; }
    public EndBattleEventArgs( bool isVictory, bool isBossDead)
    {
        this.IsVictory = isVictory;
        this.IsBoosDead = isBossDead;
    }
}

public enum DamageType
{
    Enemy,
    Normal,
    Wizard,
    Critical,
    Shield,
    DoT
}

public class TakeDamageEventArgs : BattleEventArgs
{
    public IBattleCharacter Attacker {get; set;}
    public IBattleCharacter Target {get; set; }
    public DamageType Type { get; set; } = DamageType.Normal;
    public int Damage {get;set; }
    
    public TakeDamageEventArgs(IBattleCharacter attacker, IBattleCharacter target, DamageType type, int damage)
    {
        this.Attacker = attacker;
        this.Target = target;
        this.Type = type;
        this.Damage = damage;
    }
}

public class ReceiveDamageEventArgs : BattleEventArgs
{
    public IBattleCharacter Self { get; set; }
    public int ActualDamage { get;set; }
    public DamageType Type { get;set; }
    
    public ReceiveDamageEventArgs(IBattleCharacter self, DamageType type, int actualDamage)
    {
        this.Self = self;
        this.Type = type;
        this.ActualDamage = actualDamage;
    }
}

public class AliveMonsterEventArgs : BattleEventArgs
{
    public Monster AliveMonster { get; set; }
    public int AlivePoint { get; set; } = 0; // 몬스터가 살아있을 때 증가하는 포인트

    public AliveMonsterEventArgs(Monster monster, int alivePoint)
    {
        this.AliveMonster = monster;
        this.AlivePoint = alivePoint;
    }
}

public class DeathEventArgs : BattleEventArgs
{
    // 사망시 사망한 위치에 사망 이펙트 처리하도록?
    public IBattleCharacter Target { get; private set; }

    public DeathEventArgs(IBattleCharacter target)
    {
        this.Target = target;
    }
}


public class NextPageEventArgs : BattleEventArgs
{
    public NextPageEventArgs()
    {
    }
}

public class ChargeCollisionArgs : BattleEventArgs
{
    public IBattleCharacter Attacker { get; private set; }
    public IBattleCharacter Target { get; private set; }
    
    public float KnockBackDistance { get; private set; }

    public ChargeCollisionArgs(IBattleCharacter attacker, IBattleCharacter target, float distance)
    {
        this.Attacker = attacker;
        this.Target = target;
        this.KnockBackDistance = distance;
    }
}