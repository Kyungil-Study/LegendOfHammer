using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovementFactory
{
    
    public static IMoveBehaviour Create(EnemyMovementPattern type, Monster monster)
    {
        return type switch
        {
            EnemyMovementPattern.Straight => new StraightMove(),
            EnemyMovementPattern.Zigzag   => new ZigzagMove(monster.ZigzagConfig),
            EnemyMovementPattern.Chase    => new ChaseMove(monster.ChaseConfig),
            EnemyMovementPattern.Flying   => new FlyingMove(monster.FlyingMoveConfig),
            _ => null
        };
    }
}

public static class AttackFactory
{
    public static IAttackBehaviour Create(EnemyAttackPattern type, Monster monster)
    {
        return type switch
        {
            EnemyAttackPattern.Normal  => new NormalAttack(),
            EnemyAttackPattern.Suicide => new SuicideAttack(monster.SuicideCfg),
            EnemyAttackPattern.Shield  => new ShieldAttack(monster.ShieldCfg),
            EnemyAttackPattern.Sniper  => new SniperAttack(monster.SniperCfg),
            EnemyAttackPattern.Spread  => new SpreadAttack(monster.SpreadCfg),
            EnemyAttackPattern.Radial  => new RadialAttack(monster.RadialCfg),
            EnemyAttackPattern.Flying  => new FlyingAttack(monster.FlyingAtkCfg),
            _ => null
        };
    }
}