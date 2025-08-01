using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
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
        [field:LabelText("용사단 체력"),SerializeField] public int MaxHealth { get; set; }
        
        [field:LabelText("용사단 추가 공격력 계수"),SerializeField] public float AttackDamageFactor { get; set; }
        [field:LabelText("용사단 추가 공속 감소 계수"),SerializeField] public float DecreaseAttackSpeed { get; set; } = 1;
        [field:LabelText("용사단 이동 속도"),SerializeField] public float MoveSpeed { get; set; } = 1;
        [field:LabelText("치명타 확률"),SerializeField] public float CriticalChance { get; set; } = 0;
        [field:LabelText("치명타 피해"),SerializeField] public float CriticalDamage { get; set; } = 1.5f;
        [field:LabelText("추가 타격"),SerializeField] public int BonusDamagePerHit { get; set; } = 0;
        [field:LabelText("용사단 받는 피해 증가"),SerializeField] public float TakeDamageFactor { get; set; } = 0;
        [field:LabelText("최종 데미지 증가"),SerializeField] public float FinalDamageFactor { get; set; } = 1;
    }

    public SquadStats stats = new SquadStats();
    public Warrior warrior;
    public List<object> invincible = new List<object>();
    [LabelText("피격 무적 시간")] public float hitInvincibleDuration = 1f;
    [LabelText("무적인가요?"),ShowInInspector] public bool IsInvincible => invincible.Count > 0;

    private void Awake()
    {
        stats.CurrentHealth = stats.MaxHealth;
        BattleEventManager.RegistEvent<StartBattleEventArgs>(OnStartBattle);
    }

    private void OnStartBattle(StartBattleEventArgs args)
    {
        AugmentInventory.Instance.ApplyAugmentsToSquad(this);
    }

    public void TakeDamage(TakeDamageEventArgs eventArgs)
    {
        if (IsInvincible)
        {
            return;
        }
        
        int damage = eventArgs.Damage;
        damage += Mathf.RoundToInt(damage * stats.TakeDamageFactor);
        stats.CurrentHealth -= damage;
        BattleEventManager.CallEvent(new ReceiveDamageEventArgs(this, DamageType.Enemy, eventArgs.Damage));
        ApplyInvincibility("HitInvincible", hitInvincibleDuration); 
        SoundManager.Instance.PlayPlayerDamaged();
    }

    public SpriteRenderer[] squadSprites;
    public void ApplyInvincibility(object _tag, float duration)
    {
        StartCoroutine(InvincibleCoroutine());
        return;

        IEnumerator InvincibleCoroutine()
        {
            foreach (SpriteRenderer sprite in squadSprites)
            {
                sprite.color = new Color(1,1,1,0.4f);
            }
            invincible.Add(_tag);
            yield return new WaitForSeconds(duration);
            invincible.Remove(_tag);
            foreach (SpriteRenderer sprite in squadSprites)
            {
                sprite.color = new Color(1,1,1,1f);
            }
        }
    }

    private TaskCompletionSource<bool> m_ReviveTcs;
    private async void Die()
    {
        Time.timeScale = 0;
        await WaitReviveChoice();
        Time.timeScale = 1f;

        if (m_ReviveTcs.Task.Result)
        {
            Revive();
        }
        else
        {
            BattleEventManager.CallEvent(new DeathEventArgs(this));
        }
    }

    private Task<bool> WaitReviveChoice()
    {
        m_ReviveTcs = new TaskCompletionSource<bool>();
        return m_ReviveTcs.Task;
    }

    private void Revive()
    {
        Instance.stats.CurrentHealth = stats.MaxHealth;
    }

    public void ChooseRevive()
    {
        if (m_ReviveTcs == null)
        {
            Debug.Log("[Squad] Revive Task Completion Source is null. Cannot set result.");
            return;
        }
        m_ReviveTcs.TrySetResult(true);
    }

    public void ChooseGiveUp()
    {
        if (m_ReviveTcs == null)
        {
            Debug.Log("[Squad] Revive Task Completion Source is null. Cannot set result.");
            return;
        }
        m_ReviveTcs.TrySetResult(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (warrior.IsCharging && BattleManager.TryGetMonsterBy(other, out Monster monster))
        {
            warrior.Impact(monster);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            ChooseRevive();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            ChooseGiveUp();
        }
    }
}
