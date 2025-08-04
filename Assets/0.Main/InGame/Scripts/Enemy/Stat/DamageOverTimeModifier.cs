using UnityEngine;
// 도트딜 디버프
public class DamageOverTimeModifier : IDamageModifier
{
    public readonly float DamagePerSecond;
    private float endTime;
    private float tickTimer;

    public DamageOverTimeModifier(float dps, float duration)
    {
        DamagePerSecond = dps;
        endTime = Time.time + duration;
    }

    public bool IsExpired => Time.time >= endTime;
    public float ModifyIncoming(float baseDamage) => baseDamage;
    public float RemainingDuration => Mathf.Max(0, endTime - Time.time);
    public void ExtendDuration(float additionalSeconds)
    {
        endTime = Mathf.Max(endTime, Time.time + additionalSeconds);
    }

    public int DamageTick(float deltaTime)
    {
        tickTimer += deltaTime;
        if (tickTimer >= 1f)
        {
            tickTimer -= 1f;
            return Mathf.RoundToInt(DamagePerSecond);
        }
        return 0;
    }
}