using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MonsterScale : MonoBehaviour
{
    [Header("몬스터 모델 오브젝트 연결 (자식)")]
    [SerializeField] private Transform model;
    [SerializeField] [Tooltip("적 Sprite, 랜덤 생성")]
    private Sprite[] mSprites;
    [SerializeField] [Tooltip("적 애니메이션 클립, 스프라이트와 매핑시키기")]
    private AnimationClip[] mAnimationClips;
    
    private SpriteRenderer mSpriteRenderer;
    private BoxCollider2D  mCollider;

    private int mSpriteIndex;
    
    private void Awake()
    {
        mSpriteRenderer = model.GetComponent<SpriteRenderer>();
        mCollider = GetComponent<BoxCollider2D>();
        
    }

    private void Start()
    {
        var monster = GetComponent<Monster>();
        var data    = EnemyDataManager.Instance.Records[monster.EnemyID];
        int pixelSize;
        
        switch (data.Enemy_Rank)
        {
            case EnemyRank.Elite: pixelSize = 160; break;
            case EnemyRank.Boss:  pixelSize = 240; break;
            default:              pixelSize =  72; break;
        }

        float origPixels = mSpriteRenderer.sprite.rect.width;
        float scaleFactor = pixelSize / origPixels;
        model.localScale = Vector3.one * scaleFactor;

        Rect texRect = mSpriteRenderer.sprite.textureRect;
        float pixelsPerUnit = mSpriteRenderer.sprite.pixelsPerUnit;
        
        float localWidth = texRect.width  / pixelsPerUnit;
        float localHeight = texRect.height / pixelsPerUnit;

        mCollider.size = new Vector2(localWidth * scaleFactor, localHeight * scaleFactor);
        
        // 콜라이더 Offset 맞추기
        
        // Sprite 영역 픽셀에 맞게 자르기
        Vector2 rectSizePx   = new Vector2(texRect.width, texRect.height);
        Vector2 rectCenterPx = rectSizePx * 0.5f;

        // 픽셀 오프셋과 스프라이트 오프셋 차이
        Vector2 pivotPx      = mSpriteRenderer.sprite.pivot;
        Vector2 offsetPx     = rectCenterPx - pivotPx;

        // 픽셀 → 유닛 단위
        Vector2 offsetUnits  = offsetPx / pixelsPerUnit;

        // 스케일 적용 & 모델 위치 보정
        Vector2 modelPos2D   = (Vector2)model.localPosition;
        Vector2 finalOffset  = offsetUnits * scaleFactor + modelPos2D;

        // 콜라이더 Offset에 적용
        mCollider.offset     = finalOffset;
    }
}