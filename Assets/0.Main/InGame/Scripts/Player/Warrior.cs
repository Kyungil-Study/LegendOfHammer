using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Warrior : Hero
{
    public float chargeDistance = 2f;
    public float chargeDuration = 0.2f;
    public float chargeKnockbackDistance = 1f;
    public bool isCharging = false;
    
    // 전사 돌진 피해량
    // [{(전사 기본 공격 피해량 x 치명타 피해량) + 타격 당 데미지} x 받는 피해량 증가] x 최종 데미지 증가
    protected override int CalculateDamage(bool isCritical = false)
    {
        float critFactor = isCritical ? baseStats.CriticalDamage : 1f;
        return (int)(((baseAttackDamage * critFactor) + baseStats.BonusDamagePerHit) * baseStats.FinalDamageFactor);
    }
    
    public void ChargeAttack(Vector3 direction)
    {
        // if(attackCooldown > 0 || isCharging)
        // {
        //     return;
        // }
        
        isCharging = true;
        
        Vector3 endPosition = transform.position + direction.normalized * (Squad.BASE_MOVE_SPEED * baseStats.MoveSpeed * chargeDistance);
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
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isCharging = false;
    }
    
    // TODO: Implement charge knockback and damage
}
