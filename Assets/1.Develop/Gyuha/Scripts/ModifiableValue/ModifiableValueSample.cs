using System;
using UnityEngine;

namespace Jay
{
    public class ModifiableValueSample : MonoBehaviour
    {
        private void Start()
        {
            StatValueFloat atkDamage = new StatValueFloat(100);
            
            // 무기
            // 장신구
            // 버프
            // 디버프
    
            AdditiveFloat weapon = new AdditiveFloat(null, "Weapon", "Sword000", 150);
            atkDamage.Additive.AddModifier(weapon);
            
            Debug.Log($"Total : {atkDamage.Value}, BaseValue : {atkDamage.BaseValue}, " +
                      $"Additive : {atkDamage.Additive.Value}, Multiplicative : {atkDamage.Multiplicative.Value} | " +
                      $"{atkDamage.MultiplicativeAmount}");
            
            PercentBonusModifier ring = new PercentBonusModifier(null, "Ring", "Ring000", 0.2f);
            atkDamage.Multiplicative.AddModifier(ring);
            
            Debug.Log($"Total : {atkDamage.Value}, BaseValue : {atkDamage.BaseValue}, " +
                      $"Additive : {atkDamage.Additive.Value}, Multiplicative : {atkDamage.Multiplicative.Value} |" +
                      $"{atkDamage.MultiplicativeAmount}");
            
            AdditiveFloat ring2 = new AdditiveFloat(null, "Ring", "Ring001", 100);
            atkDamage.Additive.AddModifier(ring2);
            
            Debug.Log($"Total : {atkDamage.Value}, BaseValue : {atkDamage.BaseValue}, " +
                      $"Additive : {atkDamage.Additive.Value}, Multiplicative : {atkDamage.Multiplicative.Value} | " +
                      $"{atkDamage.MultiplicativeAmount}");
            
            PercentBonusModifier atkUp = new PercentBonusModifier(null, "Buff", "Buff000", 0.1f);
            atkDamage.Multiplicative.AddModifier(atkUp);
            
            Debug.Log($"Total : {atkDamage.Value}, BaseValue : {atkDamage.BaseValue}, " +
                      $"Additive : {atkDamage.Additive.Value}, Multiplicative : {atkDamage.Multiplicative.Value} | " +
                      $"{atkDamage.MultiplicativeAmount}");
            
            PercentBonusModifier atkDown = new PercentBonusModifier(null, "DeBuff", "DeBuff000", -0.2f);
            atkDamage.Multiplicative.AddModifier(atkDown);
            
            Debug.Log($"Total : {atkDamage.Value}, BaseValue : {atkDamage.BaseValue}, " +
                      $"Additive : {atkDamage.Additive.Value}, Multiplicative : {atkDamage.Multiplicative.Value} | " +
                      $"{atkDamage.MultiplicativeAmount}");
        }
    }
}

