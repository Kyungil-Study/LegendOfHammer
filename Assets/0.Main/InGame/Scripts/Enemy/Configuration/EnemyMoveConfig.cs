using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StraightMoveConfig
{
    // 직선 이동은 보통 별도 값이 필요 없으니 비워둠 (추가할 게 생기면 여기로)
}

[Serializable]
public class ZigzagMoveConfig
{
    public float amplitude = 1f;
    public float frequency = 2f;
    public float offsetX   = 0.5f;
}

[Serializable]
public class ChaseMoveConfig
{
    public float detectRange = 0.5f;
}

[Serializable]
public class FlyingMoveConfig
{
    public float distanceToStop = 3f;
}
