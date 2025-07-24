using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NormalAttackConfig
{
    // 공격 안 함. 필요하면 비워둠
}

[Serializable]
public class SuicideAttackConfig
{
    public float delay       = 3f;
    public float attackRange = 0.75f;
    public float exlposionInverval = 0.2f;
    public GameObject explosionPrefab;
}

[Serializable]
public class ShieldConfig
{
    public float radius      = 1.2f;
    public float angleDeg    = 90f;
    public Vector2 pivotOffset;
}

[Serializable]
public class SniperAttackConfig
{
    public int   burstCount    = 3;
    public float burstInterval = 0.15f;
    public float fireInterval  = 3f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
}

[Serializable]
public class SpreadAttackConfig
{
    public int   burstCount     = 3;
    public float burstInterval  = 0.2f;

    public float[] leftAngles  = { 0f, -15f, -30f, -45f };
    public float[] rightAngles = { 0f,  15f,  30f,  45f };

    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
}

[Serializable]
public class RadialAttackConfig
{
    public int   projectileCount = 12;
    public float betweenAngle    = 30f;
    public float fireInterval    = 3f;

    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
}

[Serializable]
public class FlyingAttackConfig
{
    public float fireInterval = 1f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;
}
