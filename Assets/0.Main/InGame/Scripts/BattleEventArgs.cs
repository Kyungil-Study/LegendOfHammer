using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEventArgs : System.EventArgs
{
}


public class StartBattleEventArgs : BattleEventArgs
{
    public Player player;
    public Monster monster;

    public StartBattleEventArgs(Player player, Monster monster)
    {
        this.player = player;
        this.monster = monster;
    }
}

public class EndBattleEventArgs : BattleEventArgs
{
    public Player player;
    public Monster monster;
    public bool isVictory;

    public EndBattleEventArgs(Player player, Monster monster, bool isVictory)
    {
        this.player = player;
        this.monster = monster;
        this.isVictory = isVictory;
    }
}

