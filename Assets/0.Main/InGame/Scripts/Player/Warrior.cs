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
    
    protected override void Attack()
    {
        
    }

    // TODO: Implement Warrior's specific damage calculation logic.
    protected override int CalculateDamage()
    {
        return 0;
    }
    
    public void ChargeAttack(Vector3 direction, float distance)
    {
        isCharging = true;
        
        Vector3 endPosition = transform.position + direction.normalized * distance;
        StartCoroutine(ChargeCoroutine(endPosition));
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
    }
}
