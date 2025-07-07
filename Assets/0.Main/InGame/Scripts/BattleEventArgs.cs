using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventArgs : System.EventArgs
{
}


public class StartBattleEventArgs : BattleEventArgs
{
    public StartBattleEventArgs()
    {
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

public class DeathEventArgs : BattleEventArgs
{
    public IBattleCharacter Target { get; private set; }

    public DeathEventArgs(IBattleCharacter target)
    {
        this.Target = target;
    }
}
