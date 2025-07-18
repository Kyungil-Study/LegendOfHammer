using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct Distance
{
    public const float STANDARD_DISTANCE = 2.8f;
    [field:SerializeField] private float Value { get; set; }
    public Distance(float value)
    {
        Value = value;
    }
}

public class Squad : MonoSingleton<Squad>, IBattleCharacter
{
    // 스탯마다 기준이 0, 1 이라 불편함
    // 기존 값을 대체하는냐 기존 값에 더해지냐
    [System.Serializable]
    public class SquadStats
    {
        private int _currentHealth;
        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
                if (_currentHealth == 0)
                {
                    Squad.Instance.Die();
                }
            }
        }
        [field:SerializeField] public int MaxHealth { get; set; }
        [field:SerializeField] public float AttackSpeed { get; set; } = 1;
        [field:SerializeField] public float MoveSpeed { get; set; } = 1;
        [field:SerializeField] public float CriticalChance { get; set; } = 0;
        [field:SerializeField] public float CriticalDamage { get; set; } = 1.5f;
        [field:SerializeField] public int BonusDamagePerHit { get; set; } = 0;
        [field:SerializeField] public float TakeDamageFactor { get; set; } = 1;
        [field:SerializeField] public float FinalDamageFactor { get; set; } = 1;
    }

    public SquadStats stats = new SquadStats();
    public Warrior warrior;
    public List<string> invincible = new List<string>();
    public bool IsInvincible => invincible.Count > 0;

    protected override void Awake()
    {
        stats.CurrentHealth = stats.MaxHealth;
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        if (IsInvincible)
        {
            return;
        }
        stats.CurrentHealth -= eventArgs.Damage;
        BattleEventManager.Instance.CallEvent(new ReceiveDamageEventArgs(this, eventArgs.Damage));
    }

    private void Die()
    {
        BattleEventManager.Instance.CallEvent(new DeathEventArgs(this));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (warrior.IsCharging && BattleManager.TryGetMonsterBy(other, out Monster monster))
        {
            warrior.Impact(monster);
        }
    }
}
