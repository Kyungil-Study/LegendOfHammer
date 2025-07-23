using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 발사체까지 쓰는 패턴에서만 상속
public abstract class ProjectileAttackBase : CoroutineAttackBase
{
    protected IEnumerator FireProjectiles(int count, float interval, Func<int, Vector2> aim,
        GameObject prefab, float speed, int damage, LayerMask mask)
    {
        for (int i = 0; i < count; i++)
        {
            var obj  = UnityEngine.Object.Instantiate(prefab, mMonster.transform.position, Quaternion.identity);
            var proj = obj.GetComponent<Projectile>();
            proj.Initialize(aim(i).normalized, speed, damage, mask, mMonster);

            if (interval > 0f)
            {
                yield return new WaitForSeconds(interval);
            }
            else
            {
                yield return null;
            }
        }
    }

    protected Vector2 Rotate(Vector2 vec, float deg)
    {
        float rad = deg * Mathf.Deg2Rad;
        float cosX  = Mathf.Cos(rad);
        float sinX  = Mathf.Sin(rad);
        return new Vector2(vec.x * cosX - vec.y * sinX, vec.x * sinX + vec.y * cosX);
    }
}