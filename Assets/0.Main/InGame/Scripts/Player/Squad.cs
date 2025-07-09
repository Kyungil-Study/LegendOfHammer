using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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
        [field:SerializeField] public float BonusEffectFactor { get; set; } = 0;
        [field:SerializeField] public float TakeDamageFactor { get; set; } = 1;
        [field:SerializeField] public float FinalDamageFactor { get; set; } = 1;
    }

    [Range(0,10)] public const float BASE_MOVE_SPEED = 3f;

    public SquadStats stats = new SquadStats();

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        stats.CurrentHealth -= eventArgs.Damage;
    }

    private void Die()
    {
        BattleEventManager.Instance.CallEvent(new DeathEventArgs(this));
    }
}
