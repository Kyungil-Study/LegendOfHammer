using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Warrior : Hero
{
    public Image cooldownIndicator;
    public float chargeDistance = 2f;
    public float chargeDuration = 0.2f;
    private float ChargeSpeed => Squad.STANDARD_DISTANCE * chargeDistance / chargeDuration;
    public float chargeKnockbackDistance = 1f;
    private bool _isCharging = false;
    public bool IsCharging
    {
        get => _isCharging;
        set
        {
            _isCharging = value;
            if (_isCharging == false)
            {
                m_HitMonsters.Clear();
            }
        }
    }
    private Vector3 m_ChargeDirection;

    private void Start()
    {
        bAutoFire = false;
    }

    protected override void Update()
    {
        base.Update();
        if (cooldownIndicator != null)
        {
            cooldownIndicator.fillAmount = 1 - Mathf.Clamp01(leftCooldown / AttackCooldown);
        }
    }
    
    // 전사 돌진 피해량
    // [{(전사 기본 공격 피해량 x 치명타 피해량) + 타격 당 데미지} x 받는 피해량 증가] x 최종 데미지 증가
    protected override int CalculateDamage(bool isCritical = false)
    {
        float critFactor = isCritical ? baseStats.CriticalDamage : 1f;
        return (int)(((baseAttackDamage * critFactor) + baseStats.BonusDamagePerHit) * baseStats.FinalDamageFactor);
    }
    
    public void ChargeAttack(Vector3 direction)
    {
        Debug.Log("charge");
        if(leftCooldown > 0 || IsCharging)
        {
            Debug.Log("return");
            return;
        }
        
        IsCharging = true;
        m_ChargeDirection = direction.normalized;
        
        Vector3 endPosition = squad.transform.position + direction.normalized * (Squad.STANDARD_DISTANCE * chargeDistance);
        StartCoroutine(ChargeCoroutine(endPosition));
        ApplyCooldown();
    }

    private IEnumerator ChargeCoroutine(Vector3 endPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < chargeDuration)
        {
            float t = elapsedTime / chargeDuration;
            squad.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        IsCharging = false;
    }

    private List<Monster> m_HitMonsters = new List<Monster>();
    public void Impact(Monster monster)
    {
        if (m_HitMonsters.Contains(monster))
        {
            return;
        }
        m_HitMonsters.Add(monster);
        TakeDamageEventArgs eventArgs = new TakeDamageEventArgs(squad, monster, Damage);
        BattleEventManager.Instance.CallEvent(eventArgs);
        // TODO: monster.Knockback(Vector3 direction, float distance);
    }

    // 몬스터 넉백은 몬스터 쪽에서 처리하기로 함
    // private void ChargeKnockback(Monster monster)
    // {
    //     StartCoroutine(KnockbackCoroutine());
    //     IEnumerator KnockbackCoroutine()
    //     {
    //         Vector3 startPosition = monster.transform.position;
    //         Vector3 endPosition = monster.transform.position + m_ChargeDirection * Squad.STANDARD_DISTANCE;
    //         float elapsedTime = 0f;
    //         float duration = chargeDuration * 0.5f;
    //         while (elapsedTime < duration)
    //         {
    //             float t = elapsedTime / duration;
    //             transform.position = Vector3.Lerp(startPosition, endPosition, t);
    //             elapsedTime += Time.deltaTime;
    //             yield return null;
    //         }
    //     }
    // }
}
