using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoSingleton<Squad>, IBattleCharacter
{
    /// <summary>
    /// <para>스탯마다 기준이 0, 1 이라 불편함<br />
    /// 기존 값을 대체하는냐 기존 값에 더해지냐</para>
    /// </summary>
    [System.Serializable]
    public class SquadStats
    {
        public int currentHealth;
        public int maxHealth;
        public float attackSpeed = 1;
        public float moveSpeed = 1;
        public float criticalChance = 0;
        public float criticalDamage = 1.5f;
        public int bonusDamagePerHit = 0;
        public float bonusEffectCoefficient = 0;
        public float takeDamageCoefficient = 1;
        public float finalDamageCoefficient = 1;
    }

    public SquadStats stats = new SquadStats();
    public Warrior warrior;

    public void TakeDamage(IBattleCharacter damageFrom, int damage)
    {
        TakeDamageEventArgs eventArgs = new TakeDamageEventArgs(
            damageFrom,
            this,
            damage
        );
        BattleEventManager.Instance.CallEvent(eventArgs);
    }

    // TODO: UNDONE
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IBattleCharacter enemy))
        {
            if (warrior.isCharging)
            {
                
            }
        }
    }
}
