using UnityEngine;
// 딜감 디버프
public class DamageAmpModifier : IDamageModifier
{
    private readonly float multipleValue;
    public float endTime;

    public DamageAmpModifier(float value, float duration)
    {
        multipleValue = value;
        endTime = Time.time + duration;
    }

    public float Value => multipleValue;
    public bool IsExpired => Time.time >= endTime;
    public float ModifyIncoming(float baseDamage)
    {
        return baseDamage * multipleValue;
    }

    public void ExtendDuration(float additionalTime)
    {
        endTime = Mathf.Max(endTime, Time.time + additionalTime);
    }
}