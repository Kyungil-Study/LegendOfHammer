using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("중앙선 끝점 (체공형 발사지점)")]
    public Transform MidLeft;
    public Transform MidRight;

    [Header("맵 경계 X값 (지그재그 충돌처리)")]
    public float LeftBoundX;
    public float RightBoundX;

    void Awake()
    {
        Instance = this;
        
        LeftBoundX  = MidLeft.position.x;
        RightBoundX = MidRight.position.x;
    }
}