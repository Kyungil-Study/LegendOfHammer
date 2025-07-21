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
    public EndBattleEventArgs( bool isVictory)
    {
        this.IsVictory = isVictory;
    }
}

public class TakeDamageEventArgs : BattleEventArgs
{
    public IBattleCharacter Attacker {get; set;}
    public IBattleCharacter Target {get; set; }
    public int Damage {get;set; }
    public TakeDamageEventArgs(IBattleCharacter attacker, IBattleCharacter target, int damage)
    {
        this.Attacker = attacker;
        this.Target = target;
        this.Damage = damage;
    }
}

public class ReceiveDamageEventArgs : BattleEventArgs
{
    public IBattleCharacter Self {get; set; }
    public int ActualDamage {get;set; }
    public ReceiveDamageEventArgs(IBattleCharacter self, int actualDamage)
    {
        this.Self = self;
        this.ActualDamage = actualDamage;
    }
}

public class AliveMonsterEventArgs : BattleEventArgs
{
    public IBattleCharacter Monster { get; private set; }

    public AliveMonsterEventArgs(IBattleCharacter monster)
    {
        this.Monster = monster;
    }
}

public class DeathEventArgs : BattleEventArgs
{
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