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
            _ => null
        };
    }
}